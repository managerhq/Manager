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

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("ad12b60b-23bf-4421-94df-8be79cef533e")]
    [Currency(nameof(Customer))]
    public sealed class SalesInvoice : Transaction, IHasAutomaticReference, IRecurringTransactionDestination, ICustomFields, IComparable<SalesInvoice>, IForeignCurrencyTransaction, ICode, IHasCustomTheme
    {
        [Guide("Enter the date when this invoice is issued to the customer.")]
        [Guide("The issue date determines when revenue is recognized and affects your sales reports.")]
        [Guide("This date is also used to calculate payment due dates based on your payment terms.")]
        [ProtoMember(1), NoWrap] public DateTime IssueDate { get; set; }
        [Guide("Select payment terms to determine when this invoice is due for payment.")]
        [Guide("Choose `Immediate` for cash sales or immediate payment requirements.")]
        [Guide("Choose `Net` to specify a number of days from the issue date.")]
        [Guide("Choose `By` to set a specific calendar date for payment.")]
        [ProtoMember(54), NoWrap] public DueDateType DueDate { get; set; }
        [Guide("Enter the number of days from issue date when payment is due.")]
        [Guide("Common terms include Net 30 (30 days), Net 60 (60 days), or Net 90 (90 days).")]
        [Guide("The system will calculate the exact due date by adding these days to the issue date.")]
        [ProtoMember(22), NoWrap, EmptyLabel, IfEnum(nameof(DueDate), (int)DueDateType.Net), Append(nameof(Strings.Days))] public int? DueDateDays { get; set; }
        [Guide("Enter the specific date when payment for this invoice is due.")]
        [Guide("Use this for invoices with fixed payment dates, such as month-end or specific contract dates.")]
        [Guide("The due date cannot be earlier than the issue date.")]
        [ProtoMember(6), NoWrap, EmptyLabel, IfEnum(nameof(DueDate), (int)DueDateType.By), DoNotCopy] public DateTime? DueDateDate { get; set; }
        [Guide("Enter a unique reference number for this sales invoice.")]
        [Guide("Reference numbers help customers identify invoices and are used for payment matching.")]
        [Guide("Enable automatic numbering to generate sequential invoice numbers automatically.")]
        [Guide("Configure default settings and number sequences under `Settings` → `FormDefaults`.")]
        [ProtoMember(2), NoWrap] public string Reference { get; set; }
        [ProtoMember(15), NoWrap, Short, IfNull(nameof(SalesQuote)), Placeholder(nameof(Strings.Optional)), IfNotEmpty] public string QuoteNumber { get; set; }
        [ProtoMember(11), Short, IfNull(nameof(SalesOrder)), Placeholder(nameof(Strings.Optional)), IfNotEmpty] public string OrderNumber { get; set; }
        [Guide("Select the `Customer` who will receive this invoice.")]
        [Guide("The customer selection determines billing details, payment terms, and applicable pricing.")]
        [Guide("Create new customers under the `Customers` tab before creating invoices.")]
        [Guide("Customer currency settings will determine if this is a foreign currency invoice.")]
        [ProtoMember(3), NoWrap, Autocomplete(typeof(Customer)), OnChangeSetDefault(nameof(DueDateDays)), OnChangeSetDefault(nameof(BillingAddress)), OnChangeSetNull(nameof(SalesOrder)), OnChangeSetNull(nameof(SalesQuote))] public Guid? Customer { get; set; }
        [Guide("Link this invoice to a `SalesQuote` if it originated from a quote.")]
        [Guide("Linking helps track quote-to-invoice conversion rates and maintains transaction history.")]
        [Guide("The linked sales quote will automatically update to 'Accepted' status.")]
        [Guide("Quote details can be copied to the invoice to save data entry time.")]
        [ProtoMember(51), NoWrap, IfNotNull(nameof(Customer)), Short, Autocomplete(typeof(SalesQuote), Filter = nameof(Customer)), Placeholder(nameof(Strings.Optional)), EmptyLabel, Prepend(nameof(Strings.QuoteNumber))] public Guid? SalesQuote { get; set; }
        [Guide("Link this invoice to a `SalesOrder` if fulfilling an order.")]
        [Guide("Linking ensures all sales orders are properly invoiced and tracks order fulfillment.")]
        [Guide("Order details and items can be copied to the invoice automatically.")]
        [Guide("The system tracks which orders have been partially or fully invoiced.")]
        [ProtoMember(50), IfNotNull(nameof(Customer)), Short, Autocomplete(typeof(SalesOrder), Filter = nameof(Customer)), Placeholder(nameof(Strings.Optional)), EmptyLabel, Prepend(nameof(Strings.OrderNumber))] public Guid? SalesOrder { get; set; }
        [Guide("Enter the customer's billing address for this invoice.")]
        [Guide("The billing address is automatically populated from the customer record but can be modified.")]
        [Guide("This address appears on the invoice and should match where payment notices are sent.")]
        [Guide("Use a complete address including country for international customers.")]
        [ProtoMember(4), Textarea, Short] public string BillingAddress { get; set; }
        [Guide("Enter the `ExchangeRate` when invoicing customers in foreign currency.")]
        [Guide("This field appears when the selected customer uses a currency different from your base currency.")]
        [Guide("The exchange rate converts foreign currency amounts to base currency for reporting.")]
        [Guide("Configure automatic exchange rates under `Settings` → `ExchangeRates`.")]
        [ProtoMember(64), Placeholder(nameof(Strings.Autofill)), NoWrap, IfNotNull(nameof(Customer), nameof(Model.Customer.Currency)), Prepend("1 {{ (ExchangeRateIsInverse ? baseCurrency.code : getCurrencyCode()) }} = "), Append("{{ (ExchangeRateIsInverse ? getCurrencyCode() : baseCurrency.code) }}")] public decimal ExchangeRate { get; set; }
        [ProtoMember(65), IfNotNull(nameof(Customer), nameof(Model.Customer.Currency)), Icon("fa-right-left")] public bool ExchangeRateIsInverse { get; set; }
        [Guide("Enter an optional description that applies to the entire invoice.")]
        [Guide("Use this for general invoice notes, project references, or delivery instructions.")]
        [Guide("This description appears at the top of the invoice, separate from line item details.")]
        [ProtoMember(12), Long, Placeholder(nameof(Strings.Optional)), Typeahead] public string Description { get; set; }
        [Guide("Add line items to detail what you are charging the customer for.")]
        [Guide("Each line can be an inventory item, service, or other billable item.")]
        [Guide("Use multiple lines to itemize different products, services, or charge categories.")]
        [Guide("Line totals are automatically calculated based on quantity, price, discounts, and tax.")]
        [ProtoMember(49)] public Line[] Lines { get; set; }
        [Guide("Enable line numbers to display sequential numbering for each invoice line.")]
        [Guide("Line numbers help customers reference specific items when making inquiries.")]
        [Guide("Useful for invoices with many line items or when matching to purchase orders.")]
        [ProtoMember(59), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [Guide("Enable the `Description` column to add detailed explanations for each line item.")]
        [Guide("Descriptions provide additional context beyond the item name.")]
        [Guide("Essential for services or custom work where details vary per invoice.")]
        [ProtoMember(48), Label(nameof(Strings.Column), nameof(Strings.Description))] public bool HasLineDescription { get; set; }
        [Guide("Enable the `Discount` column to apply discounts to individual line items.")]
        [Guide("Choose between percentage discounts or fixed amount discounts per line.")]
        [Guide("Line discounts are applied before tax calculations.")]
        [Guide("Useful for promotional pricing, volume discounts, or customer-specific pricing.")]
        [ProtoMember(31), Label(nameof(Strings.Column), nameof(Strings.Discount))] public bool Discount { get; set; }
        [ProtoMember(32), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        //[ProtoMember(68), Label(nameof(Strings.Column), nameof(Strings.QtyDelivered))] public bool HasQtyDelivered;
        [Guide("Specify whether line item amounts include or exclude tax.")]
        [Guide("Check this box if prices already include tax - common in retail sales.")]
        [Guide("Leave unchecked if tax should be added to prices - common in business-to-business sales.")]
        [Guide("This setting affects how the invoice total is calculated and displayed.")]
        [ProtoMember(8), IfContains<TaxCode>] public bool AmountsIncludeTax { get; set; }
        [Guide("Enable rounding to adjust the final invoice total to a round number.")]
        [Guide("Rounding eliminates small cent amounts for easier payment processing.")]
        [Guide("Choose the rounding method that complies with your local regulations.")]
        [Guide("The rounding difference is typically posted to a rounding expense or income account.")]
        [ProtoMember(29)] public bool Rounding { get; set; }
        [ProtoMember(20), IfTrue(nameof(Rounding)), NoLabel] public RoundingMethod RoundingMethod { get; set; }
        [ProtoMember(28), IfWithholdingTaxReceivable] public bool WithholdingTax { get; set; }
        [ProtoMember(26), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel] public WithholdingTaxType WithholdingTaxType { get; set; }
        [ProtoMember(25), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel, Append("%"), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Rate)] public decimal WithholdingTaxPercentage { get; set; }
        [ProtoMember(27), IfTrue(nameof(WithholdingTax)), NoLabel, AppendCurrency(nameof(Customer)), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Amount)] public decimal WithholdingTaxAmount { get; set; }
        [ProtoMember(35)] public bool EarlyPaymentDiscount { get; set; }
        [ProtoMember(36), IfTrue(nameof(EarlyPaymentDiscount)), NoWrap, NoLabel] public DiscountType EarlyPaymentDiscountType { get; set; }
        [ProtoMember(37), IfTrue(nameof(EarlyPaymentDiscount)), NoWrap, NoLabel, IfEnum(nameof(EarlyPaymentDiscountType), (int)DiscountType.Percentage), Append("%")] public decimal EarlyPaymentDiscountRate { get; set; }
        [ProtoMember(38), IfTrue(nameof(EarlyPaymentDiscount)), NoWrap, NoLabel, IfEnum(nameof(EarlyPaymentDiscountType), (int)DiscountType.ExactAmount), AppendCurrency(nameof(Customer))] public decimal EarlyPaymentDiscountAmount { get; set; }
        [ProtoMember(39), IfTrue(nameof(EarlyPaymentDiscount)), NoLabel, Prepend(nameof(Strings.If_paid_within)), Append(nameof(Strings.Days))] public int? EarlyPaymentDiscountDays { get; set; }
        [ProtoMember(41)] public bool LatePaymentFees { get; set; }
        [ProtoMember(24), IfTrue(nameof(LatePaymentFees)), NoLabel, Prepend(nameof(Strings.ChargeMonthly)), Append("%")] public decimal LatePaymentFeesPercentage { get; set; }
        [ProtoMember(46)] public bool TotalAmountInBaseCurrency { get; set; }
        [ProtoMember(52), IfNotEnglish] public bool Bilingual { get; set; }
        [ProtoMember(42), Label(nameof(Strings.CustomTitle))] public bool HasSalesInvoiceCustomTitle { get; set; }
        [ProtoMember(40), IfTrue(nameof(HasSalesInvoiceCustomTitle)), NoLabel, Placeholder(nameof(Strings.Invoice))] public string SalesInvoiceCustomTitle { get; set; }
        [ProtoMember(33), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(34), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
        [ProtoMember(43), DoNotCopy] public bool AutomaticReference { get; set; }
        [ProtoMember(45), Label(nameof(Strings.Hide), nameof(Strings.DueDate))] public bool HideDueDate { get; set; }
        [ProtoMember(56), Label(nameof(Strings.Hide), nameof(Strings.BalanceDue))] public bool HideBalanceDue { get; set; }
        [ProtoMember(53), IfExists, DoNotCopy] public bool ClosedInvoice { get; set; }
        [ProtoMember(63)] public bool ShowItemImages { get; set; }
        [ProtoMember(57), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }
        [ProtoMember(69)] public bool AlsoActsAsDeliveryNote { get; set; }
        [ProtoMember(30), IfTrue(nameof(AlsoActsAsDeliveryNote)), NoLabel, Prepend(nameof(Strings.InventoryLocation)), Autocomplete(typeof(CustomInventoryLocation))] public Guid? SalesInventoryLocation { get; set; }
        [ProtoMember(60), Label(nameof(Strings.Footers))] public bool HasSalesInvoiceFooters { get; set; }
        [ProtoMember(61), Autocomplete(typeof(ManagerServer.Model.SalesInvoiceFooter)), NoLabel, IfTrue(nameof(HasSalesInvoiceFooters))] public Guid[] SalesInvoiceFooters { get; set; }
        [Guide("Add business-specific information using `CustomFields`.")]
        [Guide("Custom fields can track PO numbers, project codes, delivery dates, or any data unique to your business.")]
        [Guide("Set up custom fields under `Settings` → `CustomFields` before using them in invoices.")]
        [ProtoMember(13)] public Dictionary<Guid, string> CustomFields { get; set; }
        [Guide("Use enhanced `CustomFields` for advanced data types and validation.")]
        [Guide("Enhanced fields support dates, numbers, dropdown lists, and other structured data types.")]
        [Guide("Configure validation rules and default values under `Settings` → `CustomFields`.")]
        [ProtoMember(62)] public CustomFields CustomFields2 { get; set; }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        DateTime IRecurringTransactionDestination.Date { get => IssueDate; set => IssueDate = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        DateTime IForeignCurrencyTransaction.Date => IssueDate;
        Guid? IForeignCurrencyTransaction.Currency => Customer;
        decimal IForeignCurrencyTransaction.ExchangeRate { get => ExchangeRate; set => ExchangeRate = value; }
        bool IForeignCurrencyTransaction.ExchangeRateIsInverse { get => ExchangeRateIsInverse; set => ExchangeRateIsInverse = value; }
        string ICode.Code => Reference;

        public override string GetReference() => Reference;

        public override bool GetHasLineDescription() => HasLineDescription;
        public override bool HasLineQty() => true;
        public override bool HasLineUnitPrice() => true;
        public override DiscountType? GetLineDicountType() => Discount ? DiscountType : null;

        public override bool IsInactive()
        {
            return ClosedInvoice;
        }

        [CustomFields]
        [ProtoContract]
        [Guid("c37e4421-591a-4440-90ef-c5c9421e7948")]
        public sealed class Line : ITransactionLine
        {
            [IfTrue(nameof(HasLineNumber)), LineNumber, Label("#")] public object LineNumber { get; }
            [ProtoMember(1), Autocomplete(typeof(ISaleItem)), OnChangeSetDefault(nameof(Qty)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(SalesUnitPrice)), OnChangeSetDefault(nameof(TaxCode)), OnChangeSetDefault(nameof(Division)), Short] public Guid? Item { get; set; }
            [ProtoMember(2), Autocomplete(typeof(ISalesInvoiceAccount), Subtext = nameof(BalanceSheetAccount.Group)), Substitute(nameof(Item), nameof(ISaleItem.SaleItemAccount)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(TaxCode))] public Guid? Account { get; set; }
            [ProtoMember(9), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), Autocomplete(typeof(CapitalAccount), Filter = nameof(Account)), Prepend(nameof(Strings.CapitalAccount))] public Guid? CapitalAccount { get; set; }
            [ProtoMember(10), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), IfNotNull(nameof(CapitalAccount)), Autocomplete(typeof(SubAccount)), Prepend(nameof(Strings.SubAccount))] public Guid? SubAccount { get; set; }
            [ProtoMember(13), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForSpecialAccounts)), Autocomplete(typeof(SpecialAccount), Filter = nameof(Account)), Prepend(nameof(Strings.SpecialAccount))] public Guid? SpecialAccount { get; set; }
            [ProtoMember(14), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForFixedAssets)), Autocomplete(typeof(FixedAsset), Filter = nameof(Account)), Prepend(nameof(Strings.FixedAsset))] public Guid? FixedAsset { get; set; }
            [ProtoMember(15), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForIntangibleAssets)), Autocomplete(typeof(IntangibleAsset), Filter = nameof(Account)), Prepend(nameof(Strings.IntangibleAsset))] public Guid? IntangibleAsset { get; set; }
            [ProtoMember(17), IfTrue(nameof(HasLineDescription)), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [Guide("Add line-specific custom information using `CustomFields`.")]
            [Guide("Line custom fields can track serial numbers, batch codes, or other line-level data.")]
            [Guide("Configure line custom fields under `Settings` → `CustomFields` for invoice lines.")]
            [ProtoMember(25)] public Dictionary<Guid, string> CustomFields { get; set; }
            [Guide("Use enhanced `CustomFields` for line-specific structured data.")]
            [Guide("Supports dates, numbers, dropdown selections, and other data types at the line level.")]
            [Guide("Useful for detailed tracking requirements that vary by invoice line.")]
            [ProtoMember(27)] public CustomFields CustomFields2 { get; set; }
            [ProtoMember(18), AppendValue(nameof(Item), nameof(ManagerServer.Model.InventoryItem.UnitName)), Short] public decimal? Qty { get; set; }
            [ProtoMember(19), NoPlaceholder, AppendCurrency(nameof(Customer)), Label(nameof(Strings.UnitPrice))] public decimal SalesUnitPrice { get; set; }
            [ProtoMember(20), IfDifferentCurrency, NoPlaceholder] public decimal CurrencyAmount { get; set; }
            [ProtoMember(23), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.Percentage), Short, Append("%")] public decimal DiscountPercentage { get; set; }
            [ProtoMember(24), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.ExactAmount)] public decimal DiscountAmount { get; set; }
            [Label(nameof(Strings.Total)), Expression(Zero, Plus, nameof(SalesUnitPrice), Times, nameof(Qty), Round, Minus, nameof(DiscountAmount), TimesPercentage, nameof(DiscountPercentage), Round), Sum, AppendCurrency(nameof(Customer))] public object TotalBeforeTax { get; }
            [ProtoMember(21), Autocomplete(typeof(TaxCode)), Short] public Guid? TaxCode { get; set; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), TimesTaxCode, nameof(TaxCode)), Sum, IfFalse(nameof(AmountsIncludeTax))] public object TaxAmount { get; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), Plus, nameof(TaxAmount), Round), Sum, AppendCurrency(nameof(Customer)), IfFalse(nameof(ManagerServer.Model.SalesOrder.AmountsIncludeTax))] public object Total { get; }
            //[ProtoMember(28), IfTrue(nameof(Item), nameof(ISaleItem.HasCostOfGoodsSold)), Short] public AutomaticManual CostOfGoodsSold;
            //[ProtoMember(29), IfEnum(nameof(CostOfGoodsSold), 1), AppendBaseCurrency, EmptyLabel] public decimal CostOfGoodsSoldAmount;
            [ProtoMember(26), Autocomplete(typeof(Project)), IfTrue(nameof(Account), nameof(NamedObject.ProjectEnabled)), Short] public Guid? Project { get; set; }
            [ProtoMember(22), Autocomplete(typeof(Division)), IfTrue(nameof(Account), nameof(NamedObject.DivisionEnabled)), Short] public Guid? Division { get; set; }
            //[ProtoMember(30), IfTrue(nameof(HasQtyDelivered)), AppendValue(nameof(Item), nameof(Manager.Model.InventoryItem.UnitName)), Placeholder("0"), Short] public decimal QtyDelivered;
            //[IfTrue(nameof(HasQtyDelivered)), AppendValue(nameof(Item), nameof(Manager.Model.InventoryItem.UnitName)), Short, Expression(Zero, Plus, nameof(Qty), Minus, nameof(QtyDelivered))] public object QtyToDeliver;

            public override Guid? GetItem() => Item;
            public override Guid? GetAccount() => Account;
            public override Guid? GetFixedAsset() => FixedAsset;
            public override Guid? GetIntangibleAsset() => IntangibleAsset;
            public override Guid? GetCapitalAccount() => CapitalAccount;
            public override Guid? GetSpecialAccount() => SpecialAccount;
            public override Guid? GetSubAccount() => SubAccount;
            protected override decimal? GetUnitPrice() => SalesUnitPrice;
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
        }
        
        [ProtoMember(58)] public bool Obsolete_OldLayout { get; set; }
        [ProtoMember(55)] public bool Obsolete_NewLayout { get; set; }
        [ProtoMember(21)] public DueDateType2 Obsolete_DueDate { get; set; }
        [ProtoMember(5)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines { get; set; }
        [ProtoMember(9)] public bool Obsolete_IsCashSale { get; set; }
        [ProtoMember(10)] public Guid? Obsolete_CashSaleDebitAccount { get; set; }
        [ProtoMember(7)] public string Obsolete_InternalNotes { get; set; }
        [ProtoMember(19)] public bool Obsolete_TotalRounded { get; set; }
        [ProtoMember(18)] public Guid? Obsolete_HtmlTheme { get; set; }
        [ProtoMember(14)] public string Obsolete_Notes { get; set; }
        [ProtoMember(23)] public LatePaymentFeesType Obsolete_LatePaymentFees { get; set; }
        [ProtoMember(47)] public bool Obsolete_PartialPayment { get; set; }
        [ProtoMember(17)] public decimal Obsolete_ConversionBalance { get; set; }
        [ProtoMember(66)] public bool Obsolete_HasRelay { get; set; }
        [ProtoMember(67)] public string Obsolete_Relay { get; set; }

        public override bool OnAutocomplete(Object filter)
        {
            if (filter is ManagerServer.Model.Customer && Customer != filter.Key) return false;
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
            return (!string.IsNullOrWhiteSpace(Reference) ? Reference+ " — " : null)+IssueDate.ToShortDateString();
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }

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

        public override ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            if (Lines == null) return null;

            var customer = database.SingleOrDefault<Customer>(Customer);
            var inventoryLocation = AlsoActsAsDeliveryNote ? database.SingleOrDefault<CustomInventoryLocation>(SalesInventoryLocation) : null;

            var baseCurrency = database.Single<BaseCurrency>();
            var transactionCurrency = database.SingleOrDefault<ForeignCurrency>(customer?.Currency) as Currency ?? baseCurrency;
            var salesOrder = database.SingleOrDefault<SalesOrder>(SalesOrder);

            var list = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();

            foreach (var line in Lines)
            {
                list.AddRange(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction.From(
                    database: database,
                    date: IssueDate,
                    transaction: this,
                    transactionCurrency: transactionCurrency,
                    transactionLine: line,
                    exchangeRate: ExchangeRate,
                    exchangeRateIsInverse: ExchangeRateIsInverse,
                    amountsIncludeTax: AmountsIncludeTax,
                    customer: customer,
                    inventoryLocation: inventoryLocation,
                    salesInvoice: this,
                    salesOrder: salesOrder,
                    reverseSign: true
                ));
            }

            var total = -list.Select(x => x.TransactionAmount).SafeSum();

            if (Rounding)
            {
                if (RoundingMethod == Enums.RoundingMethod.RoundDown)
                {
                    var rounding = total - Math.Floor(total);
                    if (rounding != 0m)
                    {
                        list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            transaction: this,
                            date: IssueDate,
                            generalLedgerAccount: database.Single<ProfitAndLossStatementAccountRoundingExpense>(),
                            customer: customer,
                            transactionAmount: rounding,
                            exchangeRate: ExchangeRate,
                            isExchangeRateInverse: ExchangeRateIsInverse,
                            salesInvoice: this,
                            transactionCurrency: transactionCurrency
                        ));
                    }
                }
                if (RoundingMethod == Enums.RoundingMethod.RoundToNearest)
                {
                    var rounding = total - Math.Round(total, 0, MidpointRounding.AwayFromZero);
                    if (rounding != 0m)
                    {
                        list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            transaction: this,
                            date: IssueDate,
                            generalLedgerAccount: database.Single<ProfitAndLossStatementAccountRoundingExpense>(),
                            customer: customer,
                            transactionAmount: rounding,
                            exchangeRate: ExchangeRate,
                            isExchangeRateInverse: ExchangeRateIsInverse,
                            salesInvoice: this,
                            transactionCurrency: transactionCurrency
                        ));
                    }
                }
            }

            total = -list.Sum(x => x.TransactionAmount);

            var contraTransactions = list.ToArray();

            list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                transaction: this,
                date: IssueDate,
                generalLedgerAccount: database.Single<BalanceSheetAccountsReceivableAccount>(),
                customer: customer,
                salesInvoice: this,
                baseAmount: -list.Select(x => x.BaseAmount).SafeSum(),
                transactionAmount: -list.Select(x => x.TransactionAmount).SafeSum(),
                transactionCurrency: transactionCurrency,
                isBalancing: true,
                contraTransactions: contraTransactions,
                salesOrder: database.SingleOrDefault<SalesOrder>(SalesOrder),
                trackingCode: database.SingleOrDefault<Division>(customer?.Division)
            ));

            if (WithholdingTax)
            {
                var withholdingTax = database.Single<ManagerServer.Model.WithholdingTax>();
                if (withholdingTax.WithholdingTaxReceivable)
                {
                    var withholdingTaxAmount = 0m;
                    if (WithholdingTaxType == WithholdingTaxType.Rate && WithholdingTaxPercentage > 0m && WithholdingTaxPercentage <= 100m)
                    {
                        withholdingTaxAmount = transactionCurrency.Round(total / 100m * WithholdingTaxPercentage);
                    }
                    if (WithholdingTaxType == WithholdingTaxType.Amount && WithholdingTaxAmount > 0m)
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
                            generalLedgerAccount: database.Single<BalanceSheetWithholdingTaxReceivableAccount>(),
                            customer: customer,
                            transactionAmount: withholdingTaxAmount,
                            baseAmount: baseWithholdingTaxAmount,
                            salesInvoice: this,
                            transactionCurrency: transactionCurrency,
                            exchangeRate: ExchangeRate,
                            isExchangeRateInverse: ExchangeRateIsInverse,
                            salesOrder: database.SingleOrDefault<SalesOrder>(SalesOrder)
                        ));

                        list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            transaction: this,
                            date: IssueDate,
                            generalLedgerAccount: database.Single<BalanceSheetAccountsReceivableAccount>(),
                            customer: customer,
                            transactionAmount: -withholdingTaxAmount,
                            baseAmount: -baseWithholdingTaxAmount,
                            salesInvoice: this,
                            transactionCurrency: transactionCurrency,
                            exchangeRate: ExchangeRate,
                            isExchangeRateInverse: ExchangeRateIsInverse,
                            salesOrder: database.SingleOrDefault<SalesOrder>(SalesOrder)
                        ));
                    }
                }
            }

            return list.ToArray();
        }

        int IComparable<SalesInvoice>.CompareTo(SalesInvoice other)
        {
            return (!other.IsInactive(), other.IssueDate, other.Reference).CompareTo((!IsInactive(), IssueDate, Reference));
        }
    }
}