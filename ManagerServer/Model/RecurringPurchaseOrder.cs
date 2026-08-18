using System;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [Guide("Recurring purchase orders automate the creation of regular orders to suppliers, ensuring consistent inventory replenishment.")]
    [Guide("Use recurring purchase orders for regular stock replenishment, standing orders with suppliers, or any predictable purchasing needs.")]
    [Guide("The system creates purchase orders automatically, helping you maintain inventory levels and manage supplier relationships efficiently.")]
    [CustomFields]
    [ProtoContract]
    [Guid("3be38758-7bf2-46f1-84a5-34e8748cade0")]
    [Currency(nameof(Supplier))]
    public sealed class RecurringPurchaseOrder : Object, IRecurringTransactionFor<PurchaseOrder>, ICustomFields
    {
        [Guide("The date when the next purchase order will be automatically created. This date advances automatically based on the interval and period type after each order is generated.")]
        [ProtoMember(1), NoWrap] public DateTime? NextIssueDate { get; set; }
        [Guide("The number of days after the issue date when delivery is expected. This sets the due date for each generated order.")]
        [ProtoMember(2), NoWrap] public int? DueDate { get; set; }
        [Guide("The frequency interval for creating orders. For example, enter 1 with 'Weeks' period type to create orders every week.")]
        [ProtoMember(3), NoWrap, Placeholder("1"), Prepend(nameof(Strings.Every))] public int? Interval { get; set; }
        [Guide("The period type that works with the interval to determine frequency. Choose from Days, Weeks, Months, or Years.")]
        [ProtoMember(4), NoWrap, EmptyLabel] public Period PeriodType { get; set; }
        [Guide("For monthly recurring orders, specify which day of the month the order should be created.")]
        [ProtoMember(27), NoWrap, EmptyLabel, IfEnum(nameof(PeriodType), (int)Period.Month)] public MonthDay MonthDay { get; set; }
        [Guide("Determines when the recurring schedule ends. Choose 'Until further notice' for ongoing orders or 'Until date' to stop on a specific date.")]
        [ProtoMember(5), NoWrap, EmptyLabel] public ExpirationType ExpirationType { get; set; }
        [Guide("The date when recurring orders will stop being created. Only applicable when expiration type is set to 'Until date'.")]
        [ProtoMember(6), EmptyLabel, IfEnum(nameof(ExpirationType), (int)ExpirationType.Custom)] public DateTime? UntilDate { get; set; }
        [Guide("Select the supplier to whom these recurring orders will be sent. All generated orders will be for this supplier.")]
        [ProtoMember(7), Autocomplete(typeof(Supplier))] public Guid? Supplier { get; set; }
        [Guide("A description that identifies this recurring order template and will be copied to each generated order.")]
        [ProtoMember(8), Long, Typeahead] public string Description { get; set; }
        [Guide("The order lines specifying items, quantities, and prices. Each generated order will include these lines.")]
        [ProtoMember(9)] public PurchaseOrder.Line[] Lines { get; set; }
        [ProtoMember(10), IfContains<TaxCode>] public bool AmountsIncludeTax { get; set; }
        [ProtoMember(11), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [ProtoMember(12), Label(nameof(Strings.Column), nameof(Strings.Discount))] public bool Discount { get; set; }
        [ProtoMember(13), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        [ProtoMember(14)] public bool WithholdingTax { get; set; }
        [ProtoMember(15), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel] public WithholdingTaxType WithholdingTaxType { get; set; }
        [ProtoMember(16), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel, Append("%"), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Rate)] public decimal WithholdingTaxPercentage { get; set; }
        [ProtoMember(17), IfTrue(nameof(WithholdingTax)), NoLabel, AppendCurrency(nameof(Supplier)), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Amount)] public decimal WithholdingTaxAmount { get; set; }
        [ProtoMember(18), IfAnyNotNull(nameof(PurchaseOrder.Line.Item))] public bool TrackQuantityToReceive { get; set; }
        [ProtoMember(19), IfContains<CustomTheme>, Label(nameof(Strings.CustomTheme))] public bool HasPurchaseOrderCustomTheme { get; set; }
        [ProtoMember(20), IfTrue(nameof(HasPurchaseOrderCustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? PurchaseOrderCustomTheme { get; set; }
        [ProtoMember(21), IfContains<TaxCode>] public bool ShowTaxAmountColumn { get; set; }
        [ProtoMember(22), Label(nameof(Strings.Footers))] public bool HasPurchaseOrderFooters { get; set; }
        [ProtoMember(23), Autocomplete(typeof(ManagerServer.Model.PurchaseOrderFooter)), NoLabel, IfTrue(nameof(HasPurchaseOrderFooters))] public Guid[] PurchaseOrderFooters { get; set; }
        [Guide("When enabled, automatically generates unique reference numbers for each created order. Disable to use custom reference numbers.")]
        [ProtoMember(24)] public bool AutomaticReference { get; set; }
        [ProtoMember(25)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(26)] public CustomFields CustomFields2 { get; set; }

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