using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ManagerServer.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GuidAttribute : Attribute
    {
        public Guid Value { get; set; }

        public GuidAttribute(string guid)
        {
            Value = new Guid(guid);
        }

        private static Dictionary<Guid, Type> types = typeof(ManagerServer.Model.Attributes.GuidAttribute).Assembly.GetTypes().Where(x => x.GetCustomAttribute<GuidAttribute>() != null).ToDictionary(x => x.GetCustomAttribute<ManagerServer.Model.Attributes.GuidAttribute>().Value, x => x);
        public static Type GetTypeByGuid(Guid key)
        {
            types.TryGetValue(key, out Type type);
            return type;
        }

        private static Dictionary<Type, Guid> guids = typeof(ManagerServer.Model.Attributes.GuidAttribute).Assembly.GetTypes().Where(x => x.GetCustomAttribute<GuidAttribute>() != null).ToDictionary(x => x, x => x.GetCustomAttribute<ManagerServer.Model.Attributes.GuidAttribute>().Value);
        public static Guid GetGuidByType(Type t)
        {
            return guids[t];
        }
    }
}
