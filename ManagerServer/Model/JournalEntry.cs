using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using System.Collections.Generic;
using System.Linq;
using static ManagerServer.Model.Attributes.ExpressionAttribute.Operators;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("5ea52bc4-90ae-4e4a-aec4-ef1224b279ad")]
    [Currency]
    public sealed class JournalEntry : Transaction, IHasAutomaticReference, IRecurringTransactionDestination, IForeignCurrencyProvider, IComparable<JournalEntry>, ICustomFields, IForeignCurrencyTransaction, ICode, IHasCustomTheme
    {
        [Guide("Enter the date when this journal entry should be recorded in your accounting records.")]
        [Guide("The date determines which accounting period the transaction belongs to and when it will appear in financial reports.")]
        [ProtoMember(1), NoWrap, TableColumn] public DateTime Date { get; set; }
        [Guide("Enter a unique reference number to identify this journal entry.")]
        [Guide("References help you locate specific transactions later and can be used for audit trails or cross-referencing with source documents.")]
        [Guide("You can use automatic numbering by checking the checkbox, or enter your own reference system.")]
        [ProtoMember(2)] public string Reference { get; set; }
        [Guide("Select a `ForeignCurrency` if this journal entry involves transactions in a currency different from your `BaseCurrency`.")]
        [Guide("This field only appears when you have created foreign currencies under `Settings` → `Currencies`.")]
        [Guide("When selected, all amounts in this journal entry will be entered in the chosen foreign currency.")]
        [ProtoMember(8), NoWrap, Autocomplete(typeof(ForeignCurrency))] public Guid? Currency { get; set; }
        [Guide("Enter the `ExchangeRate` to convert amounts between the selected foreign currency and your `BaseCurrency`.")]
        [Guide("The exchange rate determines how foreign currency amounts are converted for reporting in your base currency.")]
        [Guide("You can configure automatic exchange rate retrieval under `Settings` → `ExchangeRates`.")]
        [ProtoMember(21), Placeholder(nameof(Strings.Autofill)), NoWrap, IfNotNull(nameof(Currency)), WebService(typeof(WebServiceForExchangeRates)), Prepend("1 {{ (ExchangeRateIsInverse ? baseCurrency.code : Currency.Code) }} = "), Append("{{ (ExchangeRateIsInverse ? Currency.Code : baseCurrency.code) }}")] public decimal ExchangeRate { get; set; }
        [ProtoMember(22), IfNotNull(nameof(Currency)), Icon("fa-right-left")] public bool ExchangeRateIsInverse { get; set; }
        [Guide("Enter a description that explains the purpose and context of this journal entry.")]
        [Guide("Good descriptions help you understand the transaction when reviewing it later and are essential for audit purposes.")]
        [Guide("Include relevant details such as invoice numbers, contract references, or the business reason for the entry.")]
        [ProtoMember(3), Long, TableColumn] public string Narration { get; set; }
        [Guide("Add debit and credit lines to record how this transaction affects your accounts.")]
        [Guide("Each line represents one account that is either debited or credited.")]
        [Guide("The fundamental accounting rule applies: total debits must equal total credits for the entry to balance.")]
        [Guide("If the entry is out of balance, an error message will appear showing the difference.")]
        [ProtoMember(14), InitialSize(2)] public Line[] Lines { get; set; }
        [IfExpressionNotZero, Prepend(nameof(Strings.OutOfBalance)), NoLabel, Expression(Zero, PlusArray, nameof(Line.Credit), Negate, PlusArray, nameof(Line.Debit), AbsoluteValue), AppendCurrency] public object OutOfBalance { get; set; }
        [Guide("When using `TaxCodes` in this journal entry, specify whether this transaction represents a sale or purchase.")]
        [Guide("This classification determines how the transaction appears in tax reports and which tax accounts are affected.")]
        [Guide("Choose 'Sale' for revenue transactions or 'Purchase' for expense transactions.")]
        [ProtoMember(13), IfAnyNotNull(nameof(Line.TaxCode)), Prepend(nameof(Strings.ForTaxPurposesThisIs)), NoLabel] public TaxTransactionType ForTaxPurposesThisIs { get; set; }
        [Guide("Enable the `Item` column to select `InventoryItems` or `NonInventoryItems` in journal entry lines.")]
        [Guide("When an item is selected, the appropriate income or expense account is automatically populated based on the item's settings.")]
        [Guide("This is useful for recording inventory adjustments, write-offs, or other item-based transactions.")]
        [ProtoMember(23), IfContains<InventoryItem, NonInventoryItem, InventoryKit>, Label(nameof(Strings.Column), nameof(Strings.Item))] public bool ItemColumn { get; set; }
        [Guide("Enable the `Description` column to add detailed explanations for individual journal entry lines.")]
        [Guide("Line descriptions provide additional context for each debit and credit, making the entry easier to understand.")]
        [Guide("This is particularly useful for complex entries with multiple lines affecting different accounts.")]
        [ProtoMember(15), Label(nameof(Strings.Column), nameof(Strings.Description))] public bool HasLineDescription { get; set; }
        [Guide("Enable the `Qty` column to record quantities for inventory items or measurable services.")]
        [Guide("Quantities help track inventory movements and are essential for maintaining accurate stock levels.")]
        [Guide("When used with inventory items, the quantity affects your inventory on hand and cost of goods calculations.")]
        [ProtoMember(20), Label(nameof(Strings.Column), nameof(Strings.Qty))] public bool QuantityColumn { get; set; }
        [ProtoMember(9), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(10), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
        [ProtoMember(16), Label(nameof(Strings.Footers))] public bool HasJournalEntryFooters { get; set; }
        [ProtoMember(17), Autocomplete(typeof(ManagerServer.Model.JournalEntryFooter)), NoLabel, IfTrue(nameof(HasJournalEntryFooters))] public Guid[] JournalEntryFooters { get; set; }
        [Guide("Mark this journal entry as a cash transaction if it involves actual cash movement.")]
        [Guide("Cash transactions are distinguished from accrual entries and affect how they appear in the `CashFlowStatement`.")]
        [Guide("Examples include cash sales, cash purchases, or any transaction involving immediate payment.")]
        [ProtoMember(19)] public bool CashTransactionForCashFlowStatementPurposes { get; set; }
        [Guide("Add custom information specific to your business needs using `CustomFields`.")]
        [Guide("Custom fields can track project codes, approval references, department codes, or any other data you need.")]
        [Guide("Set up custom fields under `Settings` → `CustomFields` before using them in transactions.")]
        [ProtoMember(7)] public Dictionary<Guid, string> CustomFields { get; set; }
        [Guide("Use enhanced `CustomFields` for more advanced data types including dates, numbers, and dropdown selections.")]
        [Guide("These fields offer validation and formatting options not available in classic custom fields.")]
        [Guide("Configure enhanced custom fields under `Settings` → `CustomFields` with specific data types and validation rules.")]
        [ProtoMember(18)] public CustomFields CustomFields2 { get; set; }
        [ProtoMember(11), DoNotCopy] public bool AutomaticReference { get; set; }

        public override string GetReference() => Reference;

        DateTime IForeignCurrencyTransaction.Date => Date;
        Guid? IForeignCurrencyTransaction.Currency => Currency;
        decimal IForeignCurrencyTransaction.ExchangeRate { get => ExchangeRate; set => ExchangeRate = value; }
        bool IForeignCurrencyTransaction.ExchangeRateIsInverse { get => ExchangeRateIsInverse; set => ExchangeRateIsInverse = value; }
        string ICode.Code => Reference;

        public override bool GetHasLineDescription() => HasLineDescription;
        public override bool HasLineQty() => QuantityColumn;

        [CustomFields]
        [ProtoContract]
        [Guid("5e1468b4-8c7e-431a-837d-6e90384a3f4a")]
        public sealed class Line : ITransactionLine
        {
            [ProtoMember(30), IfTrue(nameof(ItemColumn)), Autocomplete(typeof(IPurchaseItem)), OnChangeSetDefault(nameof(Qty)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(TaxCode)), OnChangeSetDefault(nameof(Division)), Short] public Guid? Item { get; set; }
            [ProtoMember(23), NoLabel, IfNotNull(nameof(Item)), Autocomplete(typeof(CustomInventoryLocation)), Prepend(nameof(Strings.InventoryLocation))] public Guid? InventoryLocation { get; set; }
            [ProtoMember(1), Autocomplete(typeof(IJournalEntryAccount), Subtext = nameof(BalanceSheetAccount.Group)), Substitute(nameof(Item), nameof(IPurchaseItem.PurchaseItemAccount)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(TaxCode))] public Guid? Account { get; set; }
            [ProtoMember(29), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsCashAtBank)), Autocomplete(typeof(BankOrCashAccount), Filter = nameof(Account)), Prepend(nameof(Strings.BankOrCashAccount))] public Guid? BankOrCashAccount { get; set; }
            [ProtoMember(2), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsReceivable)), Autocomplete(typeof(Customer), Filter = nameof(Account)), OnChangeSetNull(nameof(AccountsReceivableSalesInvoice)), Prepend(nameof(Strings.Customer))] public Guid? AccountsReceivableCustomer { get; set; }
            [ProtoMember(5), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsReceivable)), IfNotNull(nameof(AccountsReceivableCustomer)), Autocomplete(typeof(SalesInvoice), Filter = nameof(AccountsReceivableCustomer)), Placeholder(nameof(Strings.Automatic)), Prepend(nameof(Strings.Invoice)), Short] public Guid? AccountsReceivableSalesInvoice { get; set; }
            [ProtoMember(21), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsBillableExpense)), Autocomplete(typeof(Customer)), Prepend(nameof(Strings.Customer))] public Guid? BillableExpenseCustomer { get; set; }
            [ProtoMember(22), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsBillableExpense)), IfNotNull(nameof(BillableExpenseCustomer)), Autocomplete(typeof(SalesInvoice), Filter = nameof(BillableExpenseCustomer)), Prepend(nameof(Strings.Invoice)), Placeholder(nameof(Strings.Uninvoiced)), Short] public Guid? BillableExpenseSalesInvoice { get; set; }
            [ProtoMember(3), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsPayable)), Autocomplete(typeof(Supplier), Filter = nameof(Account)), OnChangeSetNull(nameof(PurchaseInvoice)), Prepend(nameof(Strings.Supplier))] public Guid? AccountsPayableSupplier { get; set; }
            [ProtoMember(6), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsPayable)), IfNotNull(nameof(AccountsPayableSupplier)), Autocomplete(typeof(PurchaseInvoice), Filter = nameof(AccountsPayableSupplier)), Prepend(nameof(Strings.Invoice)), Placeholder(nameof(Strings.Automatic)), Short] public Guid? PurchaseInvoice { get; set; }
            [ProtoMember(24), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsWithholdingTaxPayablePayable)), Autocomplete(typeof(Supplier)), Prepend(nameof(Strings.Supplier))] public Guid? WithholdingTaxPayableSupplier { get; set; }
            [ProtoMember(11), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), Autocomplete(typeof(CapitalAccount), Filter = nameof(Account)), Prepend(nameof(Strings.CapitalAccount))] public Guid? CapitalAccount { get; set; }
            [ProtoMember(17), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), IfNotNull(nameof(CapitalAccount)), Autocomplete(typeof(SubAccount)), Prepend(nameof(Strings.SubAccount))] public Guid? SubAccount { get; set; }
            [ProtoMember(4), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsEmployeeClearingAccount)), Autocomplete(typeof(Employee), Filter = nameof(Account)), Prepend(nameof(Strings.Employee))] public Guid? Employee { get; set; }
            [ProtoMember(8), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForSpecialAccounts)), Autocomplete(typeof(SpecialAccount), Filter = nameof(Account)), Prepend(nameof(Strings.SpecialAccount))] public Guid? SpecialAccount { get; set; }
            [ProtoMember(9), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.HasFixedAssets)), Autocomplete(typeof(FixedAsset), Filter = nameof(Account)), Prepend(nameof(Strings.FixedAsset))] public Guid? FixedAsset { get; set; }
            [ProtoMember(10), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.HasIntangibleAssets)), Autocomplete(typeof(IntangibleAsset), Filter = nameof(Account)), Prepend(nameof(Strings.IntangibleAsset))] public Guid? IntangibleAsset { get; set; }
            [ProtoMember(18), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.HasExpenseClaimPayers)), Autocomplete(typeof(ExpenseClaimsPayer)), Prepend(nameof(Strings.ExpenseClaimsPayer))] public Guid? ExpenseClaimPayer { get; set; }
            [ProtoMember(26), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForInvestments)), Autocomplete(typeof(Investment), Filter = nameof(Account)), Prepend(nameof(Strings.Investment))] public Guid? Investment { get; set; }
            [ProtoMember(12), IfTrue(nameof(HasLineDescription)), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [ProtoMember(27)] public Dictionary<Guid, string> CustomFields { get; set; }
            [ProtoMember(28)] public CustomFields CustomFields2 { get; set; }
            [ProtoMember(16), IfTrue(nameof(QuantityColumn)), Short, AppendValue(nameof(InventoryItem), nameof(ManagerServer.Model.InventoryItem.UnitName))] public decimal Qty { get; set; }
            [ProtoMember(13), NoPlaceholder, Sum] public decimal Debit { get; set; }
            [ProtoMember(14), NoPlaceholder, Sum, AppendCurrency] public decimal Credit { get; set; }
            [ProtoMember(20), IfDifferentCurrency, IfLineAccountForeignCurrencyNotNull, NoPlaceholder] public decimal CurrencyAmount { get; set; }
            [ProtoMember(15), Autocomplete(typeof(TaxCode)), IfTrue(nameof(Account), nameof(NamedObject.TaxCodeEnabled)), Short] public Guid? TaxCode { get; set; }
            [ProtoMember(25), Autocomplete(typeof(Project)), IfTrue(nameof(Account), nameof(NamedObject.ProjectEnabled)), Short] public Guid? Project { get; set; }
            [ProtoMember(19), Autocomplete(typeof(Division)), IfTrue(nameof(Account), nameof(NamedObject.DivisionEnabled)), Short] public Guid? Division { get; set; }

            public override Guid? GetItem() => Item;
            public override Guid? GetAccount() => Account;
            public override Guid? GetBankOrCashAccount() => BankOrCashAccount;
            public override Guid? GetBillableExpenseCustomer() => BillableExpenseCustomer;
            public override Guid? GetBillableExpenseSalesInvoice() => BillableExpenseSalesInvoice;
            public override Guid? GetWithholdingTaxPayableSupplier() => WithholdingTaxPayableSupplier;
            public override Guid? GetEmployee() => Employee;
            public override Guid? GetFixedAsset() => FixedAsset;
            public override Guid? GetIntangibleAsset() => IntangibleAsset;
            public override Guid? GetCapitalAccount() => CapitalAccount;
            public override Guid? GetSpecialAccount() => SpecialAccount;
            public override Guid? GetSubAccount() => SubAccount;
            public override Guid? GetInvestment() => Investment;
            public override Guid? GetExpenseClaimPayer() => ExpenseClaimPayer;
            public override Guid? GetAccountsReceivableCustomer() => AccountsReceivableCustomer;
            public override Guid? GetAccountsReceivableSalesInvoice() => AccountsReceivableSalesInvoice;
            public override Guid? GetAccountsPayablePurchaseInvoice() => PurchaseInvoice;
            public override Guid? GetAccountsPayableSupplier() => AccountsPayableSupplier;
            protected override decimal? GetQty() => Qty;
            public override decimal? GetDebit() => Debit;
            public override decimal? GetCredit() => Credit;
            protected override string GetLineDescription() => LineDescription;
            public override Guid? GetTaxCode() => TaxCode;
            public override Guid? GetDivision() => Division;
            public override Dictionary<Guid, string> GetCustomFields() => CustomFields;
            public override CustomFields GetCustomFields2() => CustomFields2;
            public override decimal? GetProposedAccountAmount() => CurrencyAmount;
            protected override Guid? GetProject() => Project;

            [ProtoMember(7)] public Guid? Obsolete_InventoryItem { get; set; }
        }
        
        [ProtoMember(12)] public Guid? Obsolete_InventoryLocation { get; set; }
        [ProtoMember(4)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines { get; set; }
        [ProtoMember(6)] public bool Obsolete_IsReversing { get; set; }
        [ProtoMember(5)] public string Obsolete_Notes { get; set; }
        [ProtoMember(24)] public InventoryWriteOff Obsolete_InventoryWriteOff { get; set; }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        DateTime IRecurringTransactionDestination.Date { get => Date; set => Date = value; }

        Guid? IForeignCurrencyProvider.ForeignCurrency => Currency;

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;

        public override string GetDescriptionOrNull()
        {
            if (!string.IsNullOrWhiteSpace(Narration)) return Narration;
            return null;
        }

        public override string GetName()
        {
            return Reference;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }

        public override ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            if (Lines == null) return [];

            Currency currency = database.SingleOrDefault<ForeignCurrency>(Currency);
            if (currency == null) currency = database.Single<BaseCurrency>();

            var list = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();
            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i] == null) continue;

                list.AddRange(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction.From(
                    database: database,
                    transaction: this,
                    date: Date,
                    transactionLine: Lines[i],
                    transactionCurrency: currency,
                    inventoryLocation: database.SingleOrDefault<CustomInventoryLocation>(Lines[i].InventoryLocation),
                    amountsIncludeTax: true,
                    lineNumber: i,
                    exchangeRate: ExchangeRate,
                    exchangeRateIsInverse: ExchangeRateIsInverse
                ));
            }

            var transactionBalance = list.Sum(x => x.TransactionAmount);
            var baseBalance = list.Sum(x => x.BaseAmount);
            if (transactionBalance != 0m)
            {
                list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    transaction: this,
                    date: Date,
                    transactionAmount: transactionBalance * -1m,
                    baseAmount: baseBalance * -1m,
                    accountAmount: baseBalance * -1m,
                    transactionCurrency: currency,
                    generalLedgerAccount: database.Single<BalanceSheetSuspenseAccount>()
                ));
            }
            else if (baseBalance != 0m)
            {
                var divisions = list.Select(x => x.Division).Distinct().ToArray();

                list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        transaction: this,
                        date: Date,
                        transactionAmount: 0m,
                        baseAmount: baseBalance * -1m,
                        accountAmount: baseBalance * -1m,
                        transactionCurrency: currency,
                        trackingCode: divisions.Length == 1 ? divisions[0] : null,
                        generalLedgerAccount: database.Single<BalanceSheetRetainedEarningsAccount>()
                    ));
            }

            return list.ToArray();
        }

        int IComparable<JournalEntry>.CompareTo(JournalEntry other)
        {
            return (!other.IsInactive(), other.Date, other.Reference).CompareTo((!IsInactive(), Date, Reference));
        }
    }
}
