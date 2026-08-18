using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Payslips
{
    [ProtoContract]
    [Guid("99501034-2D94-4BC5-A469-9962575A133F")]
    [Title(nameof(Strings.Payslips))]
    [Guide("The **Pending Recurring Payslips** screen displays payslips that are scheduled for automatic creation based on your recurring payroll schedules.")]
    [Guide("This screen helps you review and manage upcoming payslips before they are automatically generated, ensuring accuracy and allowing you to make any necessary adjustments.")]
    [Guide("Each row in the table represents a recurring payslip that will be created on its scheduled date. The system will automatically generate these payslips based on the recurring templates you have set up.")]
    [Columns]
    internal sealed class PendingRecurringPayslips : NakedObjectsOfPendingRecurringTransactions<ManagerServer.Model.RecurringPayslip>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("25880637-A3E8-4E7D-A3CF-6BFBEF9EB9BE")]
        [Guide("Shows the scheduled date when this recurring payslip will be automatically generated. This date is calculated based on your recurring payslip settings.")]
        public DateTime?[] GetNextIssueDate(RecurringPayslip[] rows)
        {
            return rows.Select(x => x.nextIssueDate).ToArray();
        }

        [Default]
        [Guid("6900829A-F19A-4A5C-A45F-419E9F371C39")]
        [Guide("Shows the name of the employee who will receive this payslip. The employee name is displayed in the format configured in your employee settings.")]
        public string[] GetEmployee(RecurringPayslip[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Employee>(x.employee)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("7E4C710B-F3C1-46F7-9EDB-7A3EEA47F31C")]
        [Guide("Shows the description or reference for this payslip. This typically includes the pay period or other identifying information you have entered in the recurring payslip template.")]
        public string[] GetDescription(RecurringPayslip[] rows)
        {
            return rows.Select(x => x.description).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guid("8EDB5872-5032-4B32-9450-1FD075840423")]
        [Guide("Shows the total *gross pay* amount before any deductions are applied. This is the employee's total earnings for the pay period.")]
        [Guide("The *gross pay* includes all earnings such as regular wages, overtime pay, bonuses, commissions, and any other compensation configured in the payslip template.")]
        public Tuple<decimal, Currency>[] GetGrossPay(RecurringPayslip[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            var output = new List<Tuple<decimal, Currency>>();
            foreach (var e in rows)
            {
                var employee = database.SingleOrDefault<Employee>(e.employee);
                var currency = database.SingleOrDefault<ForeignCurrency>(employee?.Currency) as Currency ?? baseCurrency;
                var amount = 0m;
                if (e.Earnings != null) amount = e.Earnings.Sum(x => currency.Round((x.Units ?? 1m) * x.UnitPrice));
                output.Add(new Tuple<decimal, Currency>(amount, currency));
            }
            return output.ToArray();
        }

        [Default]
        [Right, Sum]
        [Guid("AC5682E0-6B41-4734-AAE3-59F94EA0170E")]
        [Guide("Shows the total amount of deductions that will be subtracted from the employee's *gross pay*.")]
        [Guide("Deductions typically include income tax withholdings, social security contributions, health insurance premiums, retirement plan contributions, and any other withholdings configured in the payslip template.")]
        public Tuple<decimal, Currency>[] GetDeduction(RecurringPayslip[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            var output = new List<Tuple<decimal, Currency>>();
            foreach (var e in rows)
            {
                var employee = database.SingleOrDefault<Employee>(e.employee);
                var currency = database.SingleOrDefault<ForeignCurrency>(employee?.Currency) as Currency ?? baseCurrency;
                var amount = 0m;
                if (e.Deductions != null) amount = e.Deductions.Sum(x => currency.Round(x.DeductionAmount));
                output.Add(new Tuple<decimal, Currency>(amount, currency));
            }
            return output.ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("0C1B2BBF-F2B6-45E2-BF42-4A1D1D09F5B0")]
        [Guide("Shows the *net pay* amount that the employee will actually receive after all deductions have been applied.")]
        [Guide("The *net pay* is calculated automatically as *gross pay* minus total deductions. This is the actual amount that will be paid to the employee.")]
        public Tuple<decimal, Currency>[] GetNetPay(RecurringPayslip[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            var output = new List<Tuple<decimal, Currency>>();
            foreach (var e in rows)
            {
                var employee = database.SingleOrDefault<Employee>(e.employee);
                var currency = database.SingleOrDefault<ForeignCurrency>(employee?.Currency) as Currency ?? baseCurrency;
                var earnings = 0m;
                var deduction = 0m;
                if (e.Earnings != null) earnings = e.Earnings.Sum(x => currency.Round((x.Units ?? 1m) * x.UnitPrice));
                if (e.Deductions != null) deduction = e.Deductions.Sum(x => currency.Round(x.DeductionAmount));
                output.Add(new Tuple<decimal, Currency>(earnings-deduction, currency));
            }
            return output.ToArray();
        }

        [Default]
        [Right, Sum]
        [Guid("6BDE5699-AFB6-4BF0-B978-036B40813A98")]
        [Guide("Shows the total amount of employer contributions that will be made on behalf of the employee.")]
        [Guide("Employer contributions are amounts paid by the employer in addition to the employee's wages. These typically include employer-paid portions of retirement plans, health insurance, workers' compensation, and other benefits. These amounts do not affect the employee's *net pay* but represent additional costs to the employer.")]
        public Tuple<decimal, Currency>[] GetContribution(RecurringPayslip[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            var output = new List<Tuple<decimal, Currency>>();
            foreach (var e in rows)
            {
                var employee = database.SingleOrDefault<Employee>(e.employee);
                var currency = database.SingleOrDefault<ForeignCurrency>(employee?.Currency) as Currency ?? baseCurrency;
                var amount = 0m;
                if (e.Contributions != null) amount = e.Contributions.Sum(x => currency.Round(x.ContributionAmount));
                output.Add(new Tuple<decimal, Currency>(amount, currency));
            }
            return output.ToArray();
        }
    }
}