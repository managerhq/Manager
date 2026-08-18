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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("ba89de75-cb87-4bde-b20f-314f01b31037")]
    [Currency(nameof(Customer))]
    public sealed class SalesQuote : Transaction, IHasAutomaticReference, IRecurringTransactionDestination, ICustomFields, IComparable<SalesQuote>, ICode, IHasCustomTheme
    {
        [Guide("Enter the date when this quote was issued to the customer.")]
        [ProtoMember(1), NoWrap] public DateTime IssueDate { get; set; }
        [Guide("Enter the number of days this quote remains valid. After this period, the quoted prices may no longer apply.")]
        [ProtoMember(26), NoWrap, Prepend(nameof(Strings.ValidFor)), Append(nameof(Strings.Days))] public int? ExpiryDate { get; set; }
        [Guide("Enter a reference number for this quote. This helps track and identify quotes when communicating with customers.")]
        [ProtoMember(2)] public string Reference { get; set; }
        [Guide("Select the customer or prospect who will receive this quote. Their billing address will automatically populate.")]
        [ProtoMember(3), Autocomplete(typeof(Customer)), OnChangeSetDefault(nameof(BillingAddress))] public Guid? Customer { get; set; }
        [Guide("Enter the customer's billing address. This is automatically filled from the customer record but can be modified for this quote.")]
        [ProtoMember(4), Textarea] public string BillingAddress { get; set; }
        [Guide("Optionally, add a description, introduction, or notes about this quote. This appears at the top of the quote document.")]
        [ProtoMember(9), Long, Typeahead] public string Description { get; set; }
        [Guide("Enter the products or services being quoted. Each line includes item details, quantities, prices, and descriptions.")]
        [ProtoMember(24)] public Line[] Lines { get; set; }
        [Guide("Check this box if the prices you enter already include tax. Leave unchecked if you want tax calculated on top of the entered prices.")]
        [ProtoMember(8), IfContains<TaxCode>] public bool AmountsIncludeTax { get; set; }
        [Guide("Check this box to enable rounding of the total amount. This is useful when you want to quote round numbers.")]
        [ProtoMember(16)] public bool Rounding { get; set; }
        [Guide("Select how to round the total amount - to the nearest whole number, nearest 10, nearest 100, etc.")]
        [ProtoMember(13), IfTrue(nameof(Rounding)), NoLabel] public RoundingMethod RoundingMethod { get; set; }
        [Guide("Check this box to display line numbers on the quote. This helps when discussing specific items with customers.")]
        [ProtoMember(32), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [Guide("Check this box to enable a discount column where you can offer line-item discounts.")]
        [ProtoMember(17), Label(nameof(Strings.Column), nameof(Strings.Discount))] public bool Discount { get; set; }
        [Guide("Select whether discounts are entered as percentages or fixed amounts.")]
        [ProtoMember(18), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        [ProtoMember(28)] public bool WithholdingTax { get; set; }
        [ProtoMember(29), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel] public WithholdingTaxType WithholdingTaxType { get; set; }
        [ProtoMember(30), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel, Append("%"), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Rate)] public decimal WithholdingTaxPercentage { get; set; }
        [ProtoMember(31), IfTrue(nameof(WithholdingTax)), NoLabel, AppendCurrency(nameof(Customer)), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Amount)] public decimal WithholdingTaxAmount { get; set; }
        [Guide("Check this box to hide the total amount on the printed quote. Useful for quotes where you only want to show unit prices.")]
        [ProtoMember(19)] public bool HideTotalAmount { get; set; }
        [ProtoMember(20), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(21), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
        [ProtoMember(22), Label(nameof(Strings.CustomTitle))] public bool HasSalesQuoteCustomTitle { get; set; }
        [ProtoMember(14), IfTrue(nameof(HasSalesQuoteCustomTitle)), Placeholder(nameof(Strings.Quote)), NoLabel] public string SalesQuoteCustomTitle { get; set; }
        [ProtoMember(36)] public bool ShowItemImages { get; set; }
        [ProtoMember(27), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }
        [ProtoMember(33), Label(nameof(Strings.Footers))] public bool HasSalesQuoteFooters { get; set; }
        [ProtoMember(34), Autocomplete(typeof(ManagerServer.Model.SalesQuoteFooter)), NoLabel, IfTrue(nameof(HasSalesQuoteFooters))] public Guid[] SalesQuoteFooters { get; set; }
        [Guide("This indicates whether the quote has been cancelled. Cancelled quotes remain in the system for record-keeping but are marked as inactive.")]
        [ProtoMember(25), DoNotCopy] public bool Cancelled { get; set; }
        [ProtoMember(23), DoNotCopy] public bool AutomaticReference { get; set; }
        [ProtoMember(10)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(35)] public CustomFields CustomFields2 { get; set; }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        DateTime IRecurringTransactionDestination.Date { get => IssueDate; set => IssueDate = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        string ICode.Code => Reference;

        public override bool HasLineQty() => true;
        public override bool HasLineUnitPrice() => true;
        public override DiscountType? GetLineDicountType() => Discount ? DiscountType : null;

        public override string GetReference() => Reference;

        public override bool IsInactive()
        {
            return Cancelled;
        }

        [CustomFields]
        [ProtoContract]
        [Guid("2b604a37-c978-4592-955b-428b9c891c1d")]
        public sealed class Line : ITransactionLine
        {
            [IfTrue(nameof(HasLineNumber)), LineNumber, Label("#")] public object LineNumber { get; }
            [ProtoMember(1), Short, Autocomplete(typeof(ISaleItem)), OnChangeSetDefault(nameof(LineDescription)), OnChangeSetDefault(nameof(Qty)), OnChangeSetDefault(nameof(SalesUnitPrice)), OnChangeSetDefault(nameof(TaxCode))] public Guid? Item { get; set; }
            [ProtoMember(2), Label(nameof(Strings.Description)), Textarea] public string LineDescription { get; set; }
            [ProtoMember(8)] public Dictionary<Guid, string> CustomFields { get; set; }
            [ProtoMember(9)] public CustomFields CustomFields2 { get; set; }
            [ProtoMember(3), AppendValue(nameof(Item), nameof(InventoryItem.UnitName)), Short] public decimal? Qty { get; set; }
            [ProtoMember(4), Label(nameof(Strings.UnitPrice))] public decimal SalesUnitPrice { get; set; }
            [ProtoMember(5), Label(nameof(Strings.Discount)), IfTrue(nameof(SalesQuote.Discount)), IfEnum(nameof(SalesQuote.DiscountType), (int)DiscountType.Percentage), Short, Append("%")] public decimal DiscountPercentage { get; set; }
            [ProtoMember(6), Label(nameof(Strings.Discount)), IfTrue(nameof(SalesQuote.Discount)), IfEnum(nameof(SalesQuote.DiscountType), (int)DiscountType.ExactAmount)] public decimal DiscountAmount { get; set; }
            [Label(nameof(Strings.Total)), Expression(Zero, Plus, nameof(SalesUnitPrice), Times, nameof(Qty), Round, Minus, nameof(DiscountAmount), TimesPercentage, nameof(DiscountPercentage), Round), Sum, AppendCurrency(nameof(SalesQuote.Customer))] public object TotalBeforeTax { get; }
            [ProtoMember(7), Autocomplete(typeof(TaxCode)), Short] public Guid? TaxCode { get; set; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), TimesTaxCode, nameof(TaxCode)), Sum, IfFalse(nameof(SalesQuote.AmountsIncludeTax))] public object TaxAmount { get; }
            [IfContains<TaxCode>, Expression(Zero, Plus, nameof(TotalBeforeTax), Plus, nameof(TaxAmount), Round), Sum, AppendCurrency(nameof(SalesQuote.Customer)), IfFalse(nameof(SalesQuote.AmountsIncludeTax))] public object Total { get; }

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

        [ProtoMember(5)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines { get; set; }
        [ProtoMember(7)] public string Obsolete_InternalNotes { get; set; }
        [ProtoMember(12)] public bool Obsolete_TotalRounded { get; set; }
        [ProtoMember(15)] public bool Obsolete_CustomDocumentHeader { get; set; }
        [ProtoMember(11)] public string Obsolete_Notes { get; set; }

        public override string GetDescriptionOrNull()
        {
            if (string.IsNullOrWhiteSpace(Description)) return null;
            return Description;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (filter is Customer customer && Customer != customer.Key) return false;
            return true;
        }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Reference)) return IssueDate.ToString("yyyyMMdd");
            return Reference;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return false;
        }

        public DateTime? GetExpiryDate()
        {
            if (ExpiryDate.HasValue)
            {
                try
                {
                    return IssueDate.AddDays(ExpiryDate.Value);
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }
            return null;
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
                    date: IssueDate,
                    transaction: this,
                    transactionCurrency: currency,
                    transactionLine: line,
                    amountsIncludeTax: AmountsIncludeTax,
                    customer: customer,
                    reverseSign: true
                ));
            }

            var total = list.Sum(x => x.TransactionAmount) * -1m;

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
                            transactionCurrency: currency
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
                            transactionCurrency: currency
                        ));
                    }
                }
            }

            total = list.Sum(x => x.TransactionAmount) * -1m;

            list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                transaction: this,
                date: IssueDate,
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
                        date: IssueDate,
                        generalLedgerAccount: database.Single<BalanceSheetWithholdingTaxReceivableAccount>(),
                        customer: customer,
                        transactionAmount: withholdingTaxAmount,
                        transactionCurrency: currency
                    ));

                    list.Add(new Query.GeneralLedger.GeneralLedgerTransaction(
                        database: database,
                        transaction: this,
                        date: IssueDate,
                        generalLedgerAccount: database.Single<BalanceSheetAccountsReceivableAccount>(),
                        customer: customer,
                        transactionAmount: withholdingTaxAmount * -1m,
                        transactionCurrency: currency
                    ));
                }
            }

            return list.ToArray();
        }

        int IComparable<SalesQuote>.CompareTo(SalesQuote other)
        {
            return (!other.IsInactive(), other.IssueDate, other.Reference).CompareTo((!IsInactive(), IssueDate, Reference));
        }
    }
}
