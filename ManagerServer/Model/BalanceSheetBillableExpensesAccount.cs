using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("059dbfb9-1c80-4043-887f-0fc441099fe0")]
    [Singleton]
    public sealed class BalanceSheetBillableExpensesAccount : NamedObject, IBalanceSheetAccount, IJournalEntryAccount, IReceiptOrPaymentAccount, IPurchaseInvoiceAccount, ICode
    {
        [Guide("Enter the name for this account. The default name is `Billable_expenses`, but you can rename it to better suit your business needs.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.Billable_expenses))] public string Name { get; set; }
        [Guide("Optionally, enter an account code. Codes help organize accounts and can be used for searching and sorting in reports.")]
        [ProtoMember(12), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select the `BalanceSheet` group where this account should appear. This determines its placement on the balance sheet report.")]
        [ProtoMember(3), Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }
        [Guide("If you are using `TaxCodes`, you can select a default tax code that will be automatically applied when this account is selected in transactions.")]
        [ProtoMember(13), IfContains<TaxCode>, Label(nameof(Strings.Autofill), nameof(Strings.TaxCode))] public bool HasDefaultTaxCode { get; set; }
        [ProtoMember(8), IfTrue(nameof(HasDefaultTaxCode)), Autocomplete(typeof(TaxCode)), NoLabel, Short] public Guid? DefaultTaxCode { get; set; }
        [ProtoMember(11)] public int Position { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.Billable_expenses;
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
        string ICode.Code => Code;
    }
}
