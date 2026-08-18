using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("352897d1-e7fe-462e-9965-458ed9e27b82")]
    [Singleton]
    public sealed class BalanceSheetInvestmentsAccount : NamedObject, IBalanceSheetAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, ICode
    {
        [Guide("Name of account. The default name is `InvestmentsAtCost` but it can be renamed.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.InvestmentsAtCost))] public string Name { get; set; }
        [Guide("Enter code of the account if desired")]
        [ProtoMember(2), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select group on `BalanceSheet` under which this account should be presented.")]
        [ProtoMember(3), NoWrap, Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [Guide("Select group on `CashFlowStatement` under which this account should be presented.")]
        [ProtoMember(4), EmptyLabel, Autocomplete(typeof(CashFlowStatementInvestingActivityGroup))] public Guid? CashFlowStatementInvestingActivityGroup { get; set; }
        [ProtoMember(5)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.Investments;
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
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.InvestingActivities;
        string ICode.Code => Code;

        public Guid? GetCashFlowStatementGroup()
        {
            return CashFlowStatementInvestingActivityGroup;
        }
    }
}
