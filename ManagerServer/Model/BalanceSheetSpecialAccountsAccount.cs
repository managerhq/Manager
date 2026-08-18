using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("ef49facb-203b-4b45-aebd-99af4645700b")]
    [Singleton]
    public sealed class BalanceSheetSpecialAccountsAccount : NamedObject, IBalanceSheetAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, IPurchaseInvoiceAccount, ISalesInvoiceAccount, ICode
    {
        [Guide("Name of account. The default name is `SpecialAccounts` but it can be renamed.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.SpecialAccounts))] public string Name { get; set; }
        [Guide("Enter code of the account if desired")]
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select group on `BalanceSheet` under which this account should be presented.")]
        [ProtoMember(3), NoWrap, Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [Guide("Select group on `CashFlowStatement` under which this account should be presented.")]
        [ProtoMember(13), NoWrap, EmptyLabel, Prepend(nameof(Strings.CashFlowStatement))] public CashFlowStatementCategory CashFlowStatement { get; set; }
        [ProtoMember(14), NoWrap, EmptyLabel, Autocomplete(typeof(CashFlowStatementOperatingActivityGroup)), IfEnum(nameof(CashFlowStatement), (int)CashFlowStatementCategory.OperatingActivities)] public Guid? CashFlowStatementOperatingActivityGroup { get; set; }
        [ProtoMember(15), NoWrap, EmptyLabel, Autocomplete(typeof(CashFlowStatementFinancingActivityGroup)), IfEnum(nameof(CashFlowStatement), (int)CashFlowStatementCategory.FinancingActivities)] public Guid? CashFlowStatementFinancingActivityGroup { get; set; }
        [ProtoMember(16), EmptyLabel, Autocomplete(typeof(CashFlowStatementInvestingActivityGroup)), IfEnum(nameof(CashFlowStatement), (int)CashFlowStatementCategory.InvestingActivities)] public Guid? CashFlowStatementInvestingActivityGroup { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.SpecialAccounts;
            return Name;
        }

        public string NameWithCode
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + GetName();
                else return GetName();
            }
        }

        public override string GetCodeAndName()
        {
            return NameWithCode;
        }        

        public string GetCode()
        {
            return Code;
        }

        Guid IGeneralLedgerAccount.Key => Key;
        string IGeneralLedgerAccount.Name => Name;
        string IGeneralLedgerAccount.Code => Code;
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatement;
        string ICode.Code => Code;

        public Guid? GetCashFlowStatementGroup()
        {
            if (CashFlowStatement == CashFlowStatementCategory.OperatingActivities) return CashFlowStatementOperatingActivityGroup;
            if (CashFlowStatement == CashFlowStatementCategory.InvestingActivities) return CashFlowStatementInvestingActivityGroup;
            if (CashFlowStatement == CashFlowStatementCategory.FinancingActivities) return CashFlowStatementFinancingActivityGroup;
            return null;
        }
    }
}
