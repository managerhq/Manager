using System;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [Guide("Recurring purchase invoices help you track regular bills from suppliers by automatically creating purchase invoices on a set schedule.")]
    [Guide("Use recurring purchase invoices for rent, utilities, subscriptions, service contracts, or any regular payments to suppliers.")]
    [Guide("The system creates purchase invoices automatically, helping you maintain accurate records of expected expenses and ensuring nothing is missed.")]
    [CustomFields]
    [ProtoContract]
    [Guid("11de04ac-c448-4665-b206-8aa631e63532")]
    [Currency(nameof(Supplier))]
    public sealed class RecurringPurchaseInvoice : Object, IRecurringTransactionFor<PurchaseInvoice>, ICustomFields, IHasCustomTheme
    {
        [Guide("The date when the next purchase invoice will be automatically created. This date advances automatically based on your frequency settings.")]
        [Guide("Set this to match when you expect to receive the supplier's invoice. The system checks daily for invoices due to be created.")]
        [ProtoMember(9), NoWrap] public DateTime? NextIssueDate { get; set; }
        [Guide("The number of days after the issue date when payment is due. This sets the due date for each generated invoice.")]
        [Guide("This should match your supplier's payment terms. For example, enter 30 for Net 30 terms, or 14 for payment due in two weeks.")]
        [ProtoMember(10), NoWrap, Prepend(nameof(Strings.Net)), Append(nameof(Strings.Days)), Label(nameof(Strings.DueDate))] public int? DueDateDays { get; set; }
        [Guide("The frequency interval for creating invoices. This number works with the period type to match your supplier's billing cycle.")]
        [Guide("Common examples: 1 Month = monthly bills, 3 Months = quarterly payments, 1 Year = annual contracts, 2 Weeks = fortnightly services.")]
        [ProtoMember(8), NoWrap, Placeholder("1"), Prepend(nameof(Strings.Every))] public int? Interval { get; set; }
        [Guide("The period type that works with the interval to determine frequency. Select the unit of time for your supplier's billing cycle.")]
        [Guide("Most suppliers bill monthly, but adjust this to match your actual billing schedule.")]
        [ProtoMember(7), NoWrap, EmptyLabel] public Period PeriodType { get; set; }
        [Guide("For monthly recurring invoices, specify which day of the month the invoice should be created.")]
        [Guide("Match this to when your supplier typically issues invoices. For example, rent due on the 1st, utilities on the 15th.")]
        [ProtoMember(30), NoWrap, EmptyLabel, IfEnum(nameof(PeriodType), (int)Period.Month)] public MonthDay MonthDay { get; set; }
        [Guide("Determines when the recurring schedule ends. This controls how long the system will continue creating purchase invoices.")]
        [Guide("Choose 'Until further notice' for ongoing expenses like utilities, or 'Until date' for fixed-term contracts.")]
        [ProtoMember(16), NoWrap, EmptyLabel] public ExpirationType ExpirationType { get; set; }
        [Guide("The date when recurring invoices will stop being created. The system will not create any invoices after this date.")]
        [Guide("Use this for leases, fixed-term service contracts, or any expense with a known end date.")]
        [ProtoMember(17), EmptyLabel, IfEnum(nameof(ExpirationType), (int)ExpirationType.Custom)] public DateTime? UntilDate { get; set; }
        [Guide("Select the supplier who bills you regularly. Each generated purchase invoice will be linked to this supplier.")]
        [Guide("The supplier's payment terms and other settings will be applied to each new invoice.")]
        [ProtoMember(1), Autocomplete(typeof(Supplier))] public Guid? Supplier { get; set; }
        [ProtoMember(25), Hidden] public Guid? PurchaseOrder; // https://forum.manager.io/t/recurring-purchase-invoice-edit-screen-problem/41559
        [Guide("A description that identifies what you're being billed for. This appears on each generated purchase invoice.")]
        [Guide("Be descriptive to help with expense tracking. Examples: 'Monthly Office Rent', 'Quarterly Software License', 'Weekly Cleaning Service'.")]
        [ProtoMember(4), Placeholder(nameof(Strings.Optional)), Long] public string Description { get; set; }
        [Guide("The invoice lines specifying what you're being charged for. These exact lines will appear on every generated purchase invoice.")]
        [Guide("Set up lines for each expense category. Use the appropriate expense accounts for proper financial reporting.")]
        [Guide("If prices are subject to change, you'll need to edit this template when your supplier adjusts their rates.")]
        [ProtoMember(19)] public PurchaseInvoice.Line[] Lines { get; set; }
        [ProtoMember(26), Label(nameof(Strings.Column), nameof(Strings.LineNumber))] public bool HasLineNumber { get; set; }
        [ProtoMember(20), Label(nameof(Strings.Column), nameof(Strings.Description))] public bool HasLineDescription { get; set; }
        //[ProtoMember(63), Label(nameof(Strings.Column), nameof(Strings.QtyReceived))] public bool HasQtyReceived;
        [ProtoMember(3), IfContains<TaxCode>] public bool AmountsIncludeTax { get; set; }
        [Guide("When enabled, the system automatically assigns reference numbers to each generated purchase invoice.")]
        [Guide("You might disable this if you want to manually enter your supplier's invoice numbers when you receive them.")]
        [ProtoMember(18)] public bool AutomaticReference { get; set; }
        [ProtoMember(12)] public bool Discount { get; set; }
        [ProtoMember(13), IfTrue(nameof(Discount)), NoLabel] public DiscountType DiscountType { get; set; }
        [ProtoMember(21)] public bool WithholdingTax { get; set; }
        [ProtoMember(22), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel] public WithholdingTaxType WithholdingTaxType { get; set; }
        [ProtoMember(23), IfTrue(nameof(WithholdingTax)), NoWrap, NoLabel, Append("%"), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Rate)] public decimal WithholdingTaxPercentage { get; set; }
        [ProtoMember(24), IfTrue(nameof(WithholdingTax)), NoLabel, AppendCurrency(nameof(Supplier)), IfEnum(nameof(WithholdingTaxType), (int)WithholdingTaxType.Amount)] public decimal WithholdingTaxAmount { get; set; }
        [ProtoMember(14), IfContains<CustomTheme>, Label(nameof(Strings.CustomTheme))] public bool HasPurchaseInvoiceCustomTheme { get; set; }
        [ProtoMember(15), IfTrue(nameof(HasPurchaseInvoiceCustomTheme)), NoLabel, Autocomplete(typeof(CustomTheme))] public Guid? PurchaseInvoiceCustomTheme { get; set; }
        [ProtoMember(27), Label(nameof(Strings.Footers))] public bool HasPurchaseInvoiceFooters { get; set; }
        [ProtoMember(28), Autocomplete(typeof(ManagerServer.Model.PurchaseInvoiceFooter)), NoLabel, IfTrue(nameof(HasPurchaseInvoiceFooters))] public Guid[] PurchaseInvoiceFooters { get; set; }
        [ProtoMember(11)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(29)] public CustomFields CustomFields2 { get; set; }

        DateTime? IRecurringTransaction.NextIssueDate { get => NextIssueDate; set => NextIssueDate = value; }
        int? IRecurringTransaction.Interval => Interval;
        Period IRecurringTransaction.PeriodType => PeriodType;
        ExpirationType IRecurringTransaction.ExpirationType => ExpirationType;
        DateTime? IRecurringTransaction.UntilDate => UntilDate;

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;

        MonthDay IRecurringTransaction.MonthDay => MonthDay;

        bool IHasCustomTheme.CustomTheme { get => HasPurchaseInvoiceCustomTheme; set => HasPurchaseInvoiceCustomTheme = value; }
        Guid? IHasCustomTheme.CustomThemeId { get => PurchaseInvoiceCustomTheme; set => PurchaseInvoiceCustomTheme = value; }

        /*
        [ProtoContract]
        public sealed class Line
        {
            [ProtoMember(1), Autocomplete(typeof(IPurchaseItem)), OnChange(nameof(Qty), nameof(IItem.DefaultQuantity)), OnChange(nameof(Description), nameof(Description)), OnChange(nameof(UnitPrice), nameof(Manager.Model.InventoryItem.PurchasePrice)), OnChange(nameof(TaxCode), nameof(IPurchaseItem.PurchaseItemTaxCode)), OnChange(nameof(Division), nameof(IPurchaseItem.PurchaseItemTrackingCode)), Short] public Guid? Item { get; set; }
            [ProtoMember(2), Autocomplete(typeof(IPurchaseInvoiceAccount), Subtext = nameof(BalanceSheetAccount.Group)), Substitute(nameof(Item), nameof(IPurchaseItem.PurchaseItemAccount)), OnChange(nameof(TaxCode), nameof(TaxCode))] public Guid? Account { get; set; }
            [ProtoMember(5), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsBillableExpense)), Autocomplete(typeof(Customer)), Prepend(nameof(Strings.Customer))] public Guid? BillableExpenseCustomer { get; set; }
            [ProtoMember(6), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsBillableExpense)), IfNotNull(nameof(BillableExpenseCustomer)), Autocomplete(typeof(SalesInvoice), Filter = nameof(BillableExpenseCustomer)), Prepend(nameof(Strings.Invoice)), Placeholder(nameof(Strings.Uninvoiced)), Short] public Guid? BillableExpenseSalesInvoice { get; set; }
            [ProtoMember(9), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), Autocomplete(typeof(CapitalAccount), Filter = nameof(Account)), Prepend(nameof(Strings.CapitalAccount))] public Guid? CapitalAccount { get; set; }
            [ProtoMember(10), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForCapitalAccounts)), IfNotNull(nameof(CapitalAccount)), Autocomplete(typeof(SubAccount)), Prepend(nameof(Strings.SubAccount))] public Guid? SubAccount { get; set; }
            [ProtoMember(11), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsEmployeeClearingAccount)), Autocomplete(typeof(Employee), Filter = nameof(Account)), Prepend(nameof(Strings.Employee))] public Guid? Employee { get; set; }
            [ProtoMember(13), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForSpecialAccounts)), Autocomplete(typeof(SpecialAccount), Filter = nameof(Account)), Prepend(nameof(Strings.SpecialAccount))] public Guid? SpecialAccount { get; set; }
            [ProtoMember(14), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForFixedAssets)), Autocomplete(typeof(FixedAsset), Filter = nameof(Account)), Prepend(nameof(Strings.FixedAsset))] public Guid? FixedAsset { get; set; }
            [ProtoMember(15), NoLabel, IfTrue(nameof(Account), nameof(IGeneralLedgerAccount.IsControlAccountForIntangibleAssets)), Autocomplete(typeof(IntangibleAsset), Filter = nameof(Account)), Prepend(nameof(Strings.IntangibleAsset))] public Guid? IntangibleAsset { get; set; }
            [ProtoMember(17), IfTrue(nameof(LineDescription)), Textarea] public string Description { get; set; }
            [ProtoMember(25)] public Dictionary<Guid, string> CustomFields { get; set; }
            [ProtoMember(18), AppendValue(nameof(Item), nameof(Manager.Model.InventoryItem.UnitName)), Short] public decimal? Qty { get; set; }
            [ProtoMember(19), NoPlaceholder, AppendCurrency(nameof(Supplier))] public decimal UnitPrice { get; set; }
            [ProtoMember(20), IfDifferentCurrency, NoPlaceholder] public decimal CurrencyAmount { get; set; }
            [ProtoMember(23), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.Percentage), Short, Append("%")] public decimal DiscountPercentage { get; set; }
            [ProtoMember(24), Label(nameof(Strings.Discount)), IfTrue(nameof(Discount)), IfEnum(nameof(DiscountType), (int)DiscountType.ExactAmount)] public decimal DiscountAmount { get; set; }
            [Label(nameof(Strings.Total)), Expression(Zero, Plus, nameof(UnitPrice), Times, nameof(Qty), Minus, nameof(DiscountAmount), TimesPercentage, nameof(DiscountPercentage), Round), Sum, AppendCurrency(nameof(PurchaseOrder.Supplier))] public object TotalBeforeTax { get; }
            [ProtoMember(21), Autocomplete(typeof(TaxCode)), Short] public Guid? TaxCode { get; set; }
            [IfTaxCodes, Expression(Zero, Plus, nameof(TotalBeforeTax), TimesTaxCode, nameof(TaxCode)), Sum, IfFalse(nameof(AmountsIncludeTax))] public object TaxAmount { get; }
            [IfTaxCodes, Expression(Zero, Plus, nameof(TotalBeforeTax), Plus, nameof(TaxAmount), Round), Sum, AppendCurrency(nameof(Supplier)), IfFalse(nameof(SalesOrder.AmountsIncludeTax))] public object Total { get; }
            [ProtoMember(26), Autocomplete(typeof(Project)), Short] public Guid? Project { get; set; }
            [ProtoMember(22), Autocomplete(typeof(Division)), IfTrue(nameof(Account), nameof(NamedObject.DivisionEnabled)), Short] public Guid? Division { get; set; }
        }
        */

        [ProtoMember(5)] public string Obsolete_Notes { get; set; }
        [ProtoMember(2)] public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines { get; set; }

        public DateTime? GetNextInvoiceDate()
        {
            if (!NextIssueDate.HasValue) return null;
            if (ExpirationType == ExpirationType.Custom && UntilDate.HasValue && UntilDate.Value < NextIssueDate.Value) return null;
            return NextIssueDate.Value;
        }
    }
}
