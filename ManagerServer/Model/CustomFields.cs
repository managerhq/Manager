using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace ManagerServer.Model
{
    [ProtoContract]
    public sealed class CustomFields
    {
        [ProtoMember(1)] public Dictionary<Guid, string> Strings { get; set; }
        [ProtoMember(2)] public Dictionary<Guid, decimal?> Decimals { get; set; }
        [ProtoMember(3)] public Dictionary<Guid, DateTime?> Dates { get; set; }
        [ProtoMember(4)] public Dictionary<Guid, bool> Booleans { get; set; }
        [ProtoMember(5)] public Dictionary<Guid, string[]> StringArrays { get; set; }
        [ProtoMember(6)] public Dictionary<Guid, Guid?> Images { get; set; }

        public Guid[] GetKeys()
        {
            var keys = new List<Guid>();
            if (Strings != null) keys.AddRange(Strings.Keys);
            if (Decimals != null) keys.AddRange(Decimals.Keys);
            if (Dates != null) keys.AddRange(Dates.Keys);
            if (Booleans != null) keys.AddRange(Booleans.Keys);
            if (StringArrays != null) keys.AddRange(StringArrays.Keys);
            if (Images != null) keys.AddRange(Images.Keys);
            return keys.Distinct().ToArray();
        }

        public object GetValue(ICustomField customField)
        {
            if (customField is NumberCustomField && Decimals != null)
            {
                if (Decimals.TryGetValue(customField.Key, out decimal? value)) return value;
            }
            if (customField is DateCustomField && Dates != null)
            {
                if (Dates.TryGetValue(customField.Key, out DateTime? value)) return value;
            }
            if (customField is TextCustomField && Strings != null)
            {
                if (Strings.TryGetValue(customField.Key, out string value)) return value;
            }
            if (customField is CheckboxCustomField && Booleans != null)
            {
                if (Booleans.TryGetValue(customField.Key, out bool value)) return value;
            }
            if (customField is ImageCustomField && Images != null)
            {
                if (Images.TryGetValue(customField.Key, out Guid? value)) return value;
            }
            if (customField is MultipleValueCustomField && StringArrays != null)
            {
                if (StringArrays.TryGetValue(customField.Key, out string[] value)) return value;
            }
            return null;
        }

        public object GetValue(Guid customField)
        {
            if (Decimals != null)
            {
                if (Decimals.TryGetValue(customField, out decimal? value)) return value;
            }
            if (Dates != null)
            {
                if (Dates.TryGetValue(customField, out DateTime? value)) return value;
            }
            if (Strings != null)
            {
                if (Strings.TryGetValue(customField, out string value)) return value;
            }
            if (Booleans != null)
            {
                if (Booleans.TryGetValue(customField, out bool value)) return value;
            }
            if (Images != null)
            {
                if (Images.TryGetValue(customField, out Guid? value)) return value;
            }
            if (StringArrays != null)
            {
                if (StringArrays.TryGetValue(customField, out string[] value)) return value;
            }
            return null;
        }

        public void StripValues(Guid[] keys)
        {
            foreach (var e in GetKeys())
            {
                if (keys.Contains(e)) continue;

                Decimals?.Remove(e);
                Dates?.Remove(e);
                Strings?.Remove(e);
                Booleans?.Remove(e);
                Images?.Remove(e);
                StringArrays?.Remove(e);
            }
        }
    }
}
