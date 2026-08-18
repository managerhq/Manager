using System;
using System.IO;
using System.Reflection;

namespace ManagerServer
{
    static class Serialization
    {
        internal static Tuple<Guid, byte[]> Serialize(object o)
        {
            if (o == null) throw new ArgumentNullException();

            var ms = new MemoryStream();
            ProtoBuf.Serializer.NonGeneric.Serialize(ms, o);
            var guid = o.GetType().GetCustomAttribute<ManagerServer.Model.Attributes.GuidAttribute>().Value;
            return new Tuple<Guid, byte[]>(guid, ms.ToArray());
        }

        internal static ManagerServer.Model.Object Deserialize(Guid contentType, ReadOnlySpan<byte> content, Guid key, long timestamp)
        {
            var o = Deserialize(contentType, content);
            if (o == null) return null;
            o.Key = key;
            o.Timestamp = timestamp;
            return o;
        }

        internal static ManagerServer.Model.Object Deserialize(Guid contentType, ReadOnlySpan<byte> content)
        {
            var type = ManagerServer.Model.Object.GetTypeByGuid(contentType);
            if (type == null) return null;
            if (content.Length == 0) return (ManagerServer.Model.Object)Activator.CreateInstance(type);

            var o = (ManagerServer.Model.Object)ProtoBuf.Serializer.NonGeneric.Deserialize(type, content);
            return o;
        }
    }
}
