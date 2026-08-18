using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model.Obsolete.Obsolete18
{
    [ProtoContract]
    [Guid("c1666d5c-0363-408d-9ecd-66b75db8e538")]
    internal sealed class ControlAccount18 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public ChartOfAccountsCategory18? Category;
        [ProtoMember(3)]
        public decimal StartingBalance;
        [ProtoMember(4)]
        public DebitCredit StartingBalanceType;
        [ProtoMember(6)]
        public int? Code;
        [ProtoMember(7)]
        public Guid? ClassifiedBalanceSheetAssetGroup;
        [ProtoMember(8)]
        public Guid? ClassifiedBalanceSheetLiabilityGroup;
        [ProtoMember(9)]
        public Guid? MultiStepIncomeStatementGroup;
        [ProtoMember(10)]
        public Guid? TaxCode;
        [ProtoMember(11)]
        public int? Position;
        [ProtoMember(12)]
        public bool HasStartingBalance;
    }
}
