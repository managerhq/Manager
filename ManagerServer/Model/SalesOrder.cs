using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Obsolete.Obsolete32;
using ManagerServer.Query.GeneralLedger;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using static ManagerServer.Model.Attributes.ExpressionAttribute.Operators;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("2dac5598-128d-4954-b2e4-21515047b3a7")]
    [Currency(nameof(Customer))]
    public sealed class SalesOrder : Transaction, IHasAutomaticReference, IRecurringTransactionDestination, ICustomFields, IComparable<SalesOrder>, ICode, IHasCustomTheme
    {
        [Guide("Enter the date of the sales order. This is typically when the customer placed the order.")]
        [ProtoMember(1), NoWrap] public DateTime Date { get; set; }
        [Guide("Enter a reference number for this sales order. This could be an order number, customer PO number, or your internal reference.")]
        [ProtoMember(2)] public string Reference { get; set; }
        [Guide("Select the customer who placed this order. Their billing address will automatically populate from the customer record.")]
        [ProtoMember(3), NoWrap, Autocomplete(typeof(Customer)), OnChangeSetDefault(nameof(BillingAddress))] public Guid? Customer { get; set; }
        [Guide("Optionally, link this sales order to a sales quote. This helps track quote-to-order conversion and automatically populates order details.")]
        [ProtoMember(19), IfNotNull(nameof(Customer)), Short, Autocomplete(typeof(SalesQuote), Filter = nameof(Customer)), Placeholder(nameof(Strings.Optional)), EmptyLabel, Prepend(nameof(Strings.QuoteNumber))] public Guid? SalesQuote { get; set; }
        [Guide("Enter the customer's billing address. This is automatically filled from the customer record but can be modified for this specific order.")]
        [ProtoMember(11), Textarea, Short] public string BillingAddress { get; set; }
        [Guide("Optionally, add a description or notes about this order, such as special requirements or delivery instructions.")]
        [ProtoMember(12), Long, Typeahead] public string Description { get; set; }
        [Guide("Enter the line items for this order. Each line represents a product or service with quantity, price, and other details.")]
        [ProtoMember(18)] public Line[] Lines { get; set; }
        [Guide("Check this box if the prices you enter already include tax. Leave unchecked if you want tax calculated on top of the entered prices.")]
        [ProtoMember(9), IfContains<TaxCode>] public bool AmountsIncludeTax { get; set; }
        [Guide("Check this box to display line numbers on the sales order. This helps reference specific items when discussing the order.")]
        [ProtoMember(27), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [Guide("Check this box to enable a discount column where you can apply line-item discounts.")]
        [ProtoMember(13), Label(nameof(Strings.Column), nameof(Strings.Discount))] public bool Discount { get; set; }
        [Guide("Select whether discounts are entered as percentages or fixed amounts.")]
        [ProtoMember(14), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        [Guide("Check this box if withholding tax applies to this order. This is typically required for certain types of transactions or customers.")]
        [ProtoMember(23)] public bool WithholdingTax { get; set; }
        [ProtoMember(24), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel] public WithholdingTaxType WithholdingTaxType { get; set; }
        [ProtoMember(25), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel, Append("%"), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Rate)] public decimal WithholdingTaxPercentage { get; set; }
        [ProtoMember(26), IfTrue(nameof(WithholdingTax)), NoLabel, AppendCurrency(nameof(Customer)), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Amount)] public decimal WithholdingTaxAmount { get; set; }
        [ProtoMember(15), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(16), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
        [ProtoMember(33)] public bool ShowItemImages { get; set; }
        [ProtoMember(22), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }
        [ProtoMember(31), Label(nameof(Strings.CustomTitle))] public bool HasSalesOrderCustomTitle { get; set; }
        [ProtoMember(32), IfTrue(nameof(HasSalesOrderCustomTitle)), Placeholder(nameof(Strings.SalesOrder)), NoLabel] public string SalesOrderCustomTitle { get; set; }
        [ProtoMember(28), Label(nameof(Strings.Footers))] public bool HasSalesOrderFooters { get; set; }
        [ProtoMember(29), Autocomplete(typeof(ManagerServer.Model.SalesOrderFooter)), NoLabel, IfTrue(nameof(HasSalesOrderFooters))] public Guid[] SalesOrderFooters { get; set; }
        [Guide("This indicates whether the sales order has been cancelled. Cancelled orders remain in the system for record-keeping but don't affect reports.")]
        [ProtoMember(21), DoNotCopy] public bool Cancelled { get; set; }
        [ProtoMember(17), DoNotCopy] public bool AutomaticReference { get; set; }
        [ProtoMember(10)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(30)] public CustomFields CustomFields2 { get; set; }

        public override string GetReference() => Reference;

        public override bool IsInactive()
        {
            return Cancelled;
        }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        DateTime IRecurringTransactionDestination.Date { get => Date; set => Date = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        string ICode.Code => Reference;

        public override bool HasLineQty() => true;
        public override bool HasLineUnitPrice() => true;
        public override DiscountType? GetLineDicountType() => Discount ? DiscountType : null;

        [CustomFields]
        [ProtoContract]
        [Guid("b8660bf5-d93b-4572-8edf-19bec47fb915")]
        public sealed class Line : ITransactionLine
        {
            [IfTrue(nameof(HasLineNumber)), LineNumber, Label("#")] public object LineNumber { get; }
            [ProtoMember(1), Short, Autocomplete(typeof(ISaleItem)), OnChangeSetDefault(nameof(Qty)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(SalesUnitPrice)), OnChangeSetDefault(nameof(TaxCode))] public Guid? Item { get; set; }
            [ProtoMember(2), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [ProtoMember(8)] public Dictionary<Guid, string> CustomFields { get; set; }
            [ProtoMember(9)] public CustomFields CustomFields2 { get; set; }
            [ProtoMember(3), AppendValue(nameof(Item), nameof(InventoryItem.UnitName)), Short] public decimal? Qty { get; set; }
            [ProtoMember(4), Label(nameof(Strings.UnitPrice))] public decimal SalesUnitPrice { get; set; }
            [ProtoMember(5), Label(nameof(Strings.Discount)), IfTrue(nameof(SalesOrder.Discount)), IfEnum(nameof(SalesOrder.DiscountType), (int)DiscountType.Percentage), Short, Append("%")] public decimal DiscountPercentage { get; set; }
            [ProtoMember(6), Label(nameof(Strings.Discount)), IfTrue(nameof(SalesOrder.Discount)), IfEnum(nameof(SalesOrder.DiscountType), (int)DiscountType.ExactAmount)] public decimal DiscountAmount { get; set; }
            [Label(nameof(Strings.Total)), Expression(Zero, Plus, nameof(SalesUnitPrice), Times, nameof(Qty), Round, Minus, nameof(DiscountAmount), TimesPercentage, nameof(DiscountPercentage), Round), Sum, AppendCurrency(nameof(SalesOrder.Customer))] public object TotalBeforeTax { get; }
            [ProtoMember(7), Autocomplete(typeof(TaxCode)), Short] public Guid? TaxCode { get; set; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), TimesTaxCode, nameof(TaxCode)), Sum, IfFalse(nameof(SalesOrder.AmountsIncludeTax))] public object TaxAmount { get; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), Plus, nameof(TaxAmount), Round), Sum, AppendCurrency(nameof(SalesOrder.Customer)), IfFalse(nameof(SalesOrder.AmountsIncludeTax))] public object Total { get; }

            public override Guid? GetItem() => Item;
            protected override string GetLineDescription() => LineDescription;
            protected override decimal? GetQty() => Qty;
            protected override decimal? GetUnitPrice() => SalesUnitPrice;
            protected override decimal? GetDiscountPercentage() => DiscountPercentage;
            protected override decimal? GetDiscountAmount() => DiscountAmount;
            public override Guid? GetTaxCode() => TaxCode;
            public override Dictionary<Guid, string> GetCustomFields() => CustomFields;
            public override CustomFields GetCustomFields2() => CustomFields2;
        }

        [ProtoMember(20)] public bool Obsolete_TrackQuantityToDeliver { get; set; }
        [ProtoMember(4)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines { get; set; }
        [ProtoMember(5)] public string Obsolete_DeliveryInstructions { get; set; }
        [ProtoMember(6)] public string Obsolete_AuthorizedBy { get; set; }
        [ProtoMember(7)] public DateTime? Obsolete_DeliveryDate { get; set; }
        [ProtoMember(8)] public string Obsolete_DeliveryAddress { get; set; }

        public override string GetDescriptionOrNull()
        {
            if (string.IsNullOrWhiteSpace(Description)) return null;
            return Description;
        }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Reference)) return Date.ToString("yyyyMMdd");
            return Reference;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return false;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (filter is Customer customer && Customer != customer.Key) return false;
            return true;
        }

        public override GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            if (Lines == null) return null;

            var customer = database.SingleOrDefault<Customer>(Customer);

            Currency currency = database.SingleOrDefault<ForeignCurrency>(customer?.Currency);
            if (currency == null) currency = database.Single<BaseCurrency>();

            var list = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();

            foreach (var line in Lines)
            {
                list.AddRange(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction.From(
                    database: database,
                    date: Date,
                    transaction: this,
                    transactionCurrency: currency,
                    transactionLine: line,
                    amountsIncludeTax: AmountsIncludeTax,
                    customer: customer,
                    reverseSign: true
                ));
            }

            var total = list.Select(x => x.TransactionAmount).SafeSum() * -1m;

            list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                transaction: this,
                date: Date,
                generalLedgerAccount: database.Single<BalanceSheetAccountsReceivableAccount>(),
                customer: customer,
                transactionAmount: total,
                transactionCurrency: currency,
                isBalancing: true
            ));

            if (WithholdingTax)
            {
                var withholdingTaxAmount = 0m;
                if (WithholdingTaxType == WithholdingTaxType.Rate && WithholdingTaxPercentage > 0m && WithholdingTaxPercentage <= 100m)
                {
                    withholdingTaxAmount = currency.Round(total / 100m * WithholdingTaxPercentage);
                }
                if (WithholdingTaxType == WithholdingTaxType.Amount && WithholdingTaxAmount > 0m)
                {
                    withholdingTaxAmount = WithholdingTaxAmount;
                }

                if (withholdingTaxAmount != 0m)
                {
                    list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        transaction: this,
                        date: Date,
                        generalLedgerAccount: database.Single<BalanceSheetWithholdingTaxReceivableAccount>(),
                        customer: customer,
                        transactionAmount: withholdingTaxAmount,
                        transactionCurrency: currency
                    ));

                    list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        transaction: this,
                        date: Date,
                        generalLedgerAccount: database.Single<BalanceSheetAccountsReceivableAccount>(),
                        customer: customer,
                        transactionAmount: withholdingTaxAmount * -1m,
                        transactionCurrency: currency
                    ));
                }
            }

            return list.ToArray();
        }

        int IComparable<SalesOrder>.CompareTo(SalesOrder other)
        {
            return (!other.IsInactive(), other.Date, other.Reference).CompareTo((!IsInactive(), Date, Reference));
        }
    }
}