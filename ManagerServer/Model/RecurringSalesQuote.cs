using System;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [Guide("Recurring sales quotes automate the creation of regular quotes for customers, ideal for businesses with frequent pricing requests.")]
    [Guide("Use recurring quotes for service contracts that require periodic renewal, subscription pricing updates, or regular bid submissions.")]
    [Guide("The system creates quotes automatically, which customers can review and accept. Accepted quotes can be converted to sales orders or invoices.")]
    [CustomFields]
    [ProtoContract]
    [Guid("1ca6ee3a-3583-41d8-83b1-74ac9129e1c1")]
    [Currency(nameof(Customer))]
    public sealed class RecurringSalesQuote : ManagerServer.Model.Object, IRecurringTransactionFor<SalesQuote>, ICustomFields
    {
        [Guide("The date when the next sales quote will be automatically created. This date advances automatically based on the interval and period type after each quote is generated.")]
        [ProtoMember(1), NoWrap] public DateTime? NextIssueDate { get; set; }
        [Guide("The number of days the quote remains valid before expiring. This sets the expiry date for each generated quote.")]
        [ProtoMember(2), Prepend(nameof(Strings.ValidFor)), Append(nameof(Strings.Days))] public int? ExpiryDate { get; set; }
        [Guide("The frequency interval for creating quotes. For example, enter 3 with 'Months' period type to create quotes every 3 months (quarterly).")]
        [ProtoMember(3), NoWrap, Placeholder("1"), Prepend(nameof(Strings.Every))] public int? Interval { get; set; }
        [Guide("The period type that works with the interval to determine frequency. Choose from Days, Weeks, Months, or Years.")]
        [ProtoMember(4), NoWrap, EmptyLabel] public Period PeriodType { get; set; }
        [Guide("For monthly recurring quotes, specify which day of the month the quote should be created.")]
        [ProtoMember(33), NoWrap, EmptyLabel, IfEnum(nameof(PeriodType), (int)Period.Month)] public MonthDay MonthDay { get; set; }
        [Guide("Determines when the recurring schedule ends. Choose 'Until further notice' for ongoing quotes or 'Until date' to stop on a specific date.")]
        [ProtoMember(5), EmptyLabel, NoWrap] public ExpirationType ExpirationType { get; set; }
        [Guide("The date when recurring quotes will stop being created. Only applicable when expiration type is set to 'Until date'.")]
        [ProtoMember(6), EmptyLabel, IfEnum(nameof(ExpirationType), (int)ExpirationType.Custom)] public DateTime? UntilDate { get; set; }
        [Guide("Select the customer for whom these recurring quotes will be created. All generated quotes will be for this customer.")]
        [ProtoMember(7), Autocomplete(typeof(Customer)), OnChangeSetDefault(nameof(BillingAddress))] public Guid? Customer { get; set; }
        [Guide("The billing address for the customer. This will be copied to each generated quote.")]
        [ProtoMember(8), Textarea] public string BillingAddress { get; set; }
        [Guide("A description that identifies this recurring quote template and will be copied to each generated quote.")]
        [ProtoMember(9), Long, Typeahead] public string Description { get; set; }
        [Guide("The quote lines specifying items, quantities, and prices. Each generated quote will include these lines.")]
        [ProtoMember(10)] public SalesQuote.Line[] Lines { get; set; }
        [ProtoMember(11), IfContains<TaxCode>] public bool AmountsIncludeTax { get; set; }
        [ProtoMember(12)] public bool Rounding { get; set; }
        [ProtoMember(13), IfTrue(nameof(Rounding)), NoLabel] public RoundingMethod RoundingMethod { get; set; }
        [ProtoMember(14), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [ProtoMember(15), Label(nameof(Strings.Column), nameof(Strings.Discount))] public bool Discount { get; set; }
        [ProtoMember(16), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        [ProtoMember(17)] public bool WithholdingTax { get; set; }
        [ProtoMember(18), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel] public WithholdingTaxType WithholdingTaxType { get; set; }
        [ProtoMember(19), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel, Append("%"), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Rate)] public decimal WithholdingTaxPercentage { get; set; }
        [ProtoMember(20), IfTrue(nameof(WithholdingTax)), NoLabel, AppendCurrency(nameof(Customer)), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Amount)] public decimal WithholdingTaxAmount { get; set; }
        [ProtoMember(21)] public bool HideTotalAmount { get; set; }
        [ProtoMember(22), IfContains<CustomTheme>, Label(nameof(Strings.CustomTheme))] public bool HasSalesQuoteCustomTheme { get; set; }
        [ProtoMember(23), IfTrue(nameof(HasSalesQuoteCustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? SalesQuoteCustomTheme { get; set; }
        [ProtoMember(24), Label(nameof(Strings.CustomTitle))] public bool HasSalesQuoteCustomTitle { get; set; }
        [ProtoMember(25), IfTrue(nameof(HasSalesQuoteCustomTitle)), Placeholder(nameof(Strings.Quote)), NoLabel] public string SalesQuoteCustomTitle { get; set; }
        [ProtoMember(26), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }
        [ProtoMember(27), Label(nameof(Strings.Footers))] public bool HasSalesQuoteFooters { get; set; }
        [ProtoMember(28), Autocomplete(typeof(ManagerServer.Model.SalesQuoteFooter)), NoLabel, IfTrue(nameof(HasSalesQuoteFooters))] public Guid[] SalesQuoteFooters { get; set; }
        [Guide("When enabled, automatically generates unique reference numbers for each created quote. Disable to use custom reference numbers.")]
        [ProtoMember(30)] public bool AutomaticReference { get; set; }
        [ProtoMember(31)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(32)] public CustomFields CustomFields2 { get; set; }

        DateTime? IRecurringTransaction.NextIssueDate { get => NextIssueDate; set => NextIssueDate = value; }
        int? IRecurringTransaction.Interval => Interval;
        Period IRecurringTransaction.PeriodType => PeriodType;
        ExpirationType IRecurringTransaction.ExpirationType => ExpirationType;
        DateTime? IRecurringTransaction.UntilDate => UntilDate;

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;

        MonthDay IRecurringTransaction.MonthDay => MonthDay;
    }
}
