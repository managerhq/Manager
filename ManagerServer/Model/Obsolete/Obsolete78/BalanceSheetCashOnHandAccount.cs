using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete78
{
    [ProtoContract]
    [Guid("a084e4be-981b-4b7e-8331-56b0eb3a6729")]
    [Singleton]
    public sealed class BalanceSheetCashOnHandAccount : Object
    {
        [ProtoMember(1)] public string Name;
        [ProtoMember(12)] public string Code;
        [ProtoMember(3)] public Guid? Group;
        [ProtoMember(11)] public int Position;
    }
}
