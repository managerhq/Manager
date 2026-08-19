using System;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using static ManagerServer.Model.Attributes.ExpressionAttribute.Operators;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [Guide("Recurring sales invoices automatically generate invoices at specified intervals, saving time and ensuring consistent billing for repeat customers.")]
    [Guide("Use recurring sales invoices for subscription services, monthly retainers, maintenance contracts, rental agreements, or any regular billing cycle.")]
    [Guide("The system will create new invoices based on your schedule until you delete this recurring template or it reaches its expiration date.")]
    [CustomFields]
    [ProtoContract]
    [Guid("81385989-81e5-48c7-a819-c344324c1c01")]
    [Currency(nameof(Customer))]
    public sealed class RecurringSalesInvoice : Object, IRecurringTransactionFor<SalesInvoice>, ICustomFields, IHasCustomTheme
    {
        [Guide("The date when the next sales invoice will be automatically created. This date advances automatically based on your frequency settings.")]
        [Guide("The system checks daily for recurring transactions due to be created. Set this to today or earlier to create the first invoice immediately.")]
        [ProtoMember(11), NoWrap] public DateTime? NextIssueDate { get; set; }
        [Guide("The payment terms in days. The due date for each generated invoice will be calculated as the issue date plus this number of days.")]
        [Guide("For example, enter 30 for Net 30 payment terms, or 7 for payment due within one week.")]
        [ProtoMember(13), NoWrap, Prepend(nameof(Strings.Net)), Append(nameof(Strings.Days))] public int? DueDateDays { get; set; }
        [Guide("The frequency interval for creating invoices. This number works with the period type to set your billing cycle.")]
        [Guide("Common examples: 1 Month = monthly billing, 3 Months = quarterly billing, 1 Week = weekly billing, 1 Year = annual billing.")]
        [ProtoMember(10), NoWrap, Placeholder("1"), Prepend(nameof(Strings.Every))] public int? Interval { get; set; }
        [Guide("The period type that works with the interval to determine frequency. Select the unit of time for your billing cycle.")]
        [Guide("Most businesses use 'Months' for regular billing cycles. Use 'Days' for very frequent billing or 'Years' for annual contracts.")]
        [ProtoMember(9), NoWrap, EmptyLabel] public Period PeriodType { get; set; }
        [Guide("For monthly recurring invoices, specify which day of the month the invoice should be created.")]
        [Guide("Choose 'Same day each month' to maintain the original day, or select a specific day (1st, 15th, Last day, etc.) for consistent monthly billing.")]
        [ProtoMember(53), NoWrap, EmptyLabel, IfEnum(nameof(PeriodType), (int)Period.Month)] public MonthDay MonthDay { get; set; }
        [Guide("Determines when the recurring schedule ends. This controls how long the system will continue creating invoices.")]
        [Guide("Choose 'Until further notice' for indefinite subscriptions, or 'Until date' for contracts with a specific end date.")]
        [ProtoMember(26), EmptyLabel, NoWrap] public ExpirationType ExpirationType { get; set; }
        [Guide("The date when recurring invoices will stop being created. The system will not create any invoices after this date.")]
        [Guide("Use this for fixed-term contracts or subscriptions with a known end date. Leave blank for ongoing services.")]
        [ProtoMember(27), EmptyLabel, IfEnum(nameof(ExpirationType), (int)ExpirationType.Custom)] public DateTime? UntilDate { get; set; }
        [Guide("Select the customer who will receive these recurring invoices. Each generated invoice will be linked to this customer.")]
        [Guide("The customer's current billing address, credit terms, and other settings will be applied to each new invoice.")]
        [ProtoMember(1), Autocomplete(typeof(ManagerServer.Model.Customer)), OnChangeSetDefault(nameof(BillingAddress))] public Guid? Customer { get; set; }
        [Guide("The billing address for the customer. This address will appear on each generated invoice.")]
        [Guide("This field auto-fills from the customer record but can be customized for this recurring series if needed.")]
        [ProtoMember(2), Textarea] public string BillingAddress { get; set; }
        [Guide("A description that identifies what you're billing for. This appears on each generated invoice.")]
        [Guide("Be specific to help customers understand the charges. Examples: 'Monthly Website Hosting', 'Quarterly Maintenance Service', 'Annual Software License'.")]
        [ProtoMember(5), Long, Typeahead] public string Description { get; set; }
        [Guide("The invoice lines specifying what you're charging for. These exact lines will appear on every generated invoice.")]
        [Guide("Add lines for each product or service in your recurring billing. You can use inventory items, service items, or direct account postings.")]
        [Guide("Prices and tax codes set here will be used for all future invoices unless you edit this template.")]
        [ProtoMember(48)] public SalesInvoice.Line[] Lines { get; set; }
        [ProtoMember(49), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [ProtoMember(47), Label(nameof(Strings.Column), nameof(Strings.Description))] public bool HasLineDescription { get; set; }
        [ProtoMember(18), Label(nameof(Strings.Column), nameof(Strings.Discount))] public bool Discount { get; set; }
        [ProtoMember(19), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        //[ProtoMember(68), Label(nameof(Strings.Column), nameof(Strings.QtyDelivered))] public bool HasQtyDelivered;
        [ProtoMember(4)] public bool AmountsIncludeTax { get; set; }
        [ProtoMember(16)] public bool Rounding { get; set; }
        [ProtoMember(12), IfTrue(nameof(Rounding)), NoLabel] public RoundingMethod RoundingMethod { get; set; }
        [ProtoMember(23)] public bool WithholdingTax { get; set; }
        [ProtoMember(21), IfTrue(nameof(WithholdingTax)), NoLabel, NoWrap] public WithholdingTaxType WithholdingTaxType { get; set; }
        [ProtoMember(20), IfTrue(nameof(WithholdingTax)), NoLabel, NoWrap, IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Rate), Append("%")] public decimal WithholdingTaxRate { get; set; }
        [ProtoMember(22), IfTrue(nameof(WithholdingTax)), NoLabel, IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Amount), AppendCurrency(nameof(Customer))] public decimal WithholdingTaxAmount { get; set; }
        [ProtoMember(24), IfContains<CustomTheme>, Label(nameof(Strings.CustomTheme))] public bool HasSalesInvoiceCustomTheme { get; set; }
        [ProtoMember(25), IfTrue(nameof(HasSalesInvoiceCustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? SalesInvoiceCustomTheme { get; set; }
        [ProtoMember(28)] public bool EarlyPaymentDiscount { get; set; }
        [ProtoMember(29), IfTrue(nameof(EarlyPaymentDiscount)), NoLabel, NoWrap] public DiscountType EarlyPaymentDiscountType { get; set; }
        [ProtoMember(30), IfTrue(nameof(EarlyPaymentDiscount)), NoLabel, NoWrap, IfEnum(nameof(EarlyPaymentDiscountType), (int)DiscountType.Percentage), Append("%")] public decimal EarlyPaymentDiscountRate { get; set; }
        [ProtoMember(31), IfTrue(nameof(EarlyPaymentDiscount)), NoLabel, NoWrap, IfEnum(nameof(EarlyPaymentDiscountType), (int)DiscountType.ExactAmount), AppendCurrency(nameof(Customer))] public decimal EarlyPaymentDiscountAmount { get; set; }
        [ProtoMember(32), IfTrue(nameof(EarlyPaymentDiscount)), NoLabel, Prepend(nameof(Strings.If_paid_within)), Append(nameof(Strings.Days))] public int? EarlyPaymentDiscountDays { get; set; }
        [ProtoMember(33)] public bool LatePaymentFees { get; set; }
        [ProtoMember(15), IfTrue(nameof(LatePaymentFees)), Append("%"), Prepend(nameof(Strings.ChargeMonthly)), NoLabel] public decimal LatePaymentFeesPercentage { get; set; }
        [ProtoMember(42), Label(nameof(Strings.CustomTitle))] public bool HasSalesInvoiceCustomTitle { get; set; }
        [ProtoMember(40), IfTrue(nameof(HasSalesInvoiceCustomTitle)), NoLabel, Placeholder(nameof(Strings.Invoice))] public string SalesInvoiceCustomTitle { get; set; }
        [ProtoMember(43)] public bool TotalAmountInWords { get; set; }
        [ProtoMember(45)] public bool HideDueDate { get; set; }
        [ProtoMember(50), Label(nameof(Strings.Footers))] public bool HasSalesInvoiceFooters { get; set; }
        [ProtoMember(51), Autocomplete(typeof(ManagerServer.Model.SalesInvoiceFooter)), NoLabel, IfTrue(nameof(HasSalesInvoiceFooters))] public Guid[] SalesInvoiceFooters { get; set; }
        [Guide("When enabled, the system automatically assigns sequential reference numbers to each generated invoice.")]
        [Guide("Recommended for most businesses to ensure unique invoice numbers. Disable only if you have a specific numbering system requirement.")]
        [ProtoMember(46)] public bool AutomaticReference { get; set; }
        [ProtoMember(17)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(52)] public CustomFields CustomFields2 { get; set; }

        DateTime? IRecurringTransaction.NextIssueDate { get => NextIssueDate; set => NextIssueDate = value; }
        int? IRecurringTransaction.Interval => Interval;
        Period IRecurringTransaction.PeriodType => PeriodType;
        ExpirationType IRecurringTransaction.ExpirationType => ExpirationType;
        DateTime? IRecurringTransaction.UntilDate => UntilDate;

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;

        MonthDay IRecurringTransaction.MonthDay => MonthDay;

        bool IHasCustomTheme.CustomTheme { get => HasSalesInvoiceCustomTheme; set => HasSalesInvoiceCustomTheme = value; }
        Guid? IHasCustomTheme.CustomThemeId { get => SalesInvoiceCustomTheme; set => SalesInvoiceCustomTheme = value; }

        /*
        [ProtoContract]
        public sealed class Line
        {
            [ProtoMember(1), Autocomplete(typeof(ISaleItem)), OnChange(nameof(Qty), nameof(IItem.DefaultQuantity)), OnChange(nameof(Description), nameof(Description)), OnChange(nameof(UnitPrice), nameof(Manager.Model.InventoryItem.SalePrice)), OnChange(nameof(TaxCode), nameof(ISaleItem.SaleItemTaxCode)), OnChange(nameof(Division), nameof(ISaleItem.SaleItemTrackingCode)), Short] public Guid? Item { get; set; }
            [ProtoMember(2), Autocomplete(typeof(ISalesInvoiceAccount), Subtext = nameof(BalanceSheetAccount.Group)), Substitute(nameof(Item), nameof(ISaleItem.SaleItemAccount)), OnChange(nameof(TaxCode), nameof(TaxCode))] public Guid? Account { get; set; }
            [ProtoMember(9), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), Autocomplete(typeof(CapitalAccount), Filter = nameof(Account)), Prepend(nameof(Strings.CapitalAccount))] public Guid? CapitalAccount { get; set; }
            [ProtoMember(10), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), IfNotNull(nameof(CapitalAccount)), Autocomplete(typeof(SubAccount)), Prepend(nameof(Strings.SubAccount))] public Guid? SubAccount { get; set; }
            [ProtoMember(13), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForSpecialAccounts)), Autocomplete(typeof(SpecialAccount), Filter = nameof(Account)), Prepend(nameof(Strings.SpecialAccount))] public Guid? SpecialAccount { get; set; }
            [ProtoMember(14), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForFixedAssets)), Autocomplete(typeof(FixedAsset), Filter = nameof(Account)), Prepend(nameof(Strings.FixedAsset))] public Guid? FixedAsset { get; set; }
            [ProtoMember(15), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForIntangibleAssets)), Autocomplete(typeof(IntangibleAsset), Filter = nameof(Account)), Prepend(nameof(Strings.IntangibleAsset))] public Guid? IntangibleAsset { get; set; }
            [ProtoMember(17), IfTrue(nameof(LineDescription)), Textarea] public string Description { get; set; }
            [ProtoMember(25)] public Dictionary<Guid, string> CustomFields { get; set; }
            [ProtoMember(18), AppendValue(nameof(Item), nameof(Manager.Model.InventoryItem.UnitName)), Short] public decimal? Qty { get; set; }
            [ProtoMember(19), NoPlaceholder, AppendCurrency(nameof(Model.Customer))] public decimal UnitPrice { get; set; }
            [ProtoMember(20), IfDifferentCurrency, NoPlaceholder] public decimal CurrencyAmount { get; set; }
            [ProtoMember(23), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.Percentage), Short, Append("%")] public decimal DiscountPercentage { get; set; }
            [ProtoMember(24), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.ExactAmount)] public decimal DiscountAmount { get; set; }
            [Label(nameof(Strings.Total)), Expression(Zero, Plus, nameof(UnitPrice), Times, nameof(Qty), Minus, nameof(DiscountAmount), TimesPercentage, nameof(DiscountPercentage), Round), Sum, AppendCurrency(nameof(Model.Customer))] public object TotalBeforeTax { get; }
            [ProtoMember(21), Autocomplete(typeof(TaxCode)), Short] public Guid? TaxCode { get; set; }
            [IfTaxCodes, Expression(Zero, Plus, nameof(TotalBeforeTax), TimesTaxCode, nameof(TaxCode)), Sum, IfFalse(nameof(AmountsIncludeTax))] public object TaxAmount { get; }
            [IfTaxCodes, Expression(Zero, Plus, nameof(TotalBeforeTax), Plus, nameof(TaxAmount), Round), Sum, AppendCurrency(nameof(Model.Customer)), IfFalse(nameof(SalesOrder.AmountsIncludeTax))] public object Total { get; }
            [ProtoMember(26), Autocomplete(typeof(Project)), Short] public Guid? Project { get; set; }
            [ProtoMember(22), Autocomplete(typeof(Division)), IfTrue(nameof(Account), nameof(NamedObject.DivisionEnabled)), Short] public Guid? Division { get; set; }
        }
        */

        [ProtoMember(3)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines { get; set; }
        [ProtoMember(8)] public bool Obsolete_TotalRounded { get; set; }
        [ProtoMember(6)] public string Obsolete_Notes { get; set; }
        [ProtoMember(14)] public LatePaymentFeesType Obsolete_LatePaymentFees { get; set; }

        public DateTime? GetNextInvoiceDate()
        {
            if (!NextIssueDate.HasValue) return null;
            if (ExpirationType == ExpirationType.Custom && UntilDate.HasValue && UntilDate.Value < NextIssueDate.Value) return null;
            return NextIssueDate.Value;
        }
    }
}
