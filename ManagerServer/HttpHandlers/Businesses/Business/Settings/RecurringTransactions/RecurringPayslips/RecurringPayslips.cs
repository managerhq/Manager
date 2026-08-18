using System.Linq;
using System.Collections.Generic;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringPayslips
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Payslips))]
    [Guid("900ab30a-c629-4859-a3a3-771c96a63ef7")]
    [Title(nameof(Strings.RecurringPayslips), nameof(Strings.Pending))]
    [Guide("Recurring payslips automate the creation of regular employee payslips, saving time and ensuring consistency in payroll processing.")]
    [Guide("When you set up a recurring payslip, the system will automatically generate a new payslip based on your specified schedule. This is ideal for employees with fixed salaries or regular pay structures.")]
    [Header("Overview")]
    [Guide("Each recurring payslip template contains all the information needed to generate payslips automatically, including earnings, deductions, and employer contributions.")]
    [Guide("The system tracks when each payslip is due and displays this information in the *Next Issue Date* column. You can review and modify recurring payslips at any time before they are generated.")]
    [Header("Setting Up Recurring Payslips")]
    [Guide("To create a new recurring payslip, click the **New Recurring Payslip** button. You will need to specify the employee, pay details, and recurrence schedule.")]
    [Guide("Recurring payslips can be set up for any frequency that matches your payroll cycle, such as weekly, fortnightly, or monthly.")]
    [Columns]
    internal sealed class RecurringPayslips : NakedObjectsWithAutomaticRows<RecurringPayslip>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("d528fef6-cb50-4cf3-b97d-13c7184b2744")]
        [Guide("Displays the scheduled date for the next automatic payslip generation. This date is calculated based on the recurrence pattern you have configured for each recurring payslip.")]
        public DateTime?[] GetNextIssueDate(RecurringPayslip[] rows)
        {
            return rows.Select(x => x.nextIssueDate).ToArray();
        }

        [Default]
        [Guid("3a77f87b-6a04-4dd5-a0a6-c4348557e593")]
        [Guide("Identifies the employee who will receive the automatically generated payslip. The employee must be set up in the **Employees** tab before creating a recurring payslip.")]
        public string[] GetEmployee(RecurringPayslip[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Employee>(x.employee)?.Name).ToArray();
        }

        [Default]
        [Guid("44c98392-cfe3-428e-960d-29aa0a55b9b8")]
        [Guide("Displays the description or reference for the recurring payslip. This helps you identify different recurring payslips, especially when an employee has multiple recurring payment arrangements.")]
        public string[] GetDescription(RecurringPayslip[] rows)
        {
            return rows.Select(x => x.description).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guid("f756a29a-2402-45b7-b3c2-3fb1a357d0e1")]
        [Guide("Shows the total *gross pay* amount before any deductions. This includes all earnings items such as salary, wages, bonuses, and allowances configured in the recurring payslip template.")]
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
        [Guid("417effa2-a097-4e7e-8e74-dae6bd9ddbe1")]
        [Guide("Shows the total amount of all deductions that will be subtracted from the *gross pay*. This includes items such as tax withholdings, social security contributions, health insurance, and other employee deductions.")]
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
        [Guid("f172f033-12b4-44f4-80fa-3ee4a3698dd2")]
        [Guide("Shows the *net pay* amount that the employee will receive after all deductions have been subtracted from the *gross pay*. This is the actual amount that will be paid to the employee.")]
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
                if (e.Earnings != null) earnings = e.Earnings.Sum(x => currency.Round((x.Units ?? 1m) * x.UnitPrice));
                var deduction = 0m;
                if (e.Deductions != null) deduction = e.Deductions.Sum(x => currency.Round(x.DeductionAmount));
                output.Add(new Tuple<decimal, Currency>(earnings - deduction, currency));
            }
            return output.ToArray();
        }

        [Default]
        [Right, Sum]
        [Guid("9cceb90f-be51-493c-aa1c-c9ac4ac20d16")]
        [Guide("Shows the total *employer contributions* that will be recorded when the payslip is generated. These are amounts paid by the employer on behalf of the employee, such as employer pension contributions or employer-paid insurance, which are not deducted from the employee's pay.")]
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