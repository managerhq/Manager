using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("dac7ba37-0ccd-45e5-906e-548e6c50df37")]
    [Singleton]
    public sealed class BalanceSheetAccountsPayableAccount : NamedObject, IBalanceSheetAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, ICode
    {
        [Guide("Enter the name for this control account that tracks amounts owed to suppliers.")]
        [Guide("The default name is `AccountsPayable` but you can customize it to match your business terminology.")]
        [Guide("This account aggregates all unpaid supplier invoices and is essential for monitoring cash flow obligations.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.AccountsPayable))] public string Name { get; set; }
        [Guide("Enter an optional account code to organize your chart of accounts systematically.")]
        [Guide("Account codes help with sorting accounts and can follow your existing numbering system.")]
        [Guide("Common codes for accounts payable range from 2000-2999 in many accounting systems.")]
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select the balance sheet group where this liability account should appear in financial reports.")]
        [Guide("Accounts payable typically belongs under current liabilities as these are short-term obligations.")]
        [Guide("The grouping affects how your balance sheet is organized and subtotaled.")]
        [ProtoMember(3), NoWrap, Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [Guide("Select how changes in accounts payable should be classified on the cash flow statement.")]
        [Guide("Increases in accounts payable represent cash retained (positive cash flow from operations).")]
        [Guide("Decreases indicate cash paid to suppliers (negative cash flow from operations).")]
        [Guide("This classification is crucial for accurate cash flow analysis using the indirect method.")]
        [ProtoMember(13), EmptyLabel, Autocomplete(typeof(CashFlowStatementOperatingActivityGroup)), Prepend(nameof(Strings.CashFlowStatement), "-", nameof(Strings.OperatingActivities))] public Guid? CashFlowStatementGroup { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.AccountsPayable;
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
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.OperatingActivities;
        public Guid? GetCashFlowStatementGroup() { return CashFlowStatementGroup; }
        string ICode.Code => Code;
    }
}
