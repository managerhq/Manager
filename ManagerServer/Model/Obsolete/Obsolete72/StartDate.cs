using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete72
{
    [ProtoContract]
    [Singleton]
    [Guid("76ed5255-f1a8-463a-8132-2b58c68300c1")]
    public sealed class StartDate : Object
    {
        [ProtoMember(1)] public DateTime? Date;
        [ProtoMember(2)] public bool Obsolete_Enabled;
        [ProtoMember(3)] public bool StartingBalancesFixed;
    }
}
