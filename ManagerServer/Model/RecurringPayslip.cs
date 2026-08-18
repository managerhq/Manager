using System;
using System.Linq;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [Guide("Recurring payslips automate payroll processing by generating employee payslips on a regular schedule.")]
    [Guide("Use recurring payslips for salaried employees with consistent pay, hourly employees with regular hours, or any predictable payroll pattern.")]
    [Guide("The system creates payslips automatically, calculating earnings, deductions, and net pay based on your template settings.")]
    [CustomFields]
    [ProtoContract]
    [Guid("ae6e14e3-1d3d-4996-b466-ba41732a8dbe")]
    [Currency(nameof(employee))]
    public sealed class RecurringPayslip : Object, IRecurringTransactionFor<Payslip>, ICustomFields
    {
        [Guide("The date when the next payslip will be automatically created. This date advances automatically based on your pay cycle.")]
        [Guide("Set this to your next pay date. The system checks daily for payslips due to be created.")]
        [ProtoMember(9), NoWrap] public DateTime? nextIssueDate { get; set; }
        [Guide("The frequency interval for creating payslips. This number works with the period type to match your pay cycle.")]
        [Guide("Common examples: 1 Week = weekly pay, 2 Weeks = fortnightly/bi-weekly pay, 1 Month = monthly salary.")]
        [ProtoMember(8), NoWrap, Placeholder("1")] public int? Interval { get; set; }
        [Guide("The period type that works with the interval to determine frequency. Select the unit of time for your pay cycle.")]
        [Guide("Most businesses pay weekly, fortnightly, or monthly. Choose what matches your payroll schedule.")]
        [ProtoMember(7), NoWrap, EmptyLabel] public Period PeriodType { get; set; }
        [Guide("For monthly recurring payslips, specify which day of the month the payslip should be created.")]
        [Guide("Common choices are 'Last day of month' for month-end payroll or specific dates like the 15th or 25th.")]
        [ProtoMember(22), EmptyLabel, IfEnum(nameof(PeriodType), (int)Period.Month)] public MonthDay MonthDay { get; set; }
        [Guide("Select the employee who will receive this recurring payslip. Each generated payslip will be linked to this employee.")]
        [Guide("Create separate recurring payslips for each employee, even if they have the same pay schedule.")]
        [ProtoMember(1), Autocomplete(typeof(Employee))] public Guid? employee { get; set; }
        [Guide("A description that identifies this recurring payslip and will be copied to each generated payslip.")]
        [Guide("Optional but helpful for identifying pay periods. Examples: 'Regular salary', 'Fortnightly wages', 'Monthly pay'.")]
        [ProtoMember(12), Long, Placeholder(nameof(Strings.Optional))] public string description { get; set; }
        [Guide("The earnings items to include in each payslip. These are amounts added to the employee's gross pay.")]
        [Guide("Add lines for regular wages, overtime, bonuses, allowances, or any other earnings. Each line can have different rates or fixed amounts.")]
        [Guide("For variable earnings like overtime hours, you'll need to edit individual payslips after creation.")]
        [ProtoMember(2), FirstColumnLabel] public ManagerServer.Model.Payslip.Earned[] Earnings { get; set; }
        [Guide("The deduction items to subtract from gross earnings. These reduce the employee's net pay.")]
        [Guide("Common deductions include income tax, social security, health insurance, retirement contributions, or loan repayments.")]
        [Guide("Deductions can be fixed amounts or calculated as percentages of earnings.")]
        [ProtoMember(3), FirstColumnLabel] public ManagerServer.Model.Payslip.Deduction[] Deductions { get; set; }
        [Guide("The employer contribution items that don't affect the employee's net pay but represent additional employer costs.")]
        [Guide("These might include employer retirement contributions, employer-paid insurance, or payroll taxes paid by the employer.")]
        [Guide("Contributions appear on the payslip for transparency but don't reduce take-home pay.")]
        [ProtoMember(4), FirstColumnLabel] public ManagerServer.Model.Payslip.Contribution[] Contributions { get; set; }
        [ProtoMember(14)] public bool ShowTotalsForThePeriod { get; set; }
        [ProtoMember(15), IfTrue(nameof(ShowTotalsForThePeriod)), NoLabel, PrependAttribute(nameof(Strings.FromDate))] public DateTime? TotalsPeriodStart { get; set; }
        [ProtoMember(10), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(11), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? Theme { get; set; }
        [ProtoMember(20), Label(nameof(Strings.CustomTitle))] public bool HasPayslipCustomTitle { get; set; }
        [ProtoMember(21), NoLabel, IfTrue(nameof(HasPayslipCustomTitle)), Placeholder(nameof(Strings.Payslip))] public string PayslipCustomTitle { get; set; }
        [ProtoMember(17), Label(nameof(Strings.Footers))] public bool HasPayslipFooters { get; set; }
        [ProtoMember(18), Autocomplete(typeof(ManagerServer.Model.PayslipFooter)), NoLabel, IfTrue(nameof(HasPayslipFooters))] public Guid[] PayslipFooters { get; set; }
        [Guide("When enabled, the system automatically assigns sequential reference numbers to each generated payslip.")]
        [Guide("Recommended for maintaining organized payroll records. Reference numbers help track payments and resolve queries.")]
        [ProtoMember(16)] public bool AutomaticReference { get; set; }
        [ProtoMember(6)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(19)] public CustomFields CustomFields2 { get; set; }

        // IRecurringTransaction
        DateTime? IRecurringTransaction.NextIssueDate { get => nextIssueDate; set => nextIssueDate = value; }
        int? IRecurringTransaction.Interval => Interval;
        Period IRecurringTransaction.PeriodType => PeriodType;
        ExpirationType IRecurringTransaction.ExpirationType => ExpirationType.UntilFurtherNotice;
        DateTime? IRecurringTransaction.UntilDate => null;

        [ProtoMember(5)] public string Obsolete_Notes { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;

        MonthDay IRecurringTransaction.MonthDay => MonthDay;
    }
}