using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete78
{
    [ProtoContract]
    [Guid("7da84a1e-e6dc-4618-8cb6-e6e39e68de8a")]
    public sealed class CashAccountSummary : Object
    {
        [ProtoMember(1)] public Guid? CashAccount;
        [ProtoMember(2)] public Period[] Periods;
        [ProtoMember(3)] public bool AccountCodes;
        [ProtoMember(4)] public bool ExcludeZeroBalances;

        [ProtoContract]
        public sealed class Period
        {
            [ProtoMember(2)] public DateTime FromDate;
            [ProtoMember(3)] public DateTime ToDate;
        }
    }
}
