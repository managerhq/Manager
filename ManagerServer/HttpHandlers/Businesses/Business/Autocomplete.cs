using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    internal sealed class Autocomplete : BusinessHandler
    {
        [ProtoMember(1)] public Guid[] Types;
        [ProtoMember(2)] public string[] Expand;

        public override async Task Get()
        {            
            Response.ContentType = "application/json; charset=utf-8";

            if (!ApplicationData.Businesses.Exists(Business))
            {
                await Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    more = false,
                    results = Array.Empty<object>()
                }));
                return;
            }

            SetCulture(Business);

            var term = Request.Query["Term"].ToString();
            int.TryParse(Request.Query["Page"], out int page);

            Guid? filter = null;
            if (Guid.TryParse(Request.Query["Filter"], out Guid result))
            {
                filter = result;
            }

            // CustomFields
            if (Types != null && Types.Length == 1 && Types[0] == typeof(ManagerServer.Model.Attributes.CustomFieldsAttribute).GetCustomAttribute<ManagerServer.Model.Attributes.GuidAttribute>().Value)
            {
                await Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    more = false,
                    results = ManagerServer.Model.Attributes.CustomFieldsAttribute.All.Select(x => new
                    {
                        Key = x.GetCustomAttribute<ManagerServer.Model.Attributes.GuidAttribute>().Value,
                        Name = ManagerServer.Globalization.Strings.GetPropertyValue(x),
                        UniqueName = ManagerServer.Globalization.Strings.GetPropertyValue(x)
                    }).Where(x => string.IsNullOrWhiteSpace(term) || Search(x.UniqueName, term)).OrderBy(x => x.UniqueName)
                }));

                return;
            }

            var items = new List<ManagerServer.Model.NamedObject>();
            var types = Types ?? [];
            if (types.Length == 0 && filter.HasValue)
            {
                types = [ filter.Value ];
                filter = null;
            }

            ManagerServer.Model.Object filter2 = null;
            if (filter.HasValue)
            {
                filter2 = ApplicationData.Businesses.Get(Business).SingleOrDefault(filter.Value);
                if (filter2 == null)
                {
                    var type = ManagerServer.Model.Object.GetTypeByGuid(filter.Value);
                    if (type != null)
                    {
                        filter2 = (ManagerServer.Model.Object)Activator.CreateInstance(type);
                        filter2.Key = filter.Value;
                    }
                }
            }

            if (types != null)
            {
                foreach (var e in types)
                {
                    var type = ManagerServer.Model.Object.GetTypeByGuid(e);
                    if (type == null) continue;
                    if (!type.IsSubclassOf(typeof(ManagerServer.Model.NamedObject))) continue;

                    if (type.GetCustomAttribute<ManagerServer.Model.Attributes.SingletonAttribute>() != null)
                    {
                        var o = ApplicationData.Businesses.Get(Business).Single(e) as ManagerServer.Model.NamedObject;
                        if (o.IsActive(ApplicationData.Businesses.Get(Business)))
                        {
                            items.Add(o);
                        }
                    }
                    else
                    {
                        items.AddRange(ApplicationData.Businesses.Get(Business)
                            .UnorderedOfType(type)
                            .OfType<NamedObject>()
                            .Where(x => x.Key != e && !x.IsInactive() && x.OnAutocomplete(filter2)));
                    }
                }
            }

            items = items.Where(x => !string.IsNullOrWhiteSpace(x.UniqueName)).ToList();
            if (!string.IsNullOrWhiteSpace(term)) items = items.Where(x => Search(x.UniqueName, term)).ToList();
            items = items.OrderBy(x => x.UniqueName).Skip((page - 1) * 10).ToList();

            if (items.Any(x => x is ManagerServer.Model.IBankOrCashAccount))
            {
                var userPermissions = this.GetCurrentUserPermissions(Business).GetBankCashAccounts();
                if (userPermissions.Length > 0)
                {
                    foreach (var e in items.ToArray())
                    {
                        if (e is ManagerServer.Model.IBankOrCashAccount && !userPermissions.Contains(e.Key)) items.Remove(e);
                    }
                }
            }

            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings();
            jsonSettings.Converters.Add(new DateTimeConverter());
            if (Expand != null && Expand.Length > 0)
            {
                jsonSettings.Converters.Add(new GuidSerializer(ApplicationData.Businesses.Get(Business), Expand));
            }
            jsonSettings.Formatting = Formatting.Indented;

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                var serializer = JsonSerializer.Create(jsonSettings);

                serializer.Serialize(writer, new
                {
                    more = (items.Count >= 10),
                    results = items.Take(10).ToArray()
                });
            }

            foreach (var e in sb.GetChunks())
            {
                await Response.WriteAsync(e.ToString());
            }
        }

        private bool Search(string value, string term)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (string.IsNullOrWhiteSpace(term)) return false;
            return (value.IndexOf(term, StringComparison.OrdinalIgnoreCase) != -1);
        }

        public class GuidSerializer : JsonConverter
        {
            private ManagerServer.Database database;
            private string[] expand;

            public GuidSerializer(ManagerServer.Database database, string[] expand)
            {
                this.database = database;
                this.expand = expand;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var serializer2 = new JsonSerializer();

                var key = (Guid)value;

                var name = writer.Path.Split('.').Last();
                if (expand.Contains(name))
                {
                    var o = database.SingleOrDefault(key);
                    if (o != null)
                    {
                        serializer2.Serialize(writer, o);
                        return;
                    }

                    var type = ManagerServer.Model.Object.GetTypeByGuid((Guid)value);
                    if (type != null && type.GetCustomAttribute<ManagerServer.Model.Attributes.SingletonAttribute>() != null)
                    {
                        var o2 = (ManagerServer.Model.Object)Activator.CreateInstance(type);
                        o2.Key = (Guid)value;
                        serializer2.Serialize(writer, o2);
                        return;
                    }
                }

                serializer2.Serialize(writer, key);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType.Equals(typeof(Guid?));
            }
        }

        public class DateTimeConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                if (objectType == typeof(DateTime)) return true;
                if (objectType == typeof(DateTime?)) return true;
                return false;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var date = (DateTime)value;
                writer.WriteValue(((DateTime)value).ToString("yyyy-M-d"));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
