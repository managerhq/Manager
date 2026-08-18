using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete83
{
    [ProtoContract]
    [Guid("7c7906cc-1311-46be-a20b-5cdbbe7dce8a")]
    [Singleton]
    public sealed class ProfitAndLossStatementAccountUnrealizedCurrencyGainsLosses : Object
    {
        [ProtoMember(1)] public string Name;
        [ProtoMember(11)] public string Code;
        [ProtoMember(3)] public Guid? Group;
        [ProtoMember(12)] public Guid? CashFlowStatementGroup;
        [ProtoMember(10)] public int Position;
    }
}
