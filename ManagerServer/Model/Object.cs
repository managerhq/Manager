using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace ManagerServer.Model
{
    public abstract class Object : IObject, IComparable<Object>
    {
        public Guid Key { get; set; }
        [JsonIgnore] public long Timestamp { get; set; }

        Guid IObject.Key => Key;

        public Guid id => Key; // For Select2 component

        private static Dictionary<Guid, Type> types = typeof(ManagerServer.Model.Object).Assembly.GetTypes().Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(ManagerServer.Model.Object))).ToDictionary(x => x.GetCustomAttribute<ManagerServer.Model.Attributes.GuidAttribute>().Value, x => x);
        public static Type GetTypeByGuid(Guid key)
        {
            types.TryGetValue(key, out Type type);
            return type;
        }

        private static Dictionary<Type, Guid> guids = typeof(ManagerServer.Model.Object).Assembly.GetTypes().Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(ManagerServer.Model.Object))).ToDictionary(x => x, x => x.GetCustomAttribute<ManagerServer.Model.Attributes.GuidAttribute>().Value);

        public static Guid GetGuidByType(Type t)
        {
            return guids[t];
        }

        public Object DeepClone()
        {
            var deepClone = ProtoBuf.Serializer.NonGeneric.DeepClone(this) as Object;
            deepClone.Key = Key;
            deepClone.Timestamp = Timestamp;
            return deepClone;
        }

        public virtual bool IsInactive() => false;

        public int CompareTo(Object other)
        {
            return (IsInactive(), Key).CompareTo((other.IsInactive(), other.Key));
        }
    }
}