using System;
using System.Collections.Generic;
using ManagerServer.Globalization;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [Guide("Recurring sales orders automate the creation of regular orders from repeat customers, streamlining your order processing.")]
    [Guide("Use recurring sales orders for standing orders, regular supply agreements, subscription-based products, or any predictable customer orders.")]
    [Guide("The system creates sales orders automatically, which can then be converted to invoices as goods are delivered or services are provided.")]
    [CustomFields]
    [ProtoContract]
    [Guid("dd7d5b17-c4be-4369-b0f5-79361525f3c2")]
    [Currency(nameof(Customer))]
    public sealed class RecurringSalesOrder : ManagerServer.Model.Object, IRecurringTransactionFor<SalesOrder>, ICustomFields
    {
        [Guide("The date when the next sales order will be automatically created. This date advances automatically based on the interval and period type after each order is generated.")]
        [ProtoMember(1), NoWrap] public DateTime? NextIssueDate { get; set; }
        [Guide("The number of days the sales order remains valid before expiring. This sets the expiry date for each generated order.")]
        [ProtoMember(2), Prepend(nameof(Strings.ValidFor)), Append(nameof(Strings.Days))] public int? ExpiryDate { get; set; }
        [Guide("The frequency interval for creating orders. For example, enter 2 with 'Weeks' period type to create orders every 2 weeks.")]
        [ProtoMember(3), NoWrap, Placeholder("1"), Prepend(nameof(Strings.Every))] public int? Interval { get; set; }
        [Guide("The period type that works with the interval to determine frequency. Choose from Days, Weeks, Months, or Years.")]
        [ProtoMember(4), NoWrap, EmptyLabel] public Period PeriodType { get; set; }
        [Guide("For monthly recurring orders, specify which day of the month the order should be created.")]
        [ProtoMember(30), NoWrap, EmptyLabel, IfEnum(nameof(PeriodType), (int)Period.Month)] public MonthDay MonthDay { get; set; }
        [Guide("Determines when the recurring schedule ends. Choose 'Until further notice' for ongoing orders or 'Until date' to stop on a specific date.")]
        [ProtoMember(5), EmptyLabel, NoWrap] public ExpirationType ExpirationType { get; set; }
        [Guide("The date when recurring orders will stop being created. Only applicable when expiration type is set to 'Until date'.")]
        [ProtoMember(6), EmptyLabel, IfEnum(nameof(ExpirationType), (int)ExpirationType.Custom)] public DateTime? UntilDate { get; set; }
        [Guide("Select the customer for whom these recurring orders will be created. All generated orders will be for this customer.")]
        [ProtoMember(7), Autocomplete(typeof(Customer)), OnChangeSetDefault(nameof(BillingAddress))] public Guid? Customer { get; set; }
        [Guide("The billing address for the customer. This will be copied to each generated order.")]
        [ProtoMember(8), Textarea, Short] public string BillingAddress { get; set; }
        [Guide("A description that identifies this recurring order template and will be copied to each generated order.")]
        [ProtoMember(9), Long, Typeahead] public string Description { get; set; }
        [Guide("The order lines specifying items, quantities, and prices. Each generated order will include these lines.")]
        [ProtoMember(10)] public SalesOrder.Line[] Lines { get; set; }
        [ProtoMember(11), IfContains<TaxCode>] public bool AmountsIncludeTax { get; set; }
        [ProtoMember(12), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [ProtoMember(13), Label(nameof(Strings.Column), nameof(Strings.Discount))] public bool Discount { get; set; }
        [ProtoMember(14), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        [ProtoMember(15)] public bool WithholdingTax { get; set; }
        [ProtoMember(16), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel] public WithholdingTaxType WithholdingTaxType { get; set; }
        [ProtoMember(17), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel, Append("%"), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Rate)] public decimal WithholdingTaxPercentage { get; set; }
        [ProtoMember(18), IfTrue(nameof(WithholdingTax)), NoLabel, AppendCurrency(nameof(Customer)), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Amount)] public decimal WithholdingTaxAmount { get; set; }
        [ProtoMember(19), IfAnyNotNull(nameof(SalesOrder.Line.Item))] public bool TrackQuantityToDeliver { get; set; }
        [ProtoMember(20), IfContains<CustomTheme>, Label(nameof(Strings.CustomTheme))] public bool HasSalesOrderCustomTheme { get; set; }
        [ProtoMember(21), IfTrue(nameof(HasSalesOrderCustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? SalesOrderCustomTheme { get; set; }
        [ProtoMember(22), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }
        [ProtoMember(28), Label(nameof(Strings.CustomTitle))] public bool HasSalesOrderCustomTitle { get; set; }
        [ProtoMember(29), IfTrue(nameof(HasSalesOrderCustomTitle)), Placeholder(nameof(Strings.SalesOrder)), NoLabel] public string SalesOrderCustomTitle { get; set; }
        [ProtoMember(23), Label(nameof(Strings.Footers))] public bool HasSalesOrderFooters { get; set; }
        [ProtoMember(24), Autocomplete(typeof(ManagerServer.Model.SalesOrderFooter)), NoLabel, IfTrue(nameof(HasSalesOrderFooters))] public Guid[] SalesOrderFooters { get; set; }
        [Guide("When enabled, automatically generates unique reference numbers for each created order. Disable to use custom reference numbers.")]
        [ProtoMember(25)] public bool AutomaticReference { get; set; }
        [ProtoMember(26)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(27)] public CustomFields CustomFields2 { get; set; }

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