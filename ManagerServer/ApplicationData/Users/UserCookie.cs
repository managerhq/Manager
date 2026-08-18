
using System.IO;

namespace ManagerServer
{
    [ProtoContract]
    public sealed class UserCookie
    {
        [ProtoMember(1)] public string Username;
        [ProtoMember(2)] public Guid UserSession;
        [ProtoMember(3)] public string OnBehalfOf;

        public static UserCookie Deserialize(string cookie)
        {
            if (cookie == null) return null;
            try
            {
                var buffer = System.Buffers.Text.Base64Url.DecodeFromChars(cookie);
                using var ms = new MemoryStream(buffer);
                return ProtoBuf.Serializer.Deserialize<UserCookie>(ms);
            }
            catch (Exception) { }
            return null;
        }

        public string Serialize()
        {
            using var ms = new System.IO.MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, this);
            var buffer = ms.ToArray();
            var payload = System.Buffers.Text.Base64Url.EncodeToString(buffer);
            return payload;
        }
    }
}