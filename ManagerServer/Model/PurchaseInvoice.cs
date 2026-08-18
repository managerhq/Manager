using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Obsolete.Obsolete32;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using static ManagerServer.Model.Attributes.ExpressionAttribute.Operators;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("58b9eb90-f6b8-4abc-8ea1-12fd77b8336e")]
    [Currency(nameof(Supplier))]
    public sealed class PurchaseInvoice : Transaction, IHasAutomaticReference, IRecurringTransactionDestination, ICustomFields, IComparable<PurchaseInvoice>, IForeignCurrencyTransaction, ICode, IHasCustomTheme
    {
        [Guide("Enter the date shown on the supplier's invoice.")]
        [Guide("This date determines when the expense is recognized in your accounting records.")]
        [Guide("The issue date is used to calculate payment due dates based on supplier payment terms.")]
        [ProtoMember(1), NoWrap] public DateTime IssueDate { get; set; }

        [Guide("Select payment terms to determine when this invoice must be paid.")]
        [Guide("Choose `Immediate` for invoices requiring immediate payment.")]
        [Guide("Choose `Net` to specify payment due after a number of days.")]
        [Guide("Choose `By` to set a specific date for payment.")]
        [ProtoMember(31), NoWrap] public DueDateType DueDate { get; set; }
        [ProtoMember(19), NoWrap, EmptyLabel, IfEnum(nameof(DueDate), (int)DueDateType.Net), Append(nameof(Strings.Days))] public int? DueDateDays { get; set; }
        [ProtoMember(5), EmptyLabel, NoWrap, IfEnum(nameof(DueDate), (int)DueDateType.By), DoNotCopy] public DateTime? DueDateDate { get; set; }

        [Guide("Enter the invoice number or reference from your supplier's invoice.")]
        [Guide("This reference helps match payments to invoices and resolve supplier queries.")]
        [Guide("Each supplier invoice should have a unique reference to avoid duplicates.")]
        [ProtoMember(2), NoWrap] public string Reference { get; set; }
        [ProtoMember(12), Short, IfNull(nameof(PurchaseOrder)), Placeholder(nameof(Strings.Optional)), IfNotEmpty] public string OrderNumber { get; set; }

        [Guide("Select the `Supplier` who issued this invoice.")]
        [Guide("The supplier selection determines payment terms and account categorization.")]
        [Guide("Create new suppliers under the `Suppliers` tab before entering invoices.")]
        [Guide("Supplier currency settings determine if this is a foreign currency invoice.")]
        [ProtoMember(3), NoWrap, Autocomplete(typeof(Supplier))] public Guid? Supplier { get; set; }

        [Guide("Link this invoice to a `PurchaseQuote` if it originated from a quote.")]
        [Guide("Linking helps track the procurement process from quote to invoice.")]
        [Guide("Quote details can be copied to the invoice to ensure pricing consistency.")]
        [ProtoMember(28), NoWrap, IfNotNull(nameof(Supplier)), Short, Autocomplete(typeof(PurchaseQuote), Filter = nameof(Supplier)), Placeholder(nameof(Strings.Optional)), EmptyLabel, Prepend(nameof(Strings.QuoteNumber))] public Guid? PurchaseQuote { get; set; }

        [Guide("Link this invoice to a `PurchaseOrder` if fulfilling an order.")]
        [Guide("Linking ensures all purchase orders are properly matched to invoices.")]
        [Guide("Order details and items can be copied to verify invoice accuracy.")]
        [Guide("The system tracks which orders have been partially or fully invoiced.")]
        [ProtoMember(29), IfNotNull(nameof(Supplier)), Short, Autocomplete(typeof(PurchaseOrder), Filter = nameof(Supplier)), Placeholder(nameof(Strings.Optional)), EmptyLabel, Prepend(nameof(Strings.OrderNumber))] public Guid? PurchaseOrder { get; set; }

        [Guide("Enter the `ExchangeRate` when the supplier uses a foreign currency.")]
        [Guide("This field appears when the selected supplier's currency differs from your base currency.")]
        [Guide("The exchange rate converts foreign currency amounts to base currency for reporting.")]
        [Guide("Configure automatic exchange rates under `Settings` → `ExchangeRates`.")]
        [ProtoMember(61), Placeholder(nameof(Strings.Autofill)), NoWrap, IfNotNull(nameof(Supplier), nameof(Model.Supplier.Currency)), Prepend("1 {{ (ExchangeRateIsInverse ? baseCurrency.code : getCurrencyCode()) }} = "), Append("{{ (ExchangeRateIsInverse ? getCurrencyCode() : baseCurrency.code) }}")] public decimal ExchangeRate { get; set; }
        [ProtoMember(62), IfNotNull(nameof(Supplier), nameof(Model.Supplier.Currency)), Icon("fa-right-left")] public bool ExchangeRateIsInverse { get; set; }

        [Guide("Enter an optional description for this purchase invoice.")]
        [Guide("Use this field for general notes about the purchase or delivery details.")]
        [Guide("Descriptions help identify the invoice purpose when reviewing transactions.")]
        [ProtoMember(9), Long] public string Description { get; set; }

        [Guide("Add line items to detail what you are being charged for.")]
        [Guide("Each line can represent different products, services, or expense categories.")]
        [Guide("Use multiple lines to match the supplier's invoice layout for easy reconciliation.")]
        [Guide("Line totals are automatically calculated based on quantity, price, discounts, and tax.")]
        [Fields(typeof(Line))]
        [ProtoMember(23)] public Line[] Lines { get; set; }

        [Guide("Enable line numbers to display sequential numbering for each invoice line.")]
        [Guide("Line numbers help when discussing specific items with suppliers.")]
        [Guide("Useful for matching invoice lines to purchase orders or delivery notes.")]
        [ProtoMember(58), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }

        [Guide("Enable the `Description` column to add detailed explanations for each line item.")]
        [Guide("Descriptions provide additional context beyond the item or account name.")]
        [Guide("Essential for services or expenses that need detailed documentation.")]
        [ProtoMember(22), Label(nameof(Strings.Column), nameof(Strings.Description))] public bool HasLineDescription { get; set; }

        [Guide("Enable the `Discount` column to record discounts received on line items.")]
        [Guide("Choose between percentage discounts or fixed amount discounts per line.")]
        [Guide("Discounts reduce the line amount before tax calculations.")]
        [Guide("Useful for volume discounts, early payment discounts, or negotiated savings.")]
        [ProtoMember(14), Label(nameof(Strings.Column), nameof(Strings.Discount))] public bool Discount { get; set; }
        [ProtoMember(15), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        //[ProtoMember(63), Label(nameof(Strings.Column), nameof(Strings.QtyReceived))] public bool HasQtyReceived;

        [ProtoMember(65)] public bool FreightIn { get; set; }
        [ProtoMember(68), IfTrue(nameof(FreightIn)), EmptyLabel] public LandedCostLine[] LandedCostLines { get; set; }

        [Guide("Specify whether line item amounts include or exclude tax.")]
        [Guide("Check this box if the supplier's prices already include tax.")]
        [Guide("Leave unchecked if tax is shown separately and should be added to prices.")]
        [Guide("This must match how the supplier presents prices on their invoice.")]
        [ProtoMember(7), IfContains<TaxCode>] public bool AmountsIncludeTax { get; set; }

        [Guide("Enable withholding tax if you must deduct tax before paying the supplier.")]
        [Guide("Withholding tax is common for services, royalties, or contractor payments.")]
        [Guide("The withheld amount is paid to tax authorities on the supplier's behalf.")]
        [Guide("Check local tax regulations for withholding requirements and rates.")]
        [ProtoMember(24), IfWithholdingTaxPayable] public bool WithholdingTax { get; set; }
        [ProtoMember(25), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel] public WithholdingTaxType WithholdingTaxType { get; set; }
        [ProtoMember(26), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel, Append("%"), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Rate)] public decimal WithholdingTaxPercentage { get; set; }
        [ProtoMember(27), IfTrue(nameof(WithholdingTax)), NoLabel, AppendCurrency(nameof(Supplier)), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Amount)] public decimal WithholdingTaxAmount { get; set; }
        [ProtoMember(16), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(17), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [Guide("Hide the balance due amount when printing or emailing this invoice.")]
        [Guide("Useful for internal copies or when payment details are handled separately.")]
        [Guide("The balance due is still tracked in the system for payment matching.")]
        [ProtoMember(56), Label(nameof(Strings.Hide), nameof(Strings.BalanceDue))] public bool HideBalanceDue { get; set; }

        [Guide("Enable the tax amount column to display calculated tax for each line.")]
        [Guide("Shows how tax is calculated line by line for verification.")]
        [Guide("Helps match tax calculations to the supplier's invoice.")]
        [Guide("Total tax is the sum of individually calculated line taxes.")]
        [ProtoMember(57), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }

        [Guide("Enable this option when receiving inventory items with the invoice.")]
        [Guide("This combines the purchase invoice with a goods receipt for efficiency.")]
        [Guide("Inventory quantities will be updated immediately upon invoice entry.")]
        [Guide("Select the inventory location where goods are being received.")]
        [ProtoMember(64)] public bool AlsoActsAsGoodsReceipt { get; set; }
        [ProtoMember(13), IfTrue(nameof(AlsoActsAsGoodsReceipt)), NoLabel, Prepend(nameof(Strings.InventoryLocation)), Autocomplete(typeof(ManagerServer.Model.CustomInventoryLocation))] public Guid? PurchaseInventoryLocation { get; set; }

        [Guide("Enable custom footers to add additional information when printing invoices.")]
        [Guide("Footers can include payment instructions, terms, or internal notes.")]
        [Guide("Create reusable footers under `Settings` → `Footers` and select them here.")]
        [ProtoMember(59), Label(nameof(Strings.Footers))] public bool HasPurchaseInvoiceFooters { get; set; }
        [ProtoMember(60), Autocomplete(typeof(ManagerServer.Model.PurchaseInvoiceFooter)), NoLabel, IfTrue(nameof(HasPurchaseInvoiceFooters))] public Guid[] PurchaseInvoiceFooters { get; set; }
        
        [Guide("Archive this invoice to remove it from active lists and dropdowns.")]
        [Guide("Archived invoices are typically fully paid or no longer relevant.")]
        [Guide("The invoice remains in the system for reporting and audit purposes.")]
        [Guide("You can still view archived invoices through search or reports.")]
        [ProtoMember(30), IfExists, DoNotCopy] public bool ClosedInvoice { get; set; }
        [ProtoMember(20), DoNotCopy] public bool AutomaticReference { get; set; }
        [Guide("Add business-specific information using `CustomFields`.")]
        [Guide("Custom fields can track approval codes, project numbers, department codes, or any data unique to your business.")]
        [Guide("Set up custom fields under `Settings` → `CustomFields` before using them in invoices.")]
        [ProtoMember(8)] public Dictionary<Guid, string> CustomFields { get; set; }
        [Guide("Use enhanced `CustomFields` for advanced data types and validation.")]
        [Guide("Enhanced fields support dates, numbers, dropdown lists, and other structured data types.")]
        [Guide("Configure validation rules and default values under `Settings` → `CustomFields`.")]
        [ProtoMember(32)] public CustomFields CustomFields2 { get; set; }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        DateTime IRecurringTransactionDestination.Date { get => IssueDate; set => IssueDate = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        DateTime IForeignCurrencyTransaction.Date => IssueDate;
        Guid? IForeignCurrencyTransaction.Currency => Supplier;
        decimal IForeignCurrencyTransaction.ExchangeRate { get => ExchangeRate; set => ExchangeRate = value; }
        bool IForeignCurrencyTransaction.ExchangeRateIsInverse { get => ExchangeRateIsInverse; set => ExchangeRateIsInverse = value; }

        public override string GetReference() => Reference;

        public override bool GetHasLineDescription() => HasLineDescription;
        public override bool HasLineQty() => true;
        public override bool HasLineUnitPrice() => true;
        public override DiscountType? GetLineDicountType() => Discount ? DiscountType : null;
        string ICode.Code => Reference;

        public override bool IsInactive()
        {
            return ClosedInvoice;
        }

        [CustomFields]
        [ProtoContract]
        [Guid("b4ab284f-2b63-40b2-90ab-0daf35a0f7c5")]
        public sealed class Line : ITransactionLine
        {
            [IfTrue(nameof(HasLineNumber)), LineNumber, Label("#")] public object LineNumber { get; }

            [Guide("Choose an item, which could be either an `InventoryItem` or a `NonInventoryItem`. You also have the option to leave this field blank.")]
            [ProtoMember(1), Autocomplete(typeof(IPurchaseItem)), OnChangeSetDefault(nameof(Qty)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(PurchaseUnitPrice)), OnChangeSetDefault(nameof(TaxCode)), OnChangeSetDefault(nameof(Division)), Short] public Guid? Item { get; set; }

            [Guide("If you've previously chosen an `Item`, then the `Account` will be automatically populated based on that item.")]
            [Guide("To categorize the payment, you can choose from nearly any account in your `ChartOfAccounts`.")]
            [Guide("For example, if you are making a payment for an expense like electricity, choose the `Electricity` account.")]
            [SelectAccountScreenshot(accountName: nameof(Strings.Electricity))]
            [Guide("However you can also categorize payments directly into many sub-accounts.")]
            [Guide("For example, if this payment is for the purchase of a fixed asset, choose the `Fixed_assets_at_cost` account and then select the specific ``FixedAsset``.")]
            [SelectAccountScreenshot(accountName: nameof(Strings.Fixed_assets_at_cost), prepend: nameof(Strings.FixedAsset))]
            [ProtoMember(2), Autocomplete(typeof(IPurchaseInvoiceAccount), Subtext = nameof(BalanceSheetAccount.Group)), Substitute(nameof(Item), nameof(IPurchaseItem.PurchaseItemAccount)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(TaxCode))] public Guid? Account { get; set; }
            [ProtoMember(5), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsBillableExpense)), Autocomplete(typeof(Customer)), Prepend(nameof(Strings.Customer))] public Guid? BillableExpenseCustomer { get; set; }
            [ProtoMember(6), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsBillableExpense)), IfNotNull(nameof(BillableExpenseCustomer)), Autocomplete(typeof(SalesInvoice), Filter = nameof(BillableExpenseCustomer)), Prepend(nameof(Strings.Invoice)), Placeholder(nameof(Strings.Uninvoiced)), Short] public Guid? BillableExpenseSalesInvoice { get; set; }
            [ProtoMember(9), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), Autocomplete(typeof(CapitalAccount), Filter = nameof(Account)), Prepend(nameof(Strings.CapitalAccount))] public Guid? CapitalAccount { get; set; }
            [ProtoMember(10), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), IfNotNull(nameof(CapitalAccount)), Autocomplete(typeof(SubAccount)), Prepend(nameof(Strings.SubAccount))] public Guid? SubAccount { get; set; }
            [ProtoMember(11), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsEmployeeClearingAccount)), Autocomplete(typeof(Employee), Filter = nameof(Account)), Prepend(nameof(Strings.Employee))] public Guid? Employee { get; set; }
            [ProtoMember(13), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForSpecialAccounts)), Autocomplete(typeof(SpecialAccount), Filter = nameof(Account)), Prepend(nameof(Strings.SpecialAccount))] public Guid? SpecialAccount { get; set; }
            [ProtoMember(14), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForFixedAssets)), Autocomplete(typeof(FixedAsset), Filter = nameof(Account)), Prepend(nameof(Strings.FixedAsset))] public Guid? FixedAsset { get; set; }
            [ProtoMember(15), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForIntangibleAssets)), Autocomplete(typeof(IntangibleAsset), Filter = nameof(Account)), Prepend(nameof(Strings.IntangibleAsset))] public Guid? IntangibleAsset { get; set; }

            [Guide("Enter the description of the line. This column is visible only if the `Column-Description` option is checked.")]
            [ProtoMember(17), IfTrue(nameof(HasLineDescription)), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [ProtoMember(25)] public Dictionary<Guid, string> CustomFields { get; set; }
            [ProtoMember(27)] public CustomFields CustomFields2 { get; set; }

            [ProtoMember(18), AppendValue(nameof(Item), nameof(ManagerServer.Model.InventoryItem.UnitName)), Short] public decimal? Qty { get; set; }
            [ProtoMember(19), NoPlaceholder, AppendCurrency(nameof(Supplier)), Label(nameof(Strings.UnitPrice))] public decimal PurchaseUnitPrice { get; set; }
            [ProtoMember(20), IfDifferentCurrency, NoPlaceholder] public decimal CurrencyAmount { get; set; }
            [ProtoMember(23), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.Percentage), Short, Append("%")] public decimal DiscountPercentage { get; set; }
            [ProtoMember(24), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.ExactAmount)] public decimal DiscountAmount { get; set; }
            [Label(nameof(Strings.Total)), Expression(Zero, Plus, nameof(PurchaseUnitPrice), Times, nameof(Qty), Round, Minus, nameof(DiscountAmount), TimesPercentage, nameof(DiscountPercentage), Round), Sum, AppendCurrency(nameof(Supplier))] public object TotalBeforeTax { get; }
            [ProtoMember(21), Autocomplete(typeof(TaxCode)), Short] public Guid? TaxCode { get; set; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), TimesTaxCode, nameof(TaxCode)), Sum, IfFalse(nameof(AmountsIncludeTax))] public object TaxAmount { get; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), Plus, nameof(TaxAmount), Round), Sum, AppendCurrency(nameof(Supplier)), IfFalse(nameof(SalesOrder.AmountsIncludeTax))] public object Total { get; }
            [ProtoMember(26), Autocomplete(typeof(Project)), IfTrue(nameof(Account), nameof(NamedObject.ProjectEnabled)), Short] public Guid? Project { get; set; }
            [ProtoMember(22), Autocomplete(typeof(Division)), IfTrue(nameof(Account), nameof(NamedObject.DivisionEnabled)), Short] public Guid? Division { get; set; }
            //[ProtoMember(28), IfTrue(nameof(HasQtyReceived)), AppendValue(nameof(Item), nameof(Manager.Model.InventoryItem.UnitName)), Placeholder("0"), Short] public decimal QtyReceived;
            //[IfTrue(nameof(HasQtyReceived)), AppendValue(nameof(Item), nameof(Manager.Model.InventoryItem.UnitName)), Short, Expression(Zero, Plus, nameof(Qty), Minus, nameof(QtyReceived))] public object QtyToReceive;

            public override Guid? GetItem() => Item;
            public override Guid? GetAccount() => Account;
            public override Guid? GetBillableExpenseCustomer() => BillableExpenseCustomer;
            public override Guid? GetBillableExpenseSalesInvoice() => BillableExpenseSalesInvoice;
            public override Guid? GetEmployee() => Employee;
            public override Guid? GetFixedAsset() => FixedAsset;
            public override Guid? GetIntangibleAsset() => IntangibleAsset;
            public override Guid? GetCapitalAccount() => CapitalAccount;
            public override Guid? GetSpecialAccount() => SpecialAccount;
            public override Guid? GetSubAccount() => SubAccount;
            protected override decimal? GetUnitPrice() => PurchaseUnitPrice;
            protected override decimal? GetQty() => Qty;
            protected override decimal? GetDiscountPercentage() => DiscountPercentage;
            protected override decimal? GetDiscountAmount() => DiscountAmount;
            protected override string GetLineDescription() => LineDescription;
            public override Guid? GetTaxCode() => TaxCode;
            public override Guid? GetDivision() => Division;
            public override Dictionary<Guid, string> GetCustomFields() => CustomFields;
            public override CustomFields GetCustomFields2() => CustomFields2;
            public override decimal? GetProposedAccountAmount() => CurrencyAmount;
            protected override Guid? GetProject() => Project;
        }

        [ProtoContract]
        [Guid("cb826f68-8278-4ff1-a9ee-b8ecc5e6dd57")]
        public sealed class LandedCostLine : ITransactionLine
        {
            [ProtoMember(1), IfTrue(nameof(HasLineDescription)), Prepend(nameof(Strings.Description))] public string LandedCostDescription { get; set; }
            [ProtoMember(2), Prepend(nameof(Strings.Amount)), AppendCurrency(nameof(Supplier))] public decimal LandedCostAmount { get; set; }
            [ProtoMember(3), Autocomplete(typeof(TaxCode)), Short] public Guid? LandedCostTaxCode { get; set; }

            [ProtoMember(4)] public Line Obsolete_Line { get; set; }
        }

        [ProtoMember(18)] public DueDateType2 Obsolete_DueDate { get; set; }
        [ProtoMember(6)] public string Obsolete_Notes { get; set; }
        [ProtoMember(4)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines { get; set; }
        [ProtoMember(21)] public bool Obsolete_PartialPayment { get; set; }
        [ProtoMember(11)] public decimal Obsolete_ConversionBalance { get; set; }

        public DateTime GetDueDate()
        {
            try
            {
                var dueDate = IssueDate;
                if (DueDate == Enums.DueDateType.By && DueDateDate.HasValue) dueDate = DueDateDate.Value;
                if (DueDate == Enums.DueDateType.Net && DueDateDays.HasValue) dueDate = IssueDate.AddDays(DueDateDays.Value);
                if (dueDate > IssueDate) return dueDate;               
            }
            catch { }
            return IssueDate;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (filter is ManagerServer.Model.Supplier && Supplier != filter.Key) return false;
            if (ClosedInvoice) return false;
            return true;
        }

        public override string GetDescriptionOrNull()
        {
            if (!string.IsNullOrWhiteSpace(Description)) return Description;
            return null;
        }

        public override string GetName()
        {
            return (!string.IsNullOrWhiteSpace(Reference) ? Reference + " — " : null) + IssueDate.ToShortDateString();
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }

        public override ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            var supplier = database.SingleOrDefault<Supplier>(Supplier);
            var inventoryLocation = AlsoActsAsGoodsReceipt ? database.SingleOrDefault<CustomInventoryLocation>(PurchaseInventoryLocation) : null;

            var baseCurrency = database.Single<BaseCurrency>();
            var transactionCurrency = database.SingleOrDefault<ForeignCurrency>(supplier?.Currency) as Currency ?? baseCurrency;
            var purchaseOrder = database.SingleOrDefault<PurchaseOrder>(PurchaseOrder);

            var list = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();

            if (Lines != null)
            {
                for (int i = 0; i < Lines.Length; i++)
                {
                    list.AddRange(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction.From(
                        database: database,
                        date: IssueDate,
                        transaction: this,
                        transactionCurrency: transactionCurrency,
                        transactionLine: Lines[i],
                        exchangeRate: ExchangeRate,
                        exchangeRateIsInverse: ExchangeRateIsInverse,
                        amountsIncludeTax: AmountsIncludeTax,
                        supplier: supplier,
                        inventoryLocation: inventoryLocation,
                        purchaseInvoice: this,
                        purchaseOrder: purchaseOrder,
                        lineNumber: i
                    ));
                }
            }

            /*
            foreach (var e in list.Where(x => x.TransactionLine?.GetItem() == new Guid("3458c24f-2a5f-4dcf-9de7-7340b1463d9c") && !x.IsTaxTransaction && x.TransactionAmount > 0m).ToArray())
            {
                var transactionAmount = e.TransactionAmount;
                var inventoryItems = list.Where(x => x.InventoryItem != null && x.TransactionAmount > 0m && !x.IsTaxTransaction).ToArray();
                if (inventoryItems.Any())
                {
                    var inventoryItemsTotal = inventoryItems.Sum(x => x.TransactionAmount);
                    foreach (var line in inventoryItems)
                    {
                        var amount = 0m;
                        if (line.TransactionAmount == inventoryItemsTotal)
                        {
                            amount = transactionAmount;
                        }
                        else
                        {
                            amount = transactionCurrency.Round(line.TransactionAmount * (transactionAmount / inventoryItemsTotal));
                        }
                        transactionAmount -= amount;
                        inventoryItemsTotal -= line.TransactionAmount;

                        var baseAmount = baseCurrency.GetBaseAmount(amount, ExchangeRate, ExchangeRateIsInverse, transactionCurrency);

                        list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            date: IssueDate,
                            transaction: this,
                            supplier: supplier,
                            purchaseInvoice: this,
                            generalLedgerAccount: line.GeneralLedgerAccount,
                            inventoryItem: line.InventoryItem,
                            transactionAmount: amount,
                            transactionCurrency: transactionCurrency,
                            baseAmount: baseAmount,
                            exchangeRate: ExchangeRate,
                            isExchangeRateInverse: ExchangeRateIsInverse,
                            taxCode: e.TaxCode,
                            trackingCode: e.Division,
                            transactionLine: line.TransactionLine,
                            isLandingCost: true,
                            reportingCategory: line.ReportingCategory
                        ));
                    }

                    list.Remove(e);
                }
            }
            */

            if (FreightIn && LandedCostLines != null)
            {
                var inventoryItems = list.Where(x => x.InventoryItem != null && x.TransactionAmount > 0m && !x.IsTaxTransaction).ToArray();
                if (inventoryItems.Any())
                {
                    foreach (var e in LandedCostLines)
                    {
                        var freightInAmount = e.LandedCostAmount;
                        if (freightInAmount > 0m)
                        {
                            var inventoryItemsTotal = inventoryItems.Sum(x => x.TransactionAmount);

                            if (inventoryItemsTotal != 0m)
                            {
                                foreach (var line in inventoryItems)
                                {
                                    var amount = 0m;
                                    if (line.TransactionAmount == inventoryItemsTotal)
                                    {
                                        amount = freightInAmount;
                                    }
                                    else
                                    {
                                        amount = transactionCurrency.Round(line.TransactionAmount * (freightInAmount / inventoryItemsTotal));
                                    }
                                    freightInAmount -= amount;
                                    inventoryItemsTotal -= line.TransactionAmount;

                                    var line2 = ProtoBuf.Serializer.DeepClone<Line>((Line)line.TransactionLine);
                                    line2.Qty = null;
                                    line2.LineDescription = e.LandedCostDescription;
                                    line2.PurchaseUnitPrice = amount;
                                    line2.TaxCode = e.LandedCostTaxCode;
                                    line2.DiscountAmount = 0m;
                                    line2.DiscountPercentage = 0m;

                                    list.AddRange(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction.From(
                                        database: database,
                                        date: IssueDate,
                                        transaction: this,
                                        transactionCurrency: transactionCurrency,
                                        transactionLine: line2,
                                        exchangeRate: ExchangeRate,
                                        exchangeRateIsInverse: ExchangeRateIsInverse,
                                        amountsIncludeTax: AmountsIncludeTax,
                                        supplier: supplier,
                                        inventoryLocation: inventoryLocation,
                                        purchaseInvoice: this,
                                        purchaseOrder: purchaseOrder
                                    ));
                                }
                            }
                        }
                    }
                }
            }

            var total = list.Sum(x => x.TransactionAmount);

            var contraTransactions = list.ToArray();
           
            list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                transaction: this,
                date: IssueDate,
                generalLedgerAccount: database.Single<BalanceSheetAccountsPayableAccount>(),
                supplier: supplier,
                purchaseInvoice: this,
                baseAmount: list.Sum(x => x.BaseAmount) * -1m,
                transactionAmount: total * -1m,
                transactionCurrency: transactionCurrency,
                isBalancing: true,
                contraTransactions: contraTransactions,
                purchaseOrder: database.SingleOrDefault<PurchaseOrder>(PurchaseOrder),
                trackingCode: database.SingleOrDefault<Division>(supplier?.Division)
            ));

            if (WithholdingTax)
            {
                var withholdingTax = database.Single<ManagerServer.Model.WithholdingTax>();
                if (withholdingTax.WithholdingTaxPayable)
                {
                    var withholdingTaxAmount = 0m;
                    if (WithholdingTaxType == WithholdingTaxType.Rate && WithholdingTaxPercentage > 0m && WithholdingTaxPercentage <= 100m)
                    {
                        withholdingTaxAmount = transactionCurrency.Round(total / 100m * WithholdingTaxPercentage);
                    }
                    else if (WithholdingTaxType == WithholdingTaxType.Amount && WithholdingTaxAmount > 0m)
                    {
                        withholdingTaxAmount = transactionCurrency.Round(WithholdingTaxAmount);
                    }

                    if (withholdingTaxAmount != 0m)
                    {
                        var baseWithholdingTaxAmount = baseCurrency.GetBaseAmount(withholdingTaxAmount, ExchangeRate, ExchangeRateIsInverse, transactionCurrency);

                        list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            transaction: this,
                            date: IssueDate,
                            generalLedgerAccount: database.Single<BalanceSheetWithholdingTaxPayableAccount>(),
                            supplier: supplier,
                            transactionAmount: withholdingTaxAmount * -1m,
                            baseAmount: baseWithholdingTaxAmount * -1m,
                            exchangeRate: ExchangeRate,
                            isExchangeRateInverse: ExchangeRateIsInverse,
                            purchaseInvoice: this,
                            transactionCurrency: transactionCurrency,
                            purchaseOrder: database.SingleOrDefault<PurchaseOrder>(PurchaseOrder)
                        ));

                        list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            transaction: this,
                            date: IssueDate,
                            generalLedgerAccount: database.Single<BalanceSheetAccountsPayableAccount>(),
                            supplier: supplier,
                            transactionAmount: withholdingTaxAmount,
                            baseAmount: baseWithholdingTaxAmount,
                            exchangeRate: ExchangeRate,
                            isExchangeRateInverse: ExchangeRateIsInverse,
                            purchaseInvoice: this,
                            transactionCurrency: transactionCurrency,
                            purchaseOrder: database.SingleOrDefault<PurchaseOrder>(PurchaseOrder)
                        ));
                    }
                }
            }

            return list.ToArray();
        }

        int IComparable<PurchaseInvoice>.CompareTo(PurchaseInvoice other)
        {
            return (!other.IsInactive(), other.IssueDate, other.Reference).CompareTo((!IsInactive(), IssueDate, Reference));
        }
    }
}
