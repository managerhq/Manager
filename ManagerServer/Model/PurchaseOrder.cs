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
    [Guid("a26bbea1-57aa-4fd9-b7c9-e26b83114385")]
    [Currency(nameof(Supplier))]
    public sealed class PurchaseOrder : Transaction, IHasAutomaticReference, IRecurringTransactionDestination, ICustomFields, IComparable<PurchaseOrder>, ICode, IHasCustomTheme
    {
        [Guide("Enter the date of the purchase order. This is typically when the order is placed with the supplier.")]
        [ProtoMember(1), NoWrap] public DateTime Date { get; set; }
        [Guide("Enter a reference number for this purchase order. This could be your PO number or any reference that helps track the order.")]
        [ProtoMember(2)] public string Reference { get; set; }
        [Guide("Select the supplier from whom you are ordering. This determines the currency and payment terms for the order.")]
        [ProtoMember(3), NoWrap, Autocomplete(typeof(Supplier))] public Guid? Supplier { get; set; }
        [Guide("Optionally, link this purchase order to a purchase quote. This helps track quote-to-order conversion and ensures agreed prices.")]
        [ProtoMember(18), IfNotNull(nameof(Supplier)), Short, Autocomplete(typeof(PurchaseQuote), Filter = nameof(Supplier)), Placeholder(nameof(Strings.Optional)), EmptyLabel, Prepend(nameof(Strings.QuoteNumber))] public Guid? PurchaseQuote { get; set; }
        [Guide("Optionally, add a description or notes about this order, such as delivery instructions or special requirements.")]
        [ProtoMember(11), Long, Typeahead] public string Description { get; set; }
        [Guide("Enter the items you are ordering. Each line includes the item, quantity, unit price, and other details.")]
        [ProtoMember(17)] public Line[] Lines { get; set; }
        [Guide("Check this box if the prices from your supplier already include tax. Leave unchecked if tax is added on top of prices.")]
        [ProtoMember(9), IfContains<TaxCode>] public bool AmountsIncludeTax { get; set; }
        [Guide("Check this box to display line numbers on the purchase order. This helps reference specific items when communicating with suppliers.")]
        [ProtoMember(26), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [Guide("Check this box to enable a discount column where you can record negotiated line-item discounts.")]
        [ProtoMember(12), Label(nameof(Strings.Column), nameof(Strings.Discount))] public bool Discount { get; set; }
        [Guide("Select whether discounts are entered as percentages or fixed amounts.")]
        [ProtoMember(13), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        [Guide("Check this box if withholding tax applies to this purchase. This is typically required for certain types of services or suppliers.")]
        [ProtoMember(22)] public bool WithholdingTax { get; set; }
        [ProtoMember(23), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel] public WithholdingTaxType WithholdingTaxType { get; set; }
        [ProtoMember(24), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel, Append("%"), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Rate)] public decimal WithholdingTaxPercentage { get; set; }
        [ProtoMember(25), IfTrue(nameof(WithholdingTax)), NoLabel, AppendCurrency(nameof(Supplier)), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Amount)] public decimal WithholdingTaxAmount { get; set; }
        [ProtoMember(14), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(15), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
        [ProtoMember(21), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }
        [ProtoMember(30), Label(nameof(Strings.CustomTitle))] public bool HasPurchaseOrderCustomTitle { get; set; }
        [ProtoMember(31), IfTrue(nameof(HasPurchaseOrderCustomTitle)), Placeholder(nameof(Strings.PurchaseOrder)), NoLabel] public string PurchaseOrderCustomTitle { get; set; }
        [ProtoMember(27), Label(nameof(Strings.Footers))] public bool HasPurchaseOrderFooters { get; set; }
        [ProtoMember(28), Autocomplete(typeof(ManagerServer.Model.PurchaseOrderFooter)), NoLabel, IfTrue(nameof(HasPurchaseOrderFooters))] public Guid[] PurchaseOrderFooters { get; set; }
        [Guide("This indicates whether the purchase order has been cancelled. Cancelled orders remain in the system for record-keeping but are inactive.")]
        [ProtoMember(20), DoNotCopy] public bool Cancelled { get; set; }
        [ProtoMember(10)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(29)] public CustomFields CustomFields2 { get; set; }
        [ProtoMember(16), DoNotCopy] public bool AutomaticReference { get; set; }

        [ProtoMember(19)] public bool Obsolete_TrackQuantityToReceive { get; set; }
        [ProtoMember(4)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines { get; set; }
        [ProtoMember(5)] public string Obsolete_DeliveryInstructions { get; set; }
        [ProtoMember(6)] public string Obsolete_AuthorizedBy { get; set; }
        [ProtoMember(7)] public DateTime? Obsolete_DeliveryDate { get; set; }
        [ProtoMember(8)] public string Obsolete_DeliveryAddress { get; set; }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        DateTime IRecurringTransactionDestination.Date { get => Date; set => Date = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        string ICode.Code => Reference;

        public override string GetReference() => Reference;

        public override bool HasLineQty() => true;
        public override bool HasLineUnitPrice() => true;
        public override DiscountType? GetLineDicountType() => Discount ? DiscountType : null;

        public override bool IsInactive()
        {
            return Cancelled;
        }

        [CustomFields]
        [ProtoContract]
        [Guid("15e955cc-ec9b-4396-990e-b732fb74c57c")]
        public sealed class Line : ITransactionLine
        {
            [IfTrue(nameof(HasLineNumber)), LineNumber, Label("#")] public object LineNumber { get; }
            [ProtoMember(1), Short, Autocomplete(typeof(IPurchaseItem)), OnChangeSetDefault(nameof(Qty)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(PurchaseUnitPrice)), OnChangeSetDefault(nameof(TaxCode))] public Guid? Item { get; set; }
            [ProtoMember(2), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [ProtoMember(8)] public Dictionary<Guid, string> CustomFields { get; set; }
            [ProtoMember(10)] public CustomFields CustomFields2 { get; set; }
            [ProtoMember(3), AppendValue(nameof(Item), nameof(InventoryItem.UnitName)), Short] public decimal? Qty { get; set; }
            [ProtoMember(4), Label(nameof(Strings.UnitPrice))] public decimal PurchaseUnitPrice { get; set; }
            [ProtoMember(5), Label(nameof(Strings.Discount)), IfTrue(nameof(PurchaseOrder.Discount)), IfEnum(nameof(PurchaseOrder.DiscountType), (int)DiscountType.Percentage), Short, Append("%")] public decimal DiscountPercentage { get; set; }
            [ProtoMember(6), Label(nameof(Strings.Discount)), IfTrue(nameof(PurchaseOrder.Discount)), IfEnum(nameof(PurchaseOrder.DiscountType), (int)DiscountType.ExactAmount)] public decimal DiscountAmount { get; set; }
            [Label(nameof(Strings.Total)), Expression(Zero, Plus, nameof(PurchaseUnitPrice), Times, nameof(Qty), Round, Minus, nameof(DiscountAmount), TimesPercentage, nameof(DiscountPercentage), Round), Sum, AppendCurrency(nameof(PurchaseOrder.Supplier))] public object TotalBeforeTax { get; }
            [ProtoMember(7), Autocomplete(typeof(TaxCode)), Short] public Guid? TaxCode { get; set; }
            [ProtoMember(9), Autocomplete(typeof(Project)), Short] public Guid? Project { get; set; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), TimesTaxCode, nameof(TaxCode)), Sum, IfFalse(nameof(PurchaseOrder.AmountsIncludeTax))] public object TaxAmount { get; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), Plus, nameof(TaxAmount), Round), Sum, AppendCurrency(nameof(PurchaseOrder.Supplier)), IfFalse(nameof(PurchaseOrder.AmountsIncludeTax))] public object Total { get; }

            public override Guid? GetItem() => Item;
            protected override decimal? GetUnitPrice() => PurchaseUnitPrice;
            protected override decimal? GetQty() => Qty;
            protected override decimal? GetDiscountPercentage() => DiscountPercentage;
            protected override decimal? GetDiscountAmount() => DiscountAmount;
            protected override string GetLineDescription() => LineDescription;
            public override Guid? GetTaxCode() => TaxCode;
            public override Dictionary<Guid, string> GetCustomFields() => CustomFields;
            public override CustomFields GetCustomFields2() => CustomFields2;
            protected override Guid? GetProject() => Project;
        }

        public override string GetDescriptionOrNull()
        {
            if (!string.IsNullOrWhiteSpace(Description)) return Description;
            return null;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (filter is Supplier supplier && Supplier != supplier.Key) return false;
            return true;
        }

        public override string GetName()
        {
            return (!string.IsNullOrWhiteSpace(Reference) ? Reference + " — " : null) + Date.ToShortDateString();
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return false;
        }

        public override ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            var supplier = database.SingleOrDefault<Supplier>(Supplier);

            Currency currency = database.SingleOrDefault<ForeignCurrency>(supplier?.Currency);
            if (currency == null) currency = database.Single<BaseCurrency>();

            var list = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();

            if (Lines != null)
            {
                foreach (var line in Lines)
                {
                    list.AddRange(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction.From(
                        database: database,
                        date: Date,
                        transaction: this,
                        transactionCurrency: currency,
                        transactionLine: line,
                        amountsIncludeTax: AmountsIncludeTax,
                        supplier: supplier
                    ));
                }
            }

            var total = list.Sum(x => x.TransactionAmount);
            
            list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                transaction: this,
                date: Date,
                generalLedgerAccount: database.Single<BalanceSheetAccountsPayableAccount>(),
                supplier: supplier,
                transactionAmount: total * -1m,
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
                else if (WithholdingTaxType == WithholdingTaxType.Amount && WithholdingTaxAmount > 0m)
                {
                    withholdingTaxAmount = WithholdingTaxAmount;
                }

                if (withholdingTaxAmount != 0m)
                {
                    list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        transaction: this,
                        date: Date,
                        generalLedgerAccount: database.Single<BalanceSheetWithholdingTaxPayableAccount>(),
                        supplier: supplier,
                        transactionAmount: withholdingTaxAmount * -1m,
                        transactionCurrency: currency
                    ));

                    list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        transaction: this,
                        date: Date,
                        generalLedgerAccount: database.Single<BalanceSheetAccountsPayableAccount>(),
                        supplier: supplier,
                        transactionAmount: withholdingTaxAmount,
                        transactionCurrency: currency
                    ));
                }
            }

            return list.ToArray();
        }

        int IComparable<PurchaseOrder>.CompareTo(PurchaseOrder other)
        {
            return (!other.IsInactive(), other.Date, other.Reference).CompareTo((!IsInactive(), Date, Reference));
        }
    }
}