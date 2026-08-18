using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete83
{
    [ProtoContract]
    [Guid("23bce57f-902d-4729-9f3f-ad657d853a4b")]
    [Singleton]
    public sealed class ProfitAndLossStatementAccountRealizedCurrencyGainsLosses : Object
    {
        [ProtoMember(1)] public string Name;
        [ProtoMember(11)] public string Code;
        [ProtoMember(3)] public Guid? Group;
        [ProtoMember(12)] public Guid? CashFlowStatementGroup;
        [ProtoMember(10)] public int Position;      
    }
}
