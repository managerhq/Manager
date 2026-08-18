using System;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Query.GeneralLedger;
using System.Collections.Generic;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("6ef13e42-ad89-4d42-9480-546e0c04a411")]
    public sealed class BalanceSheetAccount : NamedObject, IBalanceSheetAccount, ICustomGeneralLedgerAccount, IInventoryWriteOffAccount, IReceiptOrPaymentAccount, IJournalEntryAccount, INonInventoryItemAccount, IPurchaseInvoiceAccount, ISalesInvoiceAccount, ICode
    {
        [Guide("Enter a descriptive name for this balance sheet account.")]
        [Guide("Use clear names that indicate the account's purpose, such as 'Prepaid Insurance', 'Accrued Expenses', or 'Loan from ABC Bank'.")]
        [Guide("This name appears in the chart of accounts, on reports, and in transaction entry screens.")]
        [ProtoMember(1), NoWrap] public string Name { get; set; }

        [Guide("Enter an account code to organize and identify this account in your chart of accounts.")]
        [Guide("Account codes are optional but recommended for systematic organization. Use a numbering scheme like 1000-1999 for assets, 2000-2999 for liabilities.")]
        [Guide("The code appears before the account name in lists and helps with sorting and searching.")]
        [ProtoMember(17), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }

        [Guide("Select the balance sheet group where this account should appear on financial reports.")]
        [Guide("Groups organize accounts into categories like Current Assets, Fixed Assets, Current Liabilities, or Long-term Liabilities.")]
        [Guide("Proper grouping ensures your `Balance Sheet` displays accounts in the correct sections with appropriate subtotals.")]
        [ProtoMember(3), NoWrap, Autocomplete(typeof(BalanceSheetAbstractGroup)), Prepend(nameof(Strings.BalanceSheet))] public Guid? Group { get; set; }

        [Guide("Select how this account should be classified on the `Cash Flow Statement`.")]
        [Guide("Operating Activities: Day-to-day business operations like receivables, payables, and prepaid expenses.")]
        [Guide("Investing Activities: Purchase or sale of long-term assets like equipment or investments.")]
        [Guide("Financing Activities: Borrowings, loan repayments, and owner contributions or drawings.")]
        [ProtoMember(18), NoWrap, EmptyLabel, Prepend(nameof(Strings.CashFlowStatement))] public CashFlowStatementCategory CashFlowStatement { get; set; }
        [ProtoMember(21), NoWrap, EmptyLabel, Autocomplete(typeof(CashFlowStatementOperatingActivityGroup)), IfEnum(nameof(CashFlowStatement), (int)CashFlowStatementCategory.OperatingActivities)] public Guid? CashFlowStatementOperatingActivityGroup { get; set; }
        [ProtoMember(22), NoWrap, EmptyLabel, Autocomplete(typeof(CashFlowStatementFinancingActivityGroup)), IfEnum(nameof(CashFlowStatement), (int)CashFlowStatementCategory.FinancingActivities)] public Guid? CashFlowStatementFinancingActivityGroup { get; set; }
        [ProtoMember(23), EmptyLabel, Autocomplete(typeof(CashFlowStatementInvestingActivityGroup)), IfEnum(nameof(CashFlowStatement), (int)CashFlowStatementCategory.InvestingActivities)] public Guid? CashFlowStatementInvestingActivityGroup { get; set; }
        [ProtoMember(16)] public int Position { get; set; }

        [Guide("Enable this option to set a default description that automatically appears when using this account.")]
        [Guide("The default description saves time during transaction entry and ensures consistency across similar transactions.")]
        [Guide("For example, 'Monthly rent payment' for a rent expense account or 'Office supplies' for a supplies account.")]
        [ProtoMember(25), Label(nameof(Strings.Autofill), nameof(Strings.LineDescription))] public bool HasDefaultLineDescription { get; set; }
        [ProtoMember(26), IfTrue(nameof(HasDefaultLineDescription)), NoLabel, Textarea] public string DefaultLineDescription { get; set; }

        [Guide("Enable this option to automatically apply a specific tax code when this account is selected.")]
        [Guide("Useful for accounts that always have the same tax treatment, such as taxable sales or tax-exempt items.")]
        [Guide("The default tax code can be overridden during transaction entry if needed.")]
        [ProtoMember(24), IfContains<TaxCode>, Label(nameof(Strings.Autofill), nameof(Strings.TaxCode))] public bool HasDefaultTaxCode { get; set; }
        [ProtoMember(8), IfTrue(nameof(HasDefaultTaxCode)), Autocomplete(typeof(TaxCode)), NoLabel, Short] public Guid? DefaultTaxCode { get; set; }
        [ProtoMember(19)] public bool Inactive { get; set; }

        [ProtoMember(20)] public Guid? Obsolete_Division { get; set; }
        [ProtoMember(5)] public decimal Obsolete_StartingBalance2 { get; set; }
        [ProtoMember(6)] public DebitCredit Obsolete_StartingBalanceType2 { get; set; }
        [ProtoMember(9)] internal ManagerServer.Model.Obsolete.Obsolete18.GeneralLedgerAccount18 Obsolete_GeneralLedgerAccount;
        [ProtoMember(7)] public Guid? Obsolete_Currency { get; set; }
        [ProtoMember(4)] public bool Obsolete_HasStartingBalance { get; set; }
        [ProtoMember(13)] public decimal Obsolete_StartingBalance { get; set; }
        [ProtoMember(15)] public bool Obsolete_DoNotReverse { get; set; }
        [ProtoMember(2)] public int? Obsolete_Code { get; set; }
        [ProtoMember(10)] public bool Obsolete_ControlAccount { get; set; }
        [ProtoMember(11)] public ControlAccountType? Obsolete_ControlAccountType { get; set; }

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

        public string NameWithCode
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + Name;
                else return Name;
            }
        }        

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            return true;
        }

        public override string GetCodeAndName()
        {
            return NameWithCode;
        }

        public override string GetName()
        {
            return Name;
        }

        public string GetCode()
        {
            return Code;
        }

        /*
        protected override GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            var startingBalance = StartingBalance;
            if (StartingBalanceType == Model.Enums.DebitCredit.Credit) startingBalance *= -1;

            if (startingBalance == 0m) return null;

            var baseCurrency = database.Single<BaseCurrency>();

            var list = new List<GeneralLedgerTransaction>();
            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: DateTime.MinValue,
                generalLedgerAccount: this,
                transactionAmount: startingBalance,
                transactionCurrency: baseCurrency,
                transaction: this,
                trackingCode: database.SingleOrDefault<Division>(Division)
            ));
            list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: DateTime.MinValue,
                generalLedgerAccount: database.Single<BalanceSheetRetainedEarningsAccount>(),
                transactionAmount: startingBalance * -1m,
                transactionCurrency: baseCurrency,
                transaction: this,
                trackingCode: database.SingleOrDefault<Division>(Division)
            ));
            return list.ToArray();
        }

        public override string GetDescriptionOrNull()
        {
            return null;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }
        */
    }
}
