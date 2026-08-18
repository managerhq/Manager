using System;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Payslips
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("b786c6d7-455c-4b95-8bfb-ae7002124998")]
    [Title(nameof(Strings.Payslips))]
    [Guide("The `Payslips` tab helps you manage employee payroll and generate detailed payment records. Use this tab to create payslips that document earnings, deductions, and employer contributions for each pay period.")]
    [Guide("Payslips serve as official records of employee compensation and are essential for maintaining accurate payroll records, tax compliance, and providing employees with documentation of their earnings.")]
    [TabScreenshot("fa-money-check-edit", nameof(Strings.Payslips))]
    [Guide("To create a new payslip, click the `New Payslip` button.")]
    [HeroButtonScreenshot(nameof(Strings.Payslips), nameof(Strings.NewPayslip))]
    [Guide("The `Payslips` tab displays the following columns:")]
    [Columns]
    internal sealed class Payslips : NakedObjectsWithAutomaticRows<Payslip>
    {
        [ProtoMember(1)] public Guid? Employee;

        protected override Payslip[] OnGetRows(Payslip[] rows)
        {
            if (Employee.HasValue) rows = rows.Where(x => x.employee == Employee).ToArray();
            return rows;
        }

        [Default]
        [WarnIfFutureDate]
        [Center, MinWidth]
        [WhitespaceNoWrap]
        [Guid("1014c84d-7688-4ead-b54a-7f358dffc95a")]
        [Guide("The date when the payslip is issued or when the pay period ends.")]
        [Guide("This date determines when the payroll expense is recorded in your accounting records and affects financial reporting for the period.")]
        [Guide("Future dates will trigger a warning, as payslips are typically processed for current or past pay periods only.")]
        public DateTime[] GetDate(ManagerServer.Model.Payslip[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [WarnIfNotUnique]
        [PaddedSorting]
        [Guid("d852b671-7007-4464-9ca5-8aa466f86c96")]
        [Guide("A unique reference number or identifier for the payslip.")]
        [Guide("This reference helps you identify and track individual payslips in your records. It should be unique to avoid confusion when searching for or referencing specific payslips.")]
        public string[] GetReference(ManagerServer.Model.Payslip[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("23812877-51e7-4865-bad7-63faead6c401")]
        [Guide("The employee who is receiving this payslip. This field displays the employee's name as configured in the `Employees` tab.")]
        public string[] GetEmployee(ManagerServer.Model.Payslip[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Employee>(x.employee)?.GetCodeAndName()).ToArray();
        }

        [Guid("f41ef10a-7874-4346-bf7a-6df63768dbc5")]
        [Guide("An optional description or note for the payslip.")]
        [Guide("Use this field to add relevant details about the pay period, special circumstances, or any other information that should be documented with this payslip.")]
        public string[] GetDescription(ManagerServer.Model.Payslip[] rows)
        {
            return rows.Select(x => x.description).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guid("9562d7e3-39fa-47b1-be26-79fcb778bf91")]
        [Guide("The total amount of all earnings before any deductions.")]
        [Guide("This includes the employee's base salary plus any additional earnings such as overtime pay, bonuses, commissions, allowances, or other compensation.")]
        [Guide("The gross pay represents the total cost of the employee's earnings before taxes and other deductions are applied.")]
        public decimal[] GetGrossPay(ManagerServer.Model.Payslip[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).Where(x => x.IsPayslipEarningsLine).Sum(x => x.TransactionAmount)).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guid("0eaa365c-ab14-4540-bc7a-5c4362e9a294")]
        [Guide("The total amount of all deductions subtracted from the employee's gross pay.")]
        [Guide("Common deductions include income tax withholdings, social security contributions, health insurance premiums, retirement plan contributions, and other employee-paid benefits or obligations.")]
        [Guide("These amounts are withheld from the employee's gross pay and typically remitted to government agencies, insurance companies, or other third parties.")]
        public decimal[] GetDeduction(ManagerServer.Model.Payslip[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).Where(x => x.IsPayslipDeductionLine).Sum(x => x.TransactionAmount)*-1m).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("d4aa671b-8e53-4144-8457-b4b523600d57")]
        [Guide("The amount the employee actually receives after all deductions have been subtracted from gross pay.")]
        [Guide("Net pay is calculated as `Gross Pay` minus `Deductions` and represents the employee's take-home pay.")]
        [Guide("When the payslip is created, the employee's balance in the `Employees` tab is automatically increased by this net pay amount, reflecting the liability owed to the employee.")]
        public Tuple<decimal, ManagerServer.Model.Currency>[] GetNetPay(ManagerServer.Model.Payslip[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.GetReversedTransactionAmountWithCurrency()).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guid("ed761e09-87e7-4a23-9f37-f83c899fd138")]
        [Guide("The total amount of employer contributions made on behalf of the employee.")]
        [Guide("These are additional costs paid by the employer beyond the employee's gross pay, such as employer pension contributions, employer-paid health insurance premiums, or employer social security contributions.")]
        [Guide("Contributions do not affect the employee's net pay but represent additional employment costs for the business. They are recorded as expenses in your accounting records.")]
        public decimal[] GetContribution(ManagerServer.Model.Payslip[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).Where(x => x.IsPayslipContributionLine).Sum(x => x.TransactionAmount)).ToArray();
        }
    }
}
