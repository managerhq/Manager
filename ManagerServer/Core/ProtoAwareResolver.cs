using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ProtoBuf;

namespace ManagerServer
{
    public class ProtoAwareResolver : DefaultJsonTypeInfoResolver
    {
        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            var jsonTypeInfo = base.GetTypeInfo(type, options);

            if (jsonTypeInfo.Kind != JsonTypeInfoKind.Object)
            {
                return jsonTypeInfo;
            }

            var isProtoContract = type.GetCustomAttribute<ProtoContractAttribute>() != null;
            if (!isProtoContract)
            {
                return jsonTypeInfo;
            }

            var toRemove = new List<JsonPropertyInfo>();

            foreach (var prop in jsonTypeInfo.Properties)
            {
                var member = type.GetFieldOrProperty(prop.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);

                var hasProtoMember = member?.GetCustomAttribute<ProtoMemberAttribute>() != null;

                if (!hasProtoMember) toRemove.Add(prop);

                if (member != null && member.Name.StartsWith("Obsolete_"))
                {
                    toRemove.Add(prop);
                }
            }

            foreach (var prop in toRemove)
            {
                jsonTypeInfo.Properties.Remove(prop);
            }

            return jsonTypeInfo;
        }
    }
}
