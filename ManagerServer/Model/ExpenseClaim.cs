using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using ManagerServer.Globalization;
using static ManagerServer.Model.Attributes.ExpressionAttribute.Operators;
using ManagerServer.Model.Enums;
using System.Net;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("02572e0c-0167-4dbd-a392-08d8f67f3fe4")]
    [Currency]
    public sealed class ExpenseClaim : Transaction, IHasAutomaticReference, IComparable<ExpenseClaim>, ICustomFields, IForeignCurrencyTransaction, ICode, IHasCustomTheme
    {
        [Header("Basic Information")]
        [Guide("Enter the date of the expense claim. This is typically when the expenses were incurred or when the claim is submitted.")]
        [Guide("This date determines when the expenses are recorded in your accounts and when the liability to reimburse is recognized.")]
        [ProtoMember(1), NoWrap] public DateTime Date { get; set; }
        [Guide("Enter a reference number for this expense claim. This helps track and process reimbursement requests.")]
        [Guide("You can use automatic numbering or create your own system, such as employee initials plus date.")]
        [ProtoMember(5)] public string Reference { get; set; }
        [Header("Claimant Details")]
        [Guide("Select the employee or capital account holder who paid for these expenses and should be reimbursed.")]
        [Guide("This creates a liability in your accounts showing you owe this person reimbursement for their out-of-pocket expenses.")]
        [ProtoMember(9), Autocomplete(typeof(IExpenseClaimPayer))] public Guid? PaidBy { get; set; }
        [Guide("Enter the name of the vendor or payee to whom the expenses were paid. This helps identify where the money was spent.")]
        [Guide("For example, enter the restaurant name for meal expenses or the taxi company for transportation costs.")]
        [ProtoMember(7), Typeahead] public string Payee { get; set; }
        [Header("Currency and Exchange Rate")]
        [Guide("Select the currency in which the expenses were paid if different from your base currency.")]
        [Guide("This is useful for travel expenses paid in foreign currencies that need to be reimbursed in your base currency.")]
        [ProtoMember(10), NoWrap, Autocomplete(typeof(ForeignCurrency))] public Guid? Currency { get; set; }
        [Guide("Enter the exchange rate for converting the foreign currency expenses to your base currency for reimbursement.")]
        [Guide("Use the exchange rate from when the expenses were incurred, not the current rate, for accurate cost recording.")]
        [ProtoMember(23), Placeholder(nameof(Strings.Autofill)), NoWrap, IfNotNull(nameof(Currency)), WebService(typeof(WebServiceForExchangeRates)), Prepend("1 {{ (ExchangeRateIsInverse ? baseCurrency.code : Currency.Code) }} = "), Append("{{ (ExchangeRateIsInverse ? Currency.Code : baseCurrency.code) }}")] public decimal ExchangeRate { get; set; }
        [ProtoMember(24), IfNotNull(nameof(Currency)), Icon("fa-right-left")] public bool ExchangeRateIsInverse { get; set; }
        [Guide("Optionally, add a description or notes about this expense claim, such as the purpose of the expenses or trip details.")]
        [Guide("Good descriptions help with approval processes and provide context for auditing and tax purposes.")]
        [ProtoMember(3), Long] public string Description { get; set; }
        [Header("Expense Line Items")]
        [Guide("Enter the individual expense items. Each line represents a different expense with its amount and account allocation.")]
        [Guide("Break down expenses by type (meals, transport, accommodation) and allocate to appropriate expense accounts.")]
        [ProtoMember(18)] public Line[] Lines { get; set; }
        [Guide("If any expenses are for inventory items, select the location where the inventory should be added.")]
        [Guide("This is used when employees purchase inventory items directly, such as emergency supplies or materials.")]
        [ProtoMember(12), IfAnyNotNull(nameof(Line.Item)), Autocomplete(typeof(CustomInventoryLocation)), NoLabel, Prepend(nameof(Strings.InventoryLocation))] public Guid? InventoryLocation { get; set; }
        [Guide("Check this box if the expense amounts already include tax. Leave unchecked if tax needs to be added.")]
        [Guide("Most receipts show tax-inclusive amounts, so this box is usually checked for expense claims.")]
        [ProtoMember(16), IfContains<TaxCode>] public bool AmountsIncludeTax { get; set; }
        [Guide("Check this box to show a description column for each expense line, allowing detailed explanations of each expense.")]
        [Guide("Descriptions help explain the business purpose of each expense, which is important for tax deductibility.")]
        [ProtoMember(17), Label(nameof(Strings.Column), nameof(Strings.Description))] public bool HasLineDescription { get; set; }
        [ProtoMember(13), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(14), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
        [ProtoMember(15), DoNotCopy] public bool AutomaticReference { get; set; }
        [ProtoMember(19), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }
        [ProtoMember(21), Label(nameof(Strings.Footers))] public bool HasExpenseClaimFooters { get; set; }
        [ProtoMember(22), Autocomplete(typeof(ManagerServer.Model.ExpenseClaimFooter)), NoLabel, IfTrue(nameof(HasExpenseClaimFooters))] public Guid[] ExpenseClaimFooters { get; set; }
        [ProtoMember(8)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(20)] public CustomFields CustomFields2 { get; set; }

        public override string GetReference() => Reference;

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        DateTime IForeignCurrencyTransaction.Date => Date;
        Guid? IForeignCurrencyTransaction.Currency => Currency;
        decimal IForeignCurrencyTransaction.ExchangeRate { get => ExchangeRate; set => ExchangeRate = value; }
        bool IForeignCurrencyTransaction.ExchangeRateIsInverse { get => ExchangeRateIsInverse; set => ExchangeRateIsInverse = value; }
        string ICode.Code => Reference;

        public override bool GetHasLineDescription() => HasLineDescription;
        public override bool HasLineQty() => true;
        public override bool HasLineUnitPrice() => true;

        [CustomFields]
        [ProtoContract]
        [Guid("2306A5CC-4140-4FF1-92C7-5CEBA32C170D")]
        public sealed class Line : ITransactionLine
        {
            [ProtoMember(1), Autocomplete(typeof(IPurchaseItem)), OnChangeSetDefault(nameof(Qty)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(PurchaseUnitPrice)), OnChangeSetDefault(nameof(TaxCode)), OnChangeSetDefault(nameof(Division)), Short] public Guid? Item { get; set; }
            [ProtoMember(2), Autocomplete(typeof(IReceiptOrPaymentAccount), Subtext = nameof(BalanceSheetAccount.Group)), Substitute(nameof(Item), nameof(IPurchaseItem.PurchaseItemAccount)), OnChangeSetDefault(nameof(TaxCode))] public Guid? Account { get; set; }
            [ProtoMember(3), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsReceivable)), Autocomplete(typeof(Customer), Filter = nameof(Account)), Prepend(nameof(Strings.Customer))] public Guid? AccountsReceivableCustomer { get; set; }
            [ProtoMember(4), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsReceivable)), IfNotNull(nameof(AccountsReceivableCustomer)), Autocomplete(typeof(SalesInvoice), Filter = nameof(AccountsReceivableCustomer)), Placeholder(nameof(Strings.Automatic)), Prepend(nameof(Strings.Invoice)), Short] public Guid? AccountsReceivableSalesInvoice { get; set; }
            [ProtoMember(5), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsBillableExpense)), Autocomplete(typeof(Customer)), Prepend(nameof(Strings.Customer))] public Guid? BillableExpenseCustomer { get; set; }
            [ProtoMember(6), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsBillableExpense)), IfNotNull(nameof(BillableExpenseCustomer)), Autocomplete(typeof(SalesInvoice), Filter = nameof(BillableExpenseCustomer)), Prepend(nameof(Strings.Invoice)), Placeholder(nameof(Strings.Uninvoiced)), Short] public Guid? BillableExpenseSalesInvoice { get; set; }
            [ProtoMember(7), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsPayable)), Autocomplete(typeof(Supplier), Filter = nameof(Account)), Prepend(nameof(Strings.Supplier))] public Guid? AccountsPayableSupplier { get; set; }
            [ProtoMember(8), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsPayable)), IfNotNull(nameof(AccountsPayableSupplier)), Autocomplete(typeof(PurchaseInvoice), Filter = nameof(AccountsPayableSupplier)), Prepend(nameof(Strings.Invoice)), Placeholder(nameof(Strings.Automatic)), Short] public Guid? PurchaseInvoice { get; set; }
            [ProtoMember(23), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsWithholdingTaxPayablePayable)), Autocomplete(typeof(Supplier)), Prepend(nameof(Strings.Supplier))] public Guid? WithholdingTaxPayableSupplier { get; set; }
            [ProtoMember(9), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), Autocomplete(typeof(CapitalAccount), Filter = nameof(Account)), Prepend(nameof(Strings.CapitalAccount))] public Guid? CapitalAccount { get; set; }
            [ProtoMember(10), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), IfNotNull(nameof(CapitalAccount)), Autocomplete(typeof(SubAccount)), Prepend(nameof(Strings.SubAccount))] public Guid? SubAccount { get; set; }
            [ProtoMember(11), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsEmployeeClearingAccount)), Autocomplete(typeof(Employee), Filter = nameof(Account)), Prepend(nameof(Strings.Employee))] public Guid? Employee { get; set; }
            [ProtoMember(13), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForSpecialAccounts)), Autocomplete(typeof(SpecialAccount), Filter = nameof(Account)), Prepend(nameof(Strings.SpecialAccount))] public Guid? SpecialAccount { get; set; }
            [ProtoMember(14), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForFixedAssets)), Autocomplete(typeof(FixedAsset), Filter = nameof(Account)), Prepend(nameof(Strings.FixedAsset))] public Guid? FixedAsset { get; set; }
            [ProtoMember(15), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForIntangibleAssets)), Autocomplete(typeof(IntangibleAsset), Filter = nameof(Account)), Prepend(nameof(Strings.IntangibleAsset))] public Guid? IntangibleAsset { get; set; }
            [ProtoMember(17), IfTrue(nameof(HasLineDescription)), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [ProtoMember(25)] public Dictionary<Guid, string> CustomFields { get; set; }
            [ProtoMember(26)] public CustomFields CustomFields2 { get; set; }
            [ProtoMember(18), AppendValue(nameof(Item), nameof(ManagerServer.Model.InventoryItem.UnitName)), Short] public decimal? Qty { get; set; }
            [ProtoMember(19), NoPlaceholder, AppendCurrency, Label(nameof(Strings.UnitPrice))] public decimal PurchaseUnitPrice { get; set; }
            [ProtoMember(20), IfDifferentCurrency, NoPlaceholder] public decimal CurrencyAmount { get; set; }
            [Label(nameof(Strings.Total)), Expression(Zero, Plus, nameof(PurchaseUnitPrice), Times, nameof(Qty), Round), Sum, AppendCurrency] public object TotalBeforeTax { get; }
            [ProtoMember(21), Autocomplete(typeof(TaxCode)), Short] public Guid? TaxCode { get; set; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), TimesTaxCode, nameof(TaxCode)), Sum, IfFalse(nameof(SalesOrder.AmountsIncludeTax))] public object TaxAmount { get; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), Plus, nameof(TaxAmount), Round), Sum, AppendCurrency, IfFalse(nameof(SalesOrder.AmountsIncludeTax))] public object Total { get; }
            [ProtoMember(24), Autocomplete(typeof(Project)), IfTrue(nameof(Account), nameof(NamedObject.ProjectEnabled)), Short] public Guid? Project { get; set; }
            [ProtoMember(22), Autocomplete(typeof(Division)), IfTrue(nameof(Account), nameof(NamedObject.DivisionEnabled)), Short] public Guid? Division { get; set; }

            public override Guid? GetItem() => Item;
            public override Guid? GetAccount() => Account;
            public override Guid? GetBillableExpenseCustomer() => BillableExpenseCustomer;
            public override Guid? GetBillableExpenseSalesInvoice() => BillableExpenseSalesInvoice;
            public override Guid? GetWithholdingTaxPayableSupplier() => WithholdingTaxPayableSupplier;
            public override Guid? GetEmployee() => Employee;
            public override Guid? GetFixedAsset() => FixedAsset;
            public override Guid? GetIntangibleAsset() => IntangibleAsset;
            public override Guid? GetCapitalAccount() => CapitalAccount;
            public override Guid? GetSpecialAccount() => SpecialAccount;
            public override Guid? GetAccountsReceivableCustomer() => AccountsReceivableCustomer;
            public override Guid? GetAccountsPayableSupplier() => AccountsPayableSupplier;
            public override Guid? GetAccountsPayablePurchaseInvoice() => PurchaseInvoice;
            public override Guid? GetAccountsReceivableSalesInvoice() => AccountsReceivableSalesInvoice;
            public override Guid? GetSubAccount() => SubAccount;
            protected override decimal? GetUnitPrice() => PurchaseUnitPrice;
            protected override decimal? GetQty() => Qty;
            protected override string GetLineDescription() => LineDescription;
            public override Guid? GetTaxCode() => TaxCode;
            public override Guid? GetDivision() => Division;
            public override Dictionary<Guid, string> GetCustomFields() => CustomFields;
            public override CustomFields GetCustomFields2() => CustomFields2;
            public override decimal? GetProposedAccountAmount() => CurrencyAmount;
            protected override Guid? GetProject() => Project;
        }

        [ProtoMember(2)] public Guid? Obsolete_CreditAccount { get; set; }
        [ProtoMember(6)] public string Obsolete_Payor { get; set; }
        [ProtoMember(11)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines { get; set; }
        [ProtoMember(4)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines2 { get; set; }

        public override string GetDescriptionOrNull()
        {
            if (!string.IsNullOrWhiteSpace(Description)) return Description;
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
            IGeneralLedgerAccount generalLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
            var employee = database.SingleOrDefault<Employee>(PaidBy);
            var capitalAccount = database.SingleOrDefault<CapitalAccount>(PaidBy);
            var expenseClaimPayer = database.SingleOrDefault<ExpenseClaimsPayer>(PaidBy);

            var inventoryLocation = database.SingleOrDefault<CustomInventoryLocation>(InventoryLocation);

            if (employee != null) generalLedgerAccount = database.Single<BalanceSheetEmployeeClearingAccount>();
            if (capitalAccount != null) generalLedgerAccount = database.Single<BalanceSheetCapitalAccountsAccount>();
            if (expenseClaimPayer != null) generalLedgerAccount = database.Single<BalanceSheetExpenseClaimsAccount>();

            Currency currency = database.SingleOrDefault<ForeignCurrency>(Currency);
            if (currency == null) currency = database.Single<BaseCurrency>();

            var list = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();
            if (Lines != null)
            {
                for (int i = 0; i < Lines.Length; i++)
                {
                    list.AddRange(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction.From(
                        database: database,
                        date: Date,
                        transaction: this,
                        transactionCurrency: currency,
                        exchangeRate: ExchangeRate,
                        exchangeRateIsInverse: ExchangeRateIsInverse,
                        transactionLine: Lines[i],
                        expenseClaimPayer: expenseClaimPayer,
                        capitalAccount: capitalAccount,
                        employee: employee,
                        amountsIncludeTax: AmountsIncludeTax,
                        inventoryLocation: inventoryLocation,
                        lineNumber: i
                    ));
                }
            }

            Division trackingCode = null;
            if (employee != null) trackingCode = database.SingleOrDefault<Division>(employee.Division);
            if (capitalAccount != null) trackingCode = database.SingleOrDefault<Division>(capitalAccount.Division);
            if (expenseClaimPayer != null) trackingCode = database.SingleOrDefault<Division>(expenseClaimPayer.Division);

            list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: Date,
                generalLedgerAccount: generalLedgerAccount,
                transactionAmount: list.Sum(x => x.TransactionAmount) * -1m,
                baseAmount: list.Sum(x => x.BaseAmount) * -1m,
                transactionCurrency: currency,
                transaction: this,
                expenseClaimPayer: expenseClaimPayer,
                employee: employee,
                capitalAccount: capitalAccount,
                capitalSubaccount: SubAccount.ExpenseClaims,
                isBalancing: true,
                contraTransactions: list.ToArray(),
                trackingCode: trackingCode
            ));

            return list.ToArray();
        }

        int IComparable<ExpenseClaim>.CompareTo(ExpenseClaim other)
        {
            return (other.Date, other.Reference).CompareTo((Date, Reference));
        }
    }
}