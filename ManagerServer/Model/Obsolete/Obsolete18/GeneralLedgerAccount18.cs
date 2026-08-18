using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Model.Enums;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete18
{
    [ProtoContract]
    [Guid("c5e1f94a-c15a-400f-9428-e4ccbd2aaf47")]
    internal sealed class GeneralLedgerAccount18 : Object
    {
        [ProtoMember(3)]
        public string Name;
        [ProtoMember(5)]
        public ChartOfAccountsCategory18 Category;
        [ProtoMember(6)]
        public string Description;
        [ProtoMember(8)]
        public decimal StartingBalance;
        [ProtoMember(10)]
        public DebitCredit StartingBalanceType;
        [ProtoMember(11)]
        public int? Code;
        [ProtoMember(12)]
        public Guid? Currency;
        [ProtoMember(13)]
        public Guid? TaxCode;
        [ProtoMember(14)]
        public Guid? ClassifiedBalanceSheetAssetGroup;
        [ProtoMember(15)]
        public Guid? ClassifiedBalanceSheetLiabilityGroup;
        [ProtoMember(16)]
        public Guid? MultiStepIncomeStatementGroup;
        [ProtoMember(17)]
        public int? Position;
        [ProtoMember(18)]
        public Dictionary<Guid, string> CustomFields;
        [ProtoMember(19)]
        public bool HasStartingBalance;
        [ProtoMember(7)]
        public bool Obsolete_HasOpeningBalance;
        [ProtoMember(9)]
        public DateTime Obsolete_OpeningBalanceDate;
    }
}
