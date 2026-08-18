using System;
using ProtoBuf;

namespace ManagerServer
{
    [ProtoContract]
    public sealed class UserSession
    {
        [ProtoMember(1)] public Guid Key;
        [ProtoMember(2)] public DateTime Timestamp = DateTime.UtcNow;
        [ProtoMember(3)] public string UserAgent;
        [ProtoMember(4)] public string Location;
    }
}
