using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("d1489e95-bb28-4f5d-b42e-67d3291b3893")]
    [Singleton]
    public sealed class BalanceSheetAccountsReceivableAccount : NamedObject, IBalanceSheetAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, ICode
    {
        [Guide("Enter the name for this control account that tracks amounts owed by customers.")]
        [Guide("The default name is `AccountsReceivable` but you can customize it to match your business terminology.")]
        [Guide("This account aggregates all unpaid customer invoices and is crucial for managing cash collections.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.AccountsReceivable))] public string Name { get; set; }
        [Guide("Enter an optional account code to organize your chart of accounts systematically.")]
        [Guide("Account codes help with sorting accounts and can follow your existing numbering system.")]
        [Guide("Common codes for accounts receivable range from 1200-1299 in many accounting systems.")]
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select the balance sheet group where this asset account should appear in financial reports.")]
        [Guide("Accounts receivable typically belongs under current assets as these are short-term receivables.")]
        [Guide("The grouping affects how your balance sheet is organized and subtotaled.")]
        [ProtoMember(3), NoWrap, Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [Guide("Select how changes in accounts receivable should be classified on the cash flow statement.")]
        [Guide("Increases in accounts receivable represent cash not yet collected (negative cash flow from operations).")]
        [Guide("Decreases indicate cash collected from customers (positive cash flow from operations).")]
        [Guide("This classification is essential for accurate cash flow analysis using the indirect method.")]
        [ProtoMember(13), EmptyLabel, Autocomplete(typeof(CashFlowStatementOperatingActivityGroup)), Prepend(nameof(Strings.CashFlowStatement), "-", nameof(Strings.OperatingActivities))] public Guid? CashFlowStatementGroup { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.AccountsReceivable;
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
