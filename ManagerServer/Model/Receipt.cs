using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using static ManagerServer.Model.Attributes.ExpressionAttribute.Operators;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("7662b887-c8d8-486e-98fd-f9dbcd41c6dc")]
    [Currency(nameof(ReceivedIn))]
    public sealed class Receipt : Transaction, IHasAutomaticReference, IRecurringTransactionDestination, IComparable<Receipt>, ICustomFields, IForeignCurrencyTransaction, ICode, IHasCustomTheme
    {
        [Guide("Enter the date when you received this payment.")]
        [Guide("This date determines when the receipt is recorded in your books and which accounting period it belongs to.")]
        [Guide("For bank deposits, use the date you deposited the funds, not necessarily when the bank clears them.")]
        [ProtoMember(1), NoWrap] public DateTime Date { get; set; }
        [Guide("Enter a reference number to uniquely identify this receipt.")]
        [Guide("Common references include receipt numbers, check numbers, bank deposit slips, or payment confirmation codes.")]
        [Guide("References help you match receipts to bank statements and locate specific transactions later.")]
        [ProtoMember(2)] public string Reference { get; set; }
        [Guide("Select the type of payer making this payment.")]
        [Guide("Choose `Customer` for customer payments against sales invoices or prepayments.")]
        [Guide("Choose `Supplier` for refunds or credit notes from suppliers.")]
        [Guide("Choose `Other` for payments from non-customer/supplier sources like loans, grants, or miscellaneous income.")]
        [ProtoMember(3), NoWrap, Prepend(nameof(Strings.Contact))] public PayerPayeeType PaidBy { get; set; }
        [ProtoMember(4), EmptyLabel, NoWrap, IfEnum(nameof(PaidBy), (int)PayerPayeeType.Customer), Autocomplete(typeof(ManagerServer.Model.Customer))] public Guid? Customer { get; set; }
        [ProtoMember(5), EmptyLabel, NoWrap, IfEnum(nameof(PaidBy), (int)PayerPayeeType.Supplier), Autocomplete(typeof(ManagerServer.Model.Supplier))] public Guid? Supplier { get; set; }
        [ProtoMember(6), EmptyLabel, IfEnum(nameof(PaidBy), (int)PayerPayeeType.Other), Placeholder(nameof(Strings.Optional)), Typeahead] public string Contact { get; set; }
        [Guide("Select the `BankAccount` or `CashAccount` that received this payment.")]
        [Guide("The selected account's balance will increase by the receipt amount.")]
        [Guide("If you haven't created the receiving account yet, set it up under `BankAndCashAccounts` first.")]
        [ProtoMember(7), NoWrap, Autocomplete(typeof(ManagerServer.Model.IBankOrCashAccount)), Prepend(nameof(Strings.Account))] public Guid? ReceivedIn { get; set; }
        [Guide("Select the clearing status for bank deposits.")]
        [Guide("Choose `Cleared` if the bank has already processed and credited your account.")]
        [Guide("Choose `Pending` if the deposit is made but not yet showing in your bank statement.")]
        [Guide("This status is crucial for accurate bank reconciliation and cash flow reporting.")]
        [ProtoMember(36), NoWrap, IfTrue(nameof(ReceivedIn), nameof(BankOrCashAccount.CanHavePendingTransactions))] public BankAccountClearStatus Cleared { get; set; }
        [ProtoMember(9), EmptyLabel, IfTrue(nameof(ReceivedIn), nameof(BankOrCashAccount.CanHavePendingTransactions)), IfEnum(nameof(Cleared), (int)BankAccountClearStatus.OnALaterDate), Placeholder(nameof(Strings.Pending)), Prepend(nameof(Strings.Date)), DoNotCopy] public DateTime? BankClearDate { get; set; }
        [Guide("Enter the `ExchangeRate` for converting foreign currency receipts to your `BaseCurrency`.")]
        [Guide("This rate applies when receiving payment into a foreign currency bank account.")]
        [Guide("The exchange rate determines the base currency value for financial reporting.")]
        [Guide("You can configure automatic exchange rates under `Settings` → `ExchangeRates`.")]
        [ProtoMember(45), Placeholder(nameof(Strings.Autofill)), NoWrap, IfNotNull(nameof(ReceivedIn), nameof(BankOrCashAccount.Currency)), Prepend("1 {{ (ExchangeRateIsInverse ? baseCurrency.code : getCurrencyCode()) }} = "), Append("{{ (ExchangeRateIsInverse ? getCurrencyCode() : baseCurrency.code) }}")] public decimal ExchangeRate { get; set; }
        [ProtoMember(46), IfNotNull(nameof(ReceivedIn), nameof(BankOrCashAccount.Currency)), Icon("fa-right-left")] public bool ExchangeRateIsInverse { get; set; }
        [Guide("Enter an optional description to provide context about this receipt.")]
        [Guide("Descriptions help identify the payment source and purpose when reviewing transactions.")]
        [Guide("Include details like payment method, customer PO numbers, or the reason for payment.")]
        [ProtoMember(10), Long, Placeholder(nameof(Strings.Optional))] public string Description { get; set; }
        [Guide("Add line items to allocate this receipt to appropriate accounts.")]
        [Guide("Each line can post to different `IncomeAccounts`, apply to `SalesInvoices`, or record `SalesItems`.")]
        [Guide("Use multiple lines to split a single payment across different income categories or invoices.")]
        [Guide("Lines without accounts will be automatically allocated to the oldest unpaid invoices for the selected customer.")]
        [ProtoMember(11)] public Line[] Lines { get; set; }
        [Guide("Select the `InventoryLocation` when receiving payment for inventory items.")]
        [Guide("This determines which warehouse or location's inventory quantities will be reduced.")]
        [Guide("Only appears when you have enabled inventory locations and are receiving payment for inventory items.")]
        [ProtoMember(30), NoLabel, Prepend(nameof(Strings.InventoryLocation)), Autocomplete(typeof(CustomInventoryLocation)), IfAnyNotNull(nameof(Line.Item))] public Guid? InventoryLocation { get; set; }
        [ProtoMember(41), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [ProtoMember(13), Label(nameof(Strings.Column), nameof(Strings.Description))] public bool HasLineDescription { get; set; }
        [ProtoMember(32), Label(nameof(Strings.Column), nameof(Strings.Qty))] public bool QuantityColumn { get; set; }
        [ProtoMember(33), Label(nameof(Strings.Column), nameof(Strings.UnitPrice)), IfTrue(nameof(QuantityColumn))] public bool UnitPriceColumn { get; set; }
        [ProtoMember(38), Label(nameof(Strings.Column), nameof(Strings.Discount))] public bool Discount { get; set; }
        [ProtoMember(39), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        [Guide("Specify whether line amounts include or exclude tax.")]
        [Guide("Check this box if amounts are tax-exclusive - tax will be calculated and added to line amounts.")]
        [Guide("Leave unchecked if amounts already include tax - tax will be calculated but included in the line amount.")]
        [Guide("This setting affects the final receipt total and how tax amounts are displayed.")]
        [ProtoMember(31), IfContains<TaxCode>] public bool AmountsAreTaxExclusive { get; set; }
        [Guide("Enable fixed total to force this receipt to match a specific amount.")]
        [Guide("Useful when you need to match exact bank deposit amounts or handle rounding differences.")]
        [Guide("Any difference between line items and the fixed total will be automatically posted to `SuspenseAccount`.")]
        [Guide("The suspense account entry helps you investigate and correct discrepancies later.")]
        [ProtoMember(34)] public bool FixedTotal { get; set; }
        [Guide("Enter the exact total amount for this receipt.")]
        [Guide("Line items will be recorded as entered, but the receipt total will match this fixed amount.")]
        [Guide("Any difference is posted to `SuspenseAccount`, creating an audit trail for discrepancies.")]
        [Guide("Review suspense account entries regularly to identify and resolve differences.")]
        [ProtoMember(35), IfTrue(nameof(FixedTotal)), NoLabel, NoWrap, AppendCurrency(nameof(ReceivedIn))] public decimal FixedTotalAmount { get; set; }
        [Prepend(nameof(Strings.OutOfBalance)), IfExpressionNotZero, Expression(Zero, Minus, nameof(FixedTotalAmount), Round, PlusArray, nameof(Line.TotalBeforeTax), PlusArray, nameof(Line.TaxAmount), Negate), NoLabel, AppendCurrency(nameof(ReceivedIn))] public object BalancingAmount { get; set; }
        [ProtoMember(15), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(16), NoLabel, IfTrue(nameof(CustomTheme)), Autocomplete(typeof(ManagerServer.Model.CustomTheme))] public Guid? CustomThemeId { get; set; }
        [ProtoMember(17), DoNotCopy] public bool AutomaticReference { get; set; }
        [ProtoMember(18), Label(nameof(Strings.CustomTitle))] public bool HasReceiptCustomTitle { get; set; }
        [ProtoMember(19), NoLabel, IfTrue(nameof(HasReceiptCustomTitle)), Placeholder(nameof(Strings.Receipt))] public string ReceiptCustomTitle { get; set; }
        [ProtoMember(40), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }
        [ProtoMember(42), Label(nameof(Strings.Footers))] public bool HasReceiptFooters { get; set; }
        [ProtoMember(43), Autocomplete(typeof(ManagerServer.Model.ReceiptFooter)), NoLabel, IfTrue(nameof(HasReceiptFooters))] public Guid[] ReceiptFooters { get; set; }
        [Guide("Add business-specific information using `CustomFields`.")]
        [Guide("Custom fields can track payment methods, authorization codes, deposit references, or any data unique to your business.")]
        [Guide("Set up custom fields under `Settings` → `CustomFields` before using them in receipts.")]
        [ProtoMember(20)] public Dictionary<Guid, string> CustomFields { get; set; }
        [Guide("Use enhanced `CustomFields` for advanced data types and validation.")]
        [Guide("Enhanced fields support dates, numbers, dropdown lists, and other structured data types.")]
        [Guide("Configure validation rules and default values under `Settings` → `CustomFields`.")]
        [ProtoMember(44)] public CustomFields CustomFields2 { get; set; }
        [Guide("System-generated identifier linking this receipt to imported bank feed transactions.")]
        [Guide("Used for automatic matching during bank reconciliation when importing bank statements.")]
        [Guide("This field is managed automatically and should not be edited manually.")]
        [ProtoMember(47), Hidden] public string FdxTransactionId { get; set; }

        public override string GetReference() => Reference;

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        DateTime IRecurringTransactionDestination.Date { get => Date; set => Date = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        DateTime IForeignCurrencyTransaction.Date => Date;
        Guid? IForeignCurrencyTransaction.Currency => ReceivedIn;
        decimal IForeignCurrencyTransaction.ExchangeRate { get => ExchangeRate; set => ExchangeRate = value; }
        bool IForeignCurrencyTransaction.ExchangeRateIsInverse { get => ExchangeRateIsInverse; set => ExchangeRateIsInverse = value; }
        string ICode.Code => Reference;

        public override bool GetHasLineDescription() => HasLineDescription;
        public override bool HasLineQty() => QuantityColumn;
        public override bool HasLineUnitPrice() => UnitPriceColumn;
        public override DiscountType? GetLineDicountType() => Discount ? DiscountType : null;

        [CustomFields]
        [ProtoContract]
        [Guid("4ab2302c-def1-49ec-abbf-b9a6dd393254")]
        public sealed class Line : ITransactionLine
        {
            [IfTrue(nameof(HasLineNumber)), LineNumber, Label("#")] public object LineNumber { get; }
            [ProtoMember(1), Autocomplete(typeof(ISaleItem)), OnChangeSetDefault(nameof(Qty)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(SalesUnitPrice)), OnChangeSetDefault(nameof(TaxCode)), OnChangeSetDefault(nameof(Division)), Short] public Guid? Item { get; set; }
            [ProtoMember(2), Autocomplete(typeof(IReceiptOrPaymentAccount), Subtext = nameof(BalanceSheetAccount.Group)), Substitute(nameof(Item), nameof(ISaleItem.SaleItemAccount)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(TaxCode))] public Guid? Account { get; set; }
            [ProtoMember(38), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsInterAccountTransfers)), Autocomplete(typeof(BankOrCashAccount)), Prepend(nameof(Strings.PaidFrom))] public Guid? InterAccountTransferAccount { get; set; }
            [ProtoMember(3), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsReceivable)), Autocomplete(typeof(Customer), Filter = nameof(Account)), Prepend(nameof(Strings.Customer)), Substitute(nameof(Customer)), OnChangeSetNull(nameof(AccountsReceivableSalesInvoice))] public Guid? AccountsReceivableCustomer { get; set; }
            [ProtoMember(4), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsReceivable)), IfNotNull(nameof(AccountsReceivableCustomer)), Autocomplete(typeof(SalesInvoice), Filter = nameof(AccountsReceivableCustomer)), Placeholder(nameof(Strings.Automatic)), Prepend(nameof(Strings.Invoice)), Short] public Guid? AccountsReceivableSalesInvoice { get; set; }
            [ProtoMember(5), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsBillableExpense)), Autocomplete(typeof(Customer)), Prepend(nameof(Strings.Customer))] public Guid? BillableExpenseCustomer { get; set; }
            [ProtoMember(6), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsBillableExpense)), IfNotNull(nameof(BillableExpenseCustomer)), Autocomplete(typeof(SalesInvoice), Filter = nameof(BillableExpenseCustomer)), Prepend(nameof(Strings.Invoice)), Placeholder(nameof(Strings.Uninvoiced)), Short] public Guid? BillableExpenseSalesInvoice { get; set; }
            [ProtoMember(7), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsPayable)), Substitute(nameof(Supplier)), Autocomplete(typeof(Supplier), Filter = nameof(Account)), Prepend(nameof(Strings.Supplier)), OnChangeSetNull(nameof(PurchaseInvoice))] public Guid? AccountsPayableSupplier { get; set; }
            [ProtoMember(8), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsPayable)), IfNotNull(nameof(AccountsPayableSupplier)), Autocomplete(typeof(PurchaseInvoice), Filter = nameof(AccountsPayableSupplier)), Prepend(nameof(Strings.Invoice)), Placeholder(nameof(Strings.Automatic)), Short] public Guid? PurchaseInvoice { get; set; }
            [ProtoMember(23), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsWithholdingTaxPayablePayable)), Autocomplete(typeof(Supplier)), Prepend(nameof(Strings.Supplier))] public Guid? WithholdingTaxPayableSupplier { get; set; }
            [ProtoMember(9), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsEmployeeClearingAccount)), Autocomplete(typeof(Employee), Filter = nameof(Account)), Prepend(nameof(Strings.Employee))] public Guid? Employee { get; set; }
            [ProtoMember(10), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), Autocomplete(typeof(CapitalAccount), Filter = nameof(Account)), Prepend(nameof(Strings.CapitalAccount))] public Guid? CapitalAccount { get; set; }
            [ProtoMember(11), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), IfNotNull(nameof(CapitalAccount)), Autocomplete(typeof(SubAccount)), Prepend(nameof(Strings.SubAccount))] public Guid? SubAccount { get; set; }
            [ProtoMember(12), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForSpecialAccounts)), Autocomplete(typeof(SpecialAccount), Filter = nameof(Account)), Prepend(nameof(Strings.SpecialAccount))] public Guid? SpecialAccount { get; set; }
            [ProtoMember(13), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForFixedAssets)), Autocomplete(typeof(FixedAsset), Filter = nameof(Account)), Prepend(nameof(Strings.FixedAsset))] public Guid? FixedAsset { get; set; }
            [ProtoMember(14), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForIntangibleAssets)), Autocomplete(typeof(IntangibleAsset), Filter = nameof(Account)), Prepend(nameof(Strings.IntangibleAsset))] public Guid? IntangibleAsset { get; set; }
            [ProtoMember(22), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.HasExpenseClaimPayers)), Autocomplete(typeof(ExpenseClaimsPayer), Filter = nameof(Account)), Prepend(nameof(Strings.ExpenseClaimsPayer))] public Guid? ExpenseClaimsPayer { get; set; }
            [ProtoMember(26), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForInvestments)), Autocomplete(typeof(Investment), Filter = nameof(Account)), Prepend(nameof(Strings.Investment))] public Guid? Investment { get; set; }
            [ProtoMember(15), IfTrue(nameof(HasLineDescription)), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [ProtoMember(16)] public Dictionary<Guid, string> CustomFields { get; set; }
            [ProtoMember(29)] public CustomFields CustomFields2 { get; set; }
            [ProtoMember(17), AppendValue(nameof(Item), nameof(ManagerServer.Model.InventoryItem.UnitName)), Short, IfTrue(nameof(QuantityColumn))] public decimal? Qty { get; set; }
            [ProtoMember(25), NoPlaceholder, AppendCurrency(nameof(ReceivedIn)), IfTrue(nameof(UnitPriceColumn)), Label(nameof(Strings.UnitPrice))] public decimal SalesUnitPrice { get; set; }
            [Expression(Zero, Plus, nameof(Amount), Divide, nameof(Qty), Round), AppendCurrency(nameof(ReceivedIn)), Label(nameof(Strings.UnitPrice)), IfTrue(nameof(QuantityColumn)), IfFalse(nameof(UnitPriceColumn))] public object AutoUnitPrice { get; set; }
            [ProtoMember(18), NoPlaceholder, AppendCurrency(nameof(ReceivedIn)), IfFalse(nameof(UnitPriceColumn))] public decimal Amount { get; set; }
            [ProtoMember(27), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.Percentage), Short, Append("%")] public decimal DiscountPercentage { get; set; }
            [ProtoMember(28), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.ExactAmount)] public decimal DiscountAmount { get; set; }
            [Label(nameof(Strings.Total)), Expression(Zero, Plus, nameof(SalesUnitPrice), Times, nameof(Qty), Plus, nameof(Amount), Minus, nameof(DiscountAmount), TimesPercentage, nameof(DiscountPercentage), Round), Sum, AppendCurrency(nameof(ReceivedIn))] public object TotalBeforeTax { get; }
            [ProtoMember(20), Autocomplete(typeof(TaxCode)), IfTrue(nameof(Account), nameof(NamedObject.TaxCodeEnabled)), Short] public Guid? TaxCode { get; set; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), TimesTaxCode, nameof(TaxCode)), Sum, IfTrue(nameof(AmountsAreTaxExclusive))] public object TaxAmount { get; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), Plus, nameof(TaxAmount), Round), Sum, AppendCurrency(nameof(ReceivedIn)), IfTrue(nameof(AmountsAreTaxExclusive))] public object Total { get; }
            [ProtoMember(19), IfDifferentCurrency, NoPlaceholder] public decimal CurrencyAmount { get; set; }
            //[ProtoMember(30), IfTrue(nameof(Item), nameof(ISaleItem.HasCostOfGoodsSold))] public AutomaticManual CostOfGoodsSold;
            //[ProtoMember(31), IfEnum(nameof(CostOfGoodsSold), 1), AppendBaseCurrency, EmptyLabel] public decimal CostOfGoodsSoldAmount;
            //[ProtoMember(32), IfNotNull(nameof(Investment)), IfNotNull(nameof(Qty)), Label(nameof(Strings.AverageCost)), Placeholder(nameof(Strings.Automatic)), AppendBaseCurrency] public decimal? InvestmentAverageCost;
            [ProtoMember(24), Autocomplete(typeof(Project)), IfTrue(nameof(Account), nameof(NamedObject.ProjectEnabled)), Short] public Guid? Project { get; set; }
            [ProtoMember(21), Autocomplete(typeof(Division)), IfTrue(nameof(Account), nameof(NamedObject.DivisionEnabled)), Short] public Guid? Division { get; set; }

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
            public override Guid? GetInvestment() => Investment;
            public override Guid? GetAccountsReceivableCustomer() => AccountsReceivableCustomer;
            public override Guid? GetAccountsPayableSupplier() => AccountsPayableSupplier;
            public override Guid? GetAccountsPayablePurchaseInvoice() => PurchaseInvoice;
            public override Guid? GetAccountsReceivableSalesInvoice() => AccountsReceivableSalesInvoice;
            public override Guid? GetExpenseClaimPayer() => ExpenseClaimsPayer;
            public override Guid? GetSubAccount() => SubAccount;
            public override Guid? GetInterAccountTransferAccount() => InterAccountTransferAccount;
            protected override decimal? GetUnitPrice() => SalesUnitPrice;
            protected override decimal? GetAmount() => Amount;
            protected override decimal? GetDiscountPercentage() => DiscountPercentage;
            protected override decimal? GetDiscountAmount() => DiscountAmount;
            protected override decimal? GetQty() => Qty;
            protected override string GetLineDescription() => LineDescription;
            public override Guid? GetTaxCode() => TaxCode;
            public override Guid? GetDivision() => Division;
            public override Dictionary<Guid, string> GetCustomFields() => CustomFields;
            public override CustomFields GetCustomFields2() => CustomFields2;
            public override decimal? GetProposedAccountAmount() => CurrencyAmount;
            protected override Guid? GetProject() => Project;
            //public override decimal? GetProposedCostOfGoodsSoldAmount() => (CostOfGoodsSold == AutomaticManual.Manual ? CostOfGoodsSoldAmount : null);
            //public override decimal? GetInvestmentAverageCost() => InvestmentAverageCost;
        }

        [ProtoMember(37)] public DateTime? Obsolete_BankClearDate { get; set; }
        [ProtoMember(8)] public BankClearStatus Obsolete_Status { get; set; }
        [ProtoMember(14)] public bool Obsolete_AmountsIncludeTax { get; set; }
        [ProtoMember(21)] public ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment Obsolete_ReceiptOrPayment { get; set; }

        public override string TransactionTitle => HasReceiptCustomTitle ? ReceiptCustomTitle : null;

        public bool IsUncategorized()
        {
            if (Lines == null) return true;
            if (Lines.All(x => !x.Item.HasValue && !x.Account.HasValue)) return true;
            return false;
        }

        public DateTime? GetClearDate()
        {
            if (Cleared == BankAccountClearStatus.OnTheSameDate) return Date;
            if (BankClearDate.HasValue && BankClearDate.Value < Date) return Date;
            return BankClearDate;
        }

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
            var bankAccount = database.SingleOrDefault<BankOrCashAccount>(ReceivedIn);
            
            var customer = database.SingleOrDefault<Customer>(Customer);
            var supplier = database.SingleOrDefault<Supplier>(Supplier);
            if (PaidBy != ManagerServer.Model.Enums.PayerPayeeType.Customer) customer = null;
            if (PaidBy != ManagerServer.Model.Enums.PayerPayeeType.Supplier) supplier = null;

            if (bankAccount != null) generalLedgerAccount = database.Single<BalanceSheetCashAtBankAccount>();

            var baseCurrency = database.Single<BaseCurrency>();
            var transactionCurrency = database.SingleOrDefault<ForeignCurrency>(bankAccount?.Currency) as Currency ?? baseCurrency;

            var inventoryLocation = database.SingleOrDefault<CustomInventoryLocation>(InventoryLocation);

            var list = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();
            if (Lines != null)
            {
                for (int i = 0; i < Lines.Length; i++)
                {
                    list.AddRange(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction.From(
                        database: database,
                        date: Date,
                        transaction: this,
                        exchangeRate: ExchangeRate,
                        exchangeRateIsInverse: ExchangeRateIsInverse,
                        transactionCurrency: transactionCurrency,
                        transactionLine: Lines[i],
                        amountsIncludeTax: !AmountsAreTaxExclusive,
                        reverseSign: true,
                        bankAccount: bankAccount,
                        customer: customer,
                        supplier: supplier,
                        inventoryLocation: inventoryLocation,
                        lineNumber: i
                    ));
                }
            }            

            Division trackingCode = null;
            if (bankAccount != null) trackingCode = database.SingleOrDefault<Division>(bankAccount.Division);

            if (FixedTotal)
            {
                var total = list.Select(x => x.TransactionAmount).SafeSum();
                var difference = total + transactionCurrency.Round(FixedTotalAmount);
                if (difference != 0m)
                {
                    var baseAmount = baseCurrency.GetBaseAmount(difference, ExchangeRate, ExchangeRateIsInverse, transactionCurrency);

                    list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        date: Date,
                        generalLedgerAccount: database.Single<BalanceSheetSuspenseAccount>(),
                        exchangeRate: ExchangeRate,
                        isExchangeRateInverse: ExchangeRateIsInverse,
                        transactionAmount: difference * -1m,
                        baseAmount: baseAmount*-1m,
                        transactionCurrency: transactionCurrency,
                        transactionLine: new Line(),
                        transaction: this,
                        bankAccount: bankAccount,
                        customer: customer,
                        supplier: supplier,
                        trackingCode: trackingCode
                    ));
                }
            }

            list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                date: Date,
                generalLedgerAccount: generalLedgerAccount,
                baseAmount: list.Select(x => x.BaseAmount).SafeSum() * -1m,
                transactionAmount: list.Select(x => x.TransactionAmount).SafeSum() * -1m,
                transactionCurrency: transactionCurrency,
                transaction: this,
                bankAccount: bankAccount,
                customer: customer,
                supplier: supplier,
                isBalancing: true,
                contraTransactions: list.ToArray(),
                trackingCode: trackingCode
            ));

            return list.ToArray();
        }

        int IComparable<Receipt>.CompareTo(Receipt other)
        {
            return (other.Date, other.Reference).CompareTo((Date, Reference));
        }
    }
}