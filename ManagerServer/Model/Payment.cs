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
    [Guid("79f99d26-e43a-4ecb-a9c9-0774601a9b2e")]
    [Currency(nameof(PaidFrom))]
    public sealed class Payment : Transaction, IHasAutomaticReference, IRecurringTransactionDestination, IComparable<Payment>, ICustomFields, IForeignCurrencyTransaction, ICode, IHasCustomTheme
    {
        [Guide("Enter the date when you made this payment.")]
        [Guide("This date determines when the payment is recorded in your books and which accounting period it belongs to.")]
        [Guide("For checks, use the date written on the check. For electronic payments, use the transaction date.")]
        [ProtoMember(1), NoWrap] public DateTime Date { get; set; }

        [Guide("Enter a reference number to uniquely identify this payment.")]
        [Guide("Common references include check numbers, wire transfer confirmations, or electronic payment IDs.")]
        [Guide("References help you match payments to bank statements and locate specific transactions later.")]
        [ProtoMember(2)] public string Reference { get; set; }

        [Guide("Select the `BankAccount` or `CashAccount` used to make this payment.")]
        [Guide("The selected account's balance will decrease by the payment amount.")]
        [Guide("If you haven't created the payment account yet, set it up under `BankAndCashAccounts` first.")]
        [ProtoMember(7), NoWrap, Autocomplete(typeof(ManagerServer.Model.IBankOrCashAccount)), Prepend(nameof(Strings.Account))] public Guid? PaidFrom { get; set; }

        [Guide("Select the clearing status for bank payments.")]
        [Guide("Choose `Cleared` if the payment has already been deducted from your bank account.")]
        [Guide("Choose `Pending` if the payment is issued but not yet showing on your bank statement.")]
        [Guide("This status is crucial for accurate bank reconciliation and cash flow reporting.")]
        [ProtoMember(37), NoWrap, IfTrue(nameof(PaidFrom), nameof(BankOrCashAccount.CanHavePendingTransactions))] public BankAccountClearStatus Cleared { get; set; }
        [ProtoMember(9), EmptyLabel, IfTrue(nameof(PaidFrom), nameof(BankOrCashAccount.CanHavePendingTransactions)), IfEnum(nameof(Cleared), (int)BankAccountClearStatus.OnALaterDate), Placeholder(nameof(Strings.Pending)), Prepend(nameof(Strings.Date)), DoNotCopy] public DateTime? BankClearDate { get; set; }

        [Guide("Enter the `ExchangeRate` for converting foreign currency payments to your `BaseCurrency`.")]
        [Guide("This rate applies when making payment from a foreign currency bank account.")]
        [Guide("The exchange rate determines the base currency value for financial reporting.")]
        [Guide("You can configure automatic exchange rates under `Settings` → `ExchangeRates`.")]
        [ProtoMember(46), Placeholder(nameof(Strings.Autofill)), NoWrap, IfNotNull(nameof(PaidFrom), nameof(BankOrCashAccount.Currency)), Prepend("1 {{ (ExchangeRateIsInverse ? baseCurrency.code : getCurrencyCode()) }} = "), Append("{{ (ExchangeRateIsInverse ? getCurrencyCode() : baseCurrency.code) }}")] public decimal ExchangeRate { get; set; }
        [ProtoMember(47), IfNotNull(nameof(PaidFrom), nameof(BankOrCashAccount.Currency)), Icon("fa-right-left")] public bool ExchangeRateIsInverse { get; set; }

        [Guide("Select the type of payee receiving this payment.")]
        [Guide("Choose `Customer` for refunds, overpayment returns, or other payments to customers.")]
        [Guide("Choose `Supplier` for vendor payments, purchase invoices, or supplier prepayments.")]
        [Guide("Choose `Other` for payments to non-customer/supplier parties like employees, tax authorities, or loan payments.")]
        [ProtoMember(3), NoWrap, Prepend(nameof(Strings.Contact))] public PayerPayeeType Payee { get; set; }
        [ProtoMember(4), EmptyLabel, NoWrap, IfEnum(nameof(Payee), (int)PayerPayeeType.Customer), Autocomplete(typeof(ManagerServer.Model.Customer))] public Guid? Customer { get; set; }
        [ProtoMember(5), EmptyLabel, NoWrap, IfEnum(nameof(Payee), (int)PayerPayeeType.Supplier), Autocomplete(typeof(ManagerServer.Model.Supplier))] public Guid? Supplier { get; set; }
        [ProtoMember(6), EmptyLabel, IfEnum(nameof(Payee), (int)PayerPayeeType.Other), Placeholder(nameof(Strings.Optional)), Typeahead] public string Contact { get; set; }

        [Guide("Enter an optional description to provide context about this payment.")]
        [Guide("Descriptions help identify the payment purpose when reviewing transactions.")]
        [Guide("Include details like invoice numbers, purchase order references, or the reason for payment.")]
        [ProtoMember(10), Long, Placeholder(nameof(Strings.Optional))] public string Description { get; set; }

        [Guide("Add line items to allocate this payment to appropriate accounts.")]
        [Guide("Each line can post to different `ExpenseAccounts`, apply to `PurchaseInvoices`, or record `PurchaseItems`.")]
        [Guide("Use multiple lines to split a single payment across different expense categories or invoices.")]
        [Guide("Lines without accounts will be automatically allocated to the oldest unpaid invoices for the selected supplier.")]
        [Fields(typeof(Line))]
        [ProtoMember(11)] public Line[] Lines { get; set; }

        [Guide("Select the `InventoryLocation` when purchasing inventory items.")]
        [Guide("This determines which warehouse or location will receive the purchased inventory.")]
        [Guide("Only appears when you have enabled inventory locations and are purchasing inventory items.")]
        [ProtoMember(30), NoLabel, Prepend(nameof(Strings.InventoryLocation)), Autocomplete(typeof(CustomInventoryLocation)), IfAnyNotNull(nameof(Line.Item))] public Guid? InventoryLocation { get; set; }

        [Guide("Enable line numbers to display sequential numbering for each payment line.")]
        [Guide("Line numbers help when discussing or referencing specific lines in complex payments.")]
        [Guide("Useful for matching payment details to supporting documentation or purchase orders.")]
        [ProtoMember(42), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }

        [Guide("Enable the `Description` column to add detailed explanations for each payment line.")]
        [Guide("Line descriptions document what each portion of a split payment is for.")]
        [Guide("Essential for expense payments that need detailed documentation for approval or reimbursement.")]
        [ProtoMember(13), Label(nameof(Strings.Column), nameof(Strings.Description))] public bool HasLineDescription { get; set; }

        [Guide("Enable `Qty` and `UnitPrice` columns for quantity-based purchases.")]
        [Guide("Essential for inventory purchases where you need to record quantities and unit costs.")]
        [Guide("Also useful for services billed by hours, units, or other measurable quantities.")]
        [Guide("The system calculates line totals by multiplying quantity by unit price.")]
        [ProtoMember(33), Label(nameof(Strings.Column), nameof(Strings.Qty))] public bool QuantityColumn { get; set; }
        [ProtoMember(34), Label(nameof(Strings.Column), nameof(Strings.UnitPrice)), IfTrue(nameof(QuantityColumn))] public bool UnitPriceColumn { get; set; }

        [Guide("Enable the `Discount` column to apply discounts to payment lines.")]
        [Guide("Choose between percentage discounts or fixed amount discounts.")]
        [Guide("Discounts are calculated per line and reduce the amount before tax calculations.")]
        [Guide("Useful for early payment discounts, volume discounts, or negotiated price reductions.")]
        [ProtoMember(39), Label(nameof(Strings.Column), nameof(Strings.Discount))] public bool Discount { get; set; }
        [ProtoMember(40), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }

        [Guide("Specify whether line amounts include or exclude tax.")]
        [Guide("Check this box if amounts are tax-exclusive - tax will be calculated and added to line amounts.")]
        [Guide("Leave unchecked if amounts already include tax - tax will be calculated but included in the line amount.")]
        [Guide("This setting affects the final payment total and how tax amounts are displayed.")]
        [ProtoMember(32), IfContains<TaxCode>] public bool AmountsAreTaxExclusive { get; set; }

        [Guide("Enable fixed total to force this payment to match a specific amount.")]
        [Guide("Useful when you need to match exact bank transaction amounts or handle rounding differences.")]
        [Guide("Any difference between line items and the fixed total will be automatically posted to `SuspenseAccount`.")]
        [Guide("The suspense account entry helps you investigate and correct discrepancies later.")]
        [ProtoMember(35)] public bool FixedTotal { get; set; }
        [ProtoMember(36), IfTrue(nameof(FixedTotal)), NoLabel, NoWrap, AppendCurrency(nameof(PaidFrom))] public decimal FixedTotalAmount { get; set; }
        [Prepend(nameof(Strings.OutOfBalance)), IfExpressionNotZero, Expression(Zero, Minus, nameof(FixedTotalAmount), Round, PlusArray, nameof(Line.TotalBeforeTax), PlusArray, nameof(Line.TaxAmount), Negate), NoLabel, AppendCurrency(nameof(PaidFrom))] public object BalancingAmount { get; set; }

        [ProtoMember(15), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(16), NoLabel, IfTrue(nameof(CustomTheme)), Autocomplete(typeof(ManagerServer.Model.CustomTheme))] public Guid? CustomThemeId { get; set; }
        [ProtoMember(17), DoNotCopy] public bool AutomaticReference { get; set; }

        [Guide("Enable custom title to replace the default 'Payment' heading on forms.")]
        [Guide("Useful for creating specialized payment types like 'Expense Reimbursement' or 'Vendor Payment'.")]
        [Guide("The custom title appears on printed and emailed payment forms.")]
        [ProtoMember(18), Label(nameof(Strings.CustomTitle))] public bool HasPaymentCustomTitle { get; set; }
        [ProtoMember(19), NoLabel, IfTrue(nameof(HasPaymentCustomTitle)), Placeholder(nameof(Strings.Payment))] public string PaymentCustomTitle { get; set; }

        [Guide("Enable the tax amount column to display calculated tax for each line.")]
        [Guide("Shows how tax is calculated line by line, with rounding applied to each line separately.")]
        [Guide("Helps verify tax calculations and ensures compliance with tax rounding rules.")]
        [Guide("The total tax is the sum of individually rounded line taxes, not a calculation on the total.")]
        [ProtoMember(41), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }

        [Guide("Enable custom footers to add additional information at the bottom of payment forms.")]
        [Guide("Footers can include payment terms, remittance instructions, or authorization signatures.")]
        [Guide("Create reusable footers under `Settings` → `Footers` and select them here.")]
        [ProtoMember(43), Label(nameof(Strings.Footers))] public bool HasPaymentFooters { get; set; }
        [ProtoMember(44), Autocomplete(typeof(ManagerServer.Model.PaymentFooter)), NoLabel, IfTrue(nameof(HasPaymentFooters))] public Guid[] PaymentFooters { get; set; }

        [Guide("Add business-specific information using `CustomFields`.")]
        [Guide("Custom fields can track approval codes, project numbers, cost centers, or any data unique to your business.")]
        [Guide("Set up custom fields under `Settings` → `CustomFields` before using them in payments.")]
        [ProtoMember(31)] public Dictionary<Guid, string> CustomFields { get; set; }
        [Guide("Use enhanced `CustomFields` for advanced data types and validation.")]
        [Guide("Enhanced fields support dates, numbers, dropdown lists, and other structured data types.")]
        [Guide("Configure validation rules and default values under `Settings` → `CustomFields`.")]
        [ProtoMember(45)] public CustomFields CustomFields2 { get; set; }

        [Guide("System-generated identifier linking this payment to imported bank feed transactions.")]
        [Guide("Used for automatic matching during bank reconciliation when importing bank statements.")]
        [Guide("This field is managed automatically and should not be edited manually.")]
        [ProtoMember(48), Hidden] public string FdxTransactionId { get; set; }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        DateTime IRecurringTransactionDestination.Date { get => Date; set => Date = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        DateTime IForeignCurrencyTransaction.Date => Date;
        Guid? IForeignCurrencyTransaction.Currency => PaidFrom;
        decimal IForeignCurrencyTransaction.ExchangeRate { get => ExchangeRate; set => ExchangeRate = value; }
        bool IForeignCurrencyTransaction.ExchangeRateIsInverse { get => ExchangeRateIsInverse; set => ExchangeRateIsInverse = value; }
        string ICode.Code => Reference;

        public override string TransactionTitle => HasPaymentCustomTitle ? PaymentCustomTitle : null;

        public override bool GetHasLineDescription() => HasLineDescription;
        public override bool HasLineQty() => QuantityColumn;
        public override bool HasLineUnitPrice() => UnitPriceColumn;
        public override DiscountType? GetLineDicountType() => Discount ? DiscountType : null;

        [CustomFields]
        [ProtoContract]
        [Guid("21ae8f9b-6289-4461-9267-c89617c42363")]
        public sealed class Line : ITransactionLine
        {
            [IfTrue(nameof(HasLineNumber)), LineNumber, Label("#")] public object LineNumber { get; }

            [Guide("Select an `InventoryItem` or `NonInventoryItem` to purchase.")]
            [Guide("When an item is selected, the appropriate expense account is automatically filled based on the item's settings.")]
            [Guide("Leave blank if you want to manually specify the account instead.")]
            [ProtoMember(1), Autocomplete(typeof(IPurchaseItem)), OnChangeSetDefault(nameof(Qty)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(PurchaseUnitPrice)), OnChangeSetDefault(nameof(TaxCode)), OnChangeSetDefault(nameof(Division)), Short] public Guid? Item { get; set; }

            [Guide("Select the account to categorize this payment line.")]
            [Guide("If you selected an `Item`, the account is automatically filled from the item's purchase account setting.")]
            [Guide("For direct expense payments, choose the appropriate expense account from your `ChartOfAccounts`.")]
            [Guide("Common expense accounts include utilities, rent, supplies, or professional fees.")]
            [SelectAccountScreenshot(accountName: nameof(Strings.Electricity))]
            [Guide("For payments to suppliers against invoices, select `AccountsPayable` and then choose the `Supplier`.")]
            [SelectAccountScreenshot(accountName: nameof(Strings.AccountsPayable), prepend: nameof(Strings.Supplier))]
            [Guide("When paying suppliers, you can select a specific `PurchaseInvoice` or leave it automatic.")]
            [Guide("Automatic allocation applies payments to the oldest unpaid invoices first (FIFO method).")]
            [Guide("For fixed asset purchases, select `Fixed_assets_at_cost` and then the specific `FixedAsset`.")]
            [SelectAccountScreenshot(accountName: nameof(Strings.Fixed_assets_at_cost), prepend: nameof(Strings.FixedAsset))]
            [Guide("For billable expenses that customers will reimburse, select `BillableExpenses` and the `Customer`.")]
            [SelectAccountScreenshot(accountName: nameof(Strings.BillableExpenses), prepend: nameof(Strings.Customer))]
            [Guide("For employee payments after payroll, select `EmployeeClearingAccount` and the `Employee`.")]
            [SelectAccountScreenshot(accountName: nameof(Strings.EmployeeClearingAccount), prepend: nameof(Strings.Employee))]
            [Guide("The account selection determines how this payment appears in financial reports and affects account balances.")]
            [ProtoMember(2), Autocomplete(typeof(IReceiptOrPaymentAccount), Subtext = nameof(BalanceSheetAccount.Group)), Substitute(nameof(Item), nameof(IPurchaseItem.PurchaseItemAccount)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(TaxCode))] public Guid? Account { get; set; }
            [ProtoMember(39), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsInterAccountTransfers)), Autocomplete(typeof(BankOrCashAccount)), Prepend(nameof(Strings.ReceivedIn))] public Guid? InterAccountTransferAccount { get; set; }
            [ProtoMember(3), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsReceivable)), Autocomplete(typeof(Customer), Filter = nameof(Account)), Prepend(nameof(Strings.Customer)), OnChangeSetNull(nameof(AccountsReceivableSalesInvoice)), Substitute(nameof(Customer))] public Guid? AccountsReceivableCustomer { get; set; }
            [ProtoMember(4), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsReceivable)), IfNotNull(nameof(AccountsReceivableCustomer)), Autocomplete(typeof(SalesInvoice), Filter = nameof(AccountsReceivableCustomer)), Placeholder(nameof(Strings.Automatic)), Prepend(nameof(Strings.Invoice)), Short] public Guid? AccountsReceivableSalesInvoice { get; set; }
            [ProtoMember(5), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsBillableExpense)), Autocomplete(typeof(Customer)), Prepend(nameof(Strings.Customer))] public Guid? BillableExpenseCustomer { get; set; }
            [ProtoMember(6), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsBillableExpense)), IfNotNull(nameof(BillableExpenseCustomer)), Autocomplete(typeof(SalesInvoice), Filter = nameof(BillableExpenseCustomer)), Prepend(nameof(Strings.Invoice)), Placeholder(nameof(Strings.Uninvoiced)), Short] public Guid? BillableExpenseSalesInvoice { get; set; }
            [ProtoMember(7), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsAccountsPayable)), Substitute(nameof(Supplier)), Autocomplete(typeof(Supplier), Filter = nameof(Account)), OnChangeSetNull(nameof(PurchaseInvoice)), Prepend(nameof(Strings.Supplier))] public Guid? AccountsPayableSupplier { get; set; }
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

            [Guide("Enter a description for this payment line.")]
            [Guide("Descriptions provide details about what this specific line item is paying for.")]
            [Guide("This field only appears when the `Description` column is enabled on the payment form.")]
            [ProtoMember(15), IfTrue(nameof(HasLineDescription)), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [Guide("Add line-specific custom information using `CustomFields`.")]
            [Guide("Line custom fields can track cost codes, authorization numbers, or other line-level data.")]
            [Guide("Configure line custom fields under `Settings` → `CustomFields` for payment lines.")]
            [ProtoMember(16)] public Dictionary<Guid, string> CustomFields { get; set; }
            [Guide("Use enhanced `CustomFields` for line-specific structured data.")]
            [Guide("Supports dates, numbers, dropdown selections, and other data types at the line level.")]
            [Guide("Useful for detailed tracking requirements that vary by payment line.")]
            [ProtoMember(29)] public CustomFields CustomFields2 { get; set; }

            [Guide("Enter the quantity of items being purchased.")]
            [Guide("For inventory items, this updates your stock levels at the specified location.")]
            [Guide("For services, enter hours, units, or other measurable quantities.")]
            [Guide("This field only appears when the `Qty` column is enabled.")]
            [ProtoMember(17), AppendValue(nameof(Item), nameof(InventoryItem.UnitName)), Short, IfTrue(nameof(QuantityColumn))] public decimal? Qty { get; set; }

            [Guide("Enter the price per unit for this line item.")]
            [Guide("The unit price multiplied by quantity gives the line total before discounts and tax.")]
            [Guide("For services, this might be an hourly rate or price per unit of service.")]
            [ProtoMember(25), NoPlaceholder, AppendCurrency(nameof(PaidFrom)), IfTrue(nameof(UnitPriceColumn)), Label(nameof(Strings.UnitPrice))] public decimal PurchaseUnitPrice { get; set; }
            [Expression(Zero, Plus, nameof(Amount), Divide, nameof(Qty), Round), AppendCurrency(nameof(PaidFrom)), Label(nameof(Strings.UnitPrice)), IfTrue(nameof(QuantityColumn)), IfFalse(nameof(UnitPriceColumn))] public object AutoUnitPrice { get; set; }
            [ProtoMember(18), NoPlaceholder, AppendCurrency(nameof(PaidFrom)), IfFalse(nameof(UnitPriceColumn))] public decimal Amount { get; set; }
            [ProtoMember(19), IfDifferentCurrency, NoPlaceholder] public decimal CurrencyAmount { get; set; }
            [ProtoMember(27), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.Percentage), Short, Append("%")] public decimal DiscountPercentage { get; set; }
            [ProtoMember(28), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.ExactAmount)] public decimal DiscountAmount { get; set; }
            [Label(nameof(Strings.Total)), Expression(Zero, Plus, nameof(PurchaseUnitPrice), Times, nameof(Qty), Plus, nameof(Amount), Minus, nameof(DiscountAmount), TimesPercentage, nameof(DiscountPercentage), Round), Sum, AppendCurrency(nameof(PaidFrom))] public object TotalBeforeTax { get; }
            [ProtoMember(20), Autocomplete(typeof(TaxCode)), IfTrue(nameof(Account), nameof(NamedObject.TaxCodeEnabled)), Short] public Guid? TaxCode { get; set; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), TimesTaxCode, nameof(TaxCode)), Sum, IfTrue(nameof(AmountsAreTaxExclusive))] public object TaxAmount { get; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), Plus, nameof(TaxAmount), Round), Sum, AppendCurrency(nameof(PaidFrom)), IfTrue(nameof(AmountsAreTaxExclusive))] public object Total { get; }
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
            public override Guid? GetInterAccountTransferAccount() => InterAccountTransferAccount;
            public override Guid? GetSubAccount() => SubAccount;
            protected override decimal? GetUnitPrice() => PurchaseUnitPrice;
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
        }

        [ProtoMember(38)] public DateTime? Obsolete_BankClearDate { get; set; }
        [ProtoMember(8)] public BankClearStatus Obsolete_Status { get; set; }
        [ProtoMember(14)] public bool Obsolete_AmountsIncludeTax { get; set; }
        [ProtoMember(21)] public ManagerServer.Model.Obsolete.Obsolete66.ReceiptOrPayment Obsolete_ReceiptOrPayment { get; set; }

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

        public override string GetReference() => Reference;

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
            var bankAccount = database.SingleOrDefault<BankOrCashAccount>(PaidFrom);

            var customer = database.SingleOrDefault<Customer>(Customer);
            var supplier = database.SingleOrDefault<Supplier>(Supplier);
            if (Payee != ManagerServer.Model.Enums.PayerPayeeType.Customer) customer = null;
            if (Payee != ManagerServer.Model.Enums.PayerPayeeType.Supplier) supplier = null;

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
                        transactionCurrency: transactionCurrency,
                        transactionLine: Lines[i],
                        exchangeRate: ExchangeRate,
                        exchangeRateIsInverse: ExchangeRateIsInverse,
                        amountsIncludeTax: !AmountsAreTaxExclusive,
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
                var transactionTotal = list.Select(x => x.TransactionAmount).SafeSum();
                var transactionDifference = transactionTotal - transactionCurrency.Round(FixedTotalAmount);
                if (transactionDifference != 0m)
                {
                    var baseAmount = baseCurrency.GetBaseAmount(transactionDifference, ExchangeRate, ExchangeRateIsInverse, transactionCurrency);

                    list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        date: Date,
                        generalLedgerAccount: database.Single<BalanceSheetSuspenseAccount>(),
                        exchangeRate: ExchangeRate,
                        isExchangeRateInverse: ExchangeRateIsInverse,
                        transactionAmount: transactionDifference * -1m,
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

        int IComparable<Payment>.CompareTo(Payment other)
        {
            return (other.Date, other.Reference).CompareTo((Date, Reference));
        }
    }
}
