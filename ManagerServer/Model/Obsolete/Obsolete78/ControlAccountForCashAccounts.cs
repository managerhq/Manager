using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model.Obsolete.Obsolete78
{
    [ProtoContract]
    [Guid("a3543f94-6c02-4050-95ff-ff2802a59ed4")]
    public sealed class ControlAccountForCashAccounts : Object
    {
        [ProtoMember(1)] public string Name;
        [ProtoMember(17)] public string Code;
        [ProtoMember(3)] public Guid? Group;
        [ProtoMember(16)] public int Position;
        [ProtoMember(18)] public bool Inactive;
    }
}
