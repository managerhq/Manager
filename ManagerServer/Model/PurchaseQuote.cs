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
    [Guid("38d606f7-6358-4ace-9d1d-f6c0b9ea9d68")]
    [Currency(nameof(Supplier))]
    public sealed class PurchaseQuote : Transaction, IHasAutomaticReference, ICustomFields, IComparable<PurchaseQuote>, ICode, IHasCustomTheme
    {
        [Guide("Enter the date of the purchase quote. This is typically when the quote was received from or sent to the supplier.")]
        [ProtoMember(1), NoWrap] public DateTime Date { get; set; }
        [Guide("Enter a reference number for this quote. This could be the supplier's quote number or your RFQ reference.")]
        [ProtoMember(2)] public string Reference { get; set; }
        [Guide("Select the supplier who provided this quote or to whom you're sending a request for quotation.")]
        [ProtoMember(3), Autocomplete(typeof(Supplier))] public Guid? Supplier { get; set; }
        [Guide("Optionally, add a description or notes about this quote, such as special terms, validity period, or requirements.")]
        [ProtoMember(11), Long, Typeahead] public string Description { get; set; }
        [Guide("Enter the items being quoted. Each line includes the item, quantity, unit price (if known), and descriptions.")]
        [ProtoMember(17)] public Line[] Lines { get; set; }
        [Guide("Check this box if this is a request for quotation (RFQ) being sent to suppliers, rather than a quote received from them.")]
        [ProtoMember(18)] public bool RequestForQuotation { get; set; }
        [Guide("Check this box if the quoted prices already include tax. Leave unchecked if tax is added on top of the quoted prices.")]
        [ProtoMember(9), IfContains<TaxCode>, IfFalse(nameof(RequestForQuotation))] public bool AmountsIncludeTax { get; set; }
        [Guide("Check this box to display line numbers on the quote. This helps reference specific items when discussing with suppliers.")]
        [ProtoMember(29), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [Guide("Check this box to enable a discount column for recording any discounts offered by the supplier.")]
        [ProtoMember(12), Label(nameof(Strings.Column), nameof(Strings.Discount)), IfFalse(nameof(RequestForQuotation))] public bool Discount { get; set; }
        [Guide("Select whether discounts are entered as percentages or fixed amounts.")]
        [ProtoMember(13), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        [Guide("Check this box if withholding tax applies to this purchase. This affects the final amount payable to the supplier.")]
        [ProtoMember(25)] public bool WithholdingTax { get; set; }
        [ProtoMember(26), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel] public WithholdingTaxType WithholdingTaxType { get; set; }
        [ProtoMember(27), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel, Append("%"), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Rate)] public decimal WithholdingTaxPercentage { get; set; }
        [ProtoMember(28), IfTrue(nameof(WithholdingTax)), NoLabel, AppendCurrency(nameof(Supplier)), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Amount)] public decimal WithholdingTaxAmount { get; set; }
        [ProtoMember(14), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(15), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
        [ProtoMember(16), DoNotCopy] public bool AutomaticReference { get; set; }
        [Guide("This indicates whether the quote has been cancelled. Cancelled quotes remain in the system for record-keeping but are marked as inactive.")]
        [ProtoMember(19), DoNotCopy] public bool Cancelled { get; set; }
        [ProtoMember(20), IfFalse(nameof(RequestForQuotation)), Label(nameof(Strings.CustomTitle))] public bool HasPurchaseQuoteCustomTitle { get; set; }
        [ProtoMember(21), IfTrue(nameof(HasPurchaseQuoteCustomTitle)), Placeholder(nameof(Strings.PurchaseQuote)), NoLabel] public string PurchaseQuoteCustomTitle { get; set; }
        [ProtoMember(22), IfTrue(nameof(RequestForQuotation)), Label(nameof(Strings.CustomTitle))] public bool RequestForQuotationCustomTitleOption { get; set; }
        [ProtoMember(23), IfTrue(nameof(RequestForQuotationCustomTitleOption)), Placeholder(nameof(Strings.RequestForQuotation)), NoLabel] public string RequestForQuotationCustomTitle { get; set; }
        [ProtoMember(24), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }
        [ProtoMember(30), Label(nameof(Strings.Footers))] public bool HasPurchaseQuoteFooters { get; set; }
        [ProtoMember(31), Autocomplete(typeof(ManagerServer.Model.PurchaseQuoteFooter)), NoLabel, IfTrue(nameof(HasPurchaseQuoteFooters))] public Guid[] PurchaseQuoteFooters { get; set; }
        [ProtoMember(10)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(32)] public CustomFields CustomFields2 { get; set; }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        string ICode.Code => Reference;

        public override string GetReference() => Reference;

        public override bool HasLineQty() => true;
        public override bool HasLineUnitPrice() => !RequestForQuotation;
        public override DiscountType? GetLineDicountType() => Discount && !RequestForQuotation ? DiscountType : null;

        public override bool IsInactive()
        {
            return Cancelled;
        }

        [CustomFields]
        [ProtoContract]
        [Guid("2da30cfc-2e68-44db-aecd-83be44cd2bbf")]
        public sealed class Line : ITransactionLine
        {
            [IfTrue(nameof(HasLineNumber)), LineNumber, Label("#")] public object LineNumber { get; }
            [ProtoMember(1), Short, DoNotHide, Autocomplete(typeof(IPurchaseItem)), OnChangeSetDefault(nameof(Qty)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(PurchaseUnitPrice)), OnChangeSetDefault(nameof(TaxCode))] public Guid? Item { get; set; }
            [ProtoMember(2), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [ProtoMember(8)] public Dictionary<Guid, string> CustomFields { get; set; }
            [ProtoMember(9)] public CustomFields CustomFields2 { get; set; }
            [ProtoMember(3), AppendValue(nameof(Item), nameof(InventoryItem.UnitName)), Short] public decimal? Qty { get; set; }
            [ProtoMember(4), IfFalse(nameof(PurchaseQuote.RequestForQuotation)), Label(nameof(Strings.UnitPrice))] public decimal PurchaseUnitPrice { get; set; }
            [ProtoMember(5), IfFalse(nameof(PurchaseQuote.RequestForQuotation)), Label(nameof(Strings.Discount)), IfTrue(nameof(PurchaseQuote.Discount)), IfEnum(nameof(PurchaseQuote.DiscountType), (int)DiscountType.Percentage), Short, Append("%")] public decimal DiscountPercentage { get; set; }
            [ProtoMember(6), IfFalse(nameof(PurchaseQuote.RequestForQuotation)), Label(nameof(Strings.Discount)), IfTrue(nameof(PurchaseQuote.Discount)), IfEnum(nameof(PurchaseQuote.DiscountType), (int)DiscountType.ExactAmount)] public decimal DiscountAmount { get; set; }
            [Label(nameof(Strings.Total)), Expression(Zero, Plus, nameof(PurchaseUnitPrice), Times, nameof(Qty), Round, Minus, nameof(DiscountAmount), TimesPercentage, nameof(DiscountPercentage), Round), Sum, AppendCurrency(nameof(PurchaseQuote.Supplier)), IfFalse(nameof(PurchaseQuote.RequestForQuotation))] public object TotalBeforeTax { get; }
            [ProtoMember(7), Autocomplete(typeof(TaxCode)), Short, IfFalse(nameof(PurchaseQuote.RequestForQuotation))] public Guid? TaxCode { get; set; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), TimesTaxCode, nameof(TaxCode)), Sum, IfFalse(nameof(PurchaseQuote.AmountsIncludeTax)), IfFalse(nameof(PurchaseQuote.RequestForQuotation))] public object TaxAmount { get; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), Plus, nameof(TaxAmount), Round), Sum, AppendCurrency(nameof(PurchaseQuote.Supplier)), IfFalse(nameof(PurchaseQuote.AmountsIncludeTax)), IfFalse(nameof(PurchaseQuote.RequestForQuotation))] public object Total { get; }

            public override Guid? GetItem() => Item;
            protected override decimal? GetUnitPrice() => PurchaseUnitPrice;
            protected override decimal? GetQty() => Qty;
            protected override decimal? GetDiscountPercentage() => DiscountPercentage;
            protected override decimal? GetDiscountAmount() => DiscountAmount;
            protected override string GetLineDescription() => LineDescription;
            public override Guid? GetTaxCode() => TaxCode;
            public override Dictionary<Guid, string> GetCustomFields() => CustomFields;
            public override CustomFields GetCustomFields2() => CustomFields2;
        }

        [ProtoMember(4)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines { get; set; }

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
            if (string.IsNullOrWhiteSpace(Reference)) return Date.ToString("yyyyMMdd");
            return Reference;
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
                isBalancing: true,
                contraTransactions: list.ToArray()
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

        int IComparable<PurchaseQuote>.CompareTo(PurchaseQuote other)
        {
            return (!other.IsInactive(), other.Date, other.Reference).CompareTo((!IsInactive(), Date, Reference));
        }
    }
}
