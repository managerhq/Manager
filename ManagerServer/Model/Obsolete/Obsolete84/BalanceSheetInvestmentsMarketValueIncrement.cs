using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete84
{
    [ProtoContract]
    [Guid("ebdcff44-67e4-4ad3-9489-b27253347674")]
    [Singleton]
    public sealed class BalanceSheetInvestmentsMarketValueIncrement : Object
    {
        [ProtoMember(1)] public string Name;
        [ProtoMember(2)] public string Code;
        [ProtoMember(3)] public Guid? Group;
        [ProtoMember(4)] public Guid? CashFlowStatementInvestingActivityGroup;
        [ProtoMember(5)] public int Position;
    }
}
