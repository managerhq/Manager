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

                // One file per locale, named by locale code, mirroring Assets/Translations.
                foreach (var file in EmbeddedAssets.GetDirectory("Localizations"))
                {
                    if (file.IsDirectory) continue;
                    if (!file.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) continue;

                    string json;
                    using (var stream = file.CreateReadStream())
                    using (var reader = new StreamReader(stream))
                    {
                        json = reader.ReadToEnd();
                    }

                    var source = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<Guid, Dictionary<Guid, object>>>(json);
                    if (source == null) continue;

                    var list = new List<ManagerServer.Model.Object>();
                    foreach (var e in source)
                    {
                        var type = ManagerServer.Model.Object.GetTypeByGuid(e.Key);
                        if (type == null) continue;

                        foreach (var e2 in e.Value)
                        {
                            var o = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(e2.Value), type, jsonSettings) as ManagerServer.Model.Object;
                            if (o != null)
                            {
                                o.Key = e2.Key;
                                list.Add(o);
                            }
                        }
                    }

                    Json.Add(Path.GetFileNameWithoutExtension(file.Name), list.ToArray());
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