using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace ManagerServer.Localizations
{
    public static class Localizations
    {
        public static Dictionary<string, ManagerServer.Model.Object[]> Json = new Dictionary<string, Model.Object[]>();

        public static ManagerServer.Model.Object[] Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return new ManagerServer.Model.Object[0];
            if (Json.TryGetValue(name, out Model.Object[] value)) return value ?? new ManagerServer.Model.Object[0];
            return new ManagerServer.Model.Object[0];
        }

        static Localizations()
        {
#if !DEBUG
            try
            {
#endif
                var jsonSettings = new JsonSerializerSettings();
                jsonSettings.ContractResolver = new CustomContractResolver();
                var localizationsJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Localizations.json");
                if (File.Exists(localizationsJson))
                {
                    var source = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Guid, Dictionary<Guid, object>>>>(File.ReadAllText(localizationsJson));

                    foreach (var e in source)
                    {
                        var list = new List<ManagerServer.Model.Object>();
                        foreach (var e2 in e.Value)
                        {
                            var type = ManagerServer.Model.Object.GetTypeByGuid(e2.Key);
                            if (type != null)
                            {
                                foreach (var e3 in e2.Value)
                                {
                                    var o = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(e3.Value), type, jsonSettings) as ManagerServer.Model.Object;
                                    if (o != null)
                                    {
                                        o.Key = e3.Key;
                                        list.Add(o);
                                    }
                                }
                            }
                        }
                        Json.Add(e.Key, list.ToArray());
                    }
                }
#if !DEBUG
            }
            catch { }
#endif
        }

        public class CustomContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var properties = base.CreateProperties(type, memberSerialization);
                properties = properties.Where(p => !p.PropertyName.StartsWith("Obsolete_")).ToList();
                foreach (var e in properties.ToArray())
                {
                    var protoMemberAttribute = e.AttributeProvider?.GetAttributes(false).OfType<ProtoBuf.ProtoMemberAttribute>().SingleOrDefault();
                    if (protoMemberAttribute == null)
                    {
                        properties.Remove(e);
                        continue;
                    }
                    e.PropertyName = protoMemberAttribute.Tag.ToString();
                }
                return properties;
            }
        }
    }
}