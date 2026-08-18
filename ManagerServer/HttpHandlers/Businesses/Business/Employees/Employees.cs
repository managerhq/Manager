using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;
using ManagerServer.Query.GeneralLedger;

namespace ManagerServer.HttpHandlers.Businesses.Business.Employees
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.Employees))]
    [Guid("3a6a37b6-b7c1-11ed-afa1-0242ac120002")]
    [Guide("The `Employees` tab helps you manage information for all employees within your business.")]
    [Guide("Use this tab to track employee details, monitor account balances, and view payment statuses.")]
    [Header("Getting Started")]
    [TabScreenshot("fa-id-card", nameof(Strings.Employees))]
    [Guide("To create a new employee, click the `New Employee` button.")]
    [HeroButtonScreenshot(nameof(Strings.Employees), nameof(Strings.NewEmployee))]
    [Header("Understanding the Employee List")]
    [Guide("The `Employees` tab displays the following columns:")]
    [Columns]
    internal sealed class Employees : NakedObjectsWithAutomaticRows<ManagerServer.Model.Employee>
    {
        [WarnIfNotUnique]
        [Guid("401e60d8-b7c1-11ed-afa1-0242ac120002")]
        [Guide("A unique identifier code for the employee. This can be an employee number, ID, or any custom code used by your organization to identify employees.")]
        public string[] GetCode(ManagerServer.Model.Employee[] rows)
        {
            return rows.Select(x => x.Code).ToArray();
        }

        [Default]
        [Guid("43a77e4c-b7c1-11ed-afa1-0242ac120002")]
        [Guide("The full name of the employee. This is typically their legal name as it appears on employment documents and will be displayed on payslips and reports.")]
        public string[] GetName(ManagerServer.Model.Employee[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Guid("6cd24450-b7c1-11ed-afa1-0242ac120002")]
        [Guide("The employee's email address used for work communications. This email can be used for sending electronic payslips and other employee-related documents.")]
        public string[] GetEmailAddress(ManagerServer.Model.Employee[] rows)
        {
            return rows.Select(x => x.Email).ToArray();
        }

        [Guid("4678f646-b7c1-11ed-afa1-0242ac120002")]
        [Guide("The control account associated with the employee. If custom control accounts are not in use, the default `Employee Clearing Account` will be displayed.")]
        public string[] GetControlAccount(ManagerServer.Model.Employee[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => (database.SingleOrDefault<ManagerServer.Model.ControlAccountForEmployees>(x.ControlAccount) as ManagerServer.Model.NamedObject ?? database.Single<ManagerServer.Model.BalanceSheetEmployeeClearingAccount>()).GetName()).ToArray();
        }

        [Guid("489d7fbe-b7c1-11ed-afa1-0242ac120002")]
        [Guide("The division to which the employee is assigned. This field is only applicable if divisional accounting is enabled in your business.")]
        public string[] GetDivision(ManagerServer.Model.Employee[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Division>(x.Division)?.Name).ToArray();
        }

        [Default]
        [Bold, Right, Sum]
        [Guid("adeb9c2a-b7c1-11ed-afa1-0242ac120002")]
        [Guide("Shows the current balance for each employee.")]
        [Guide("When you issue a `Payslip` to an employee, their balance increases. When you record a payment to the employee, their balance decreases.")]
        [Guide("A zero balance indicates the employee has been fully paid for all earnings.")]
        public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>[] GetBalance(ManagerServer.Model.Employee[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();
            var generalLedger = new GeneralLedger(Business);

            return [.. rows.Select(x => new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(generalLedger.GetAggregations().GetEmployeeCurrencyAmount(x.Key, DateTime.MinValue, DateTime.MaxValue)*-1m, database.SingleOrDefault<ManagerServer.Model.ForeignCurrency>(x.Currency) as ManagerServer.Model.Currency ?? baseCurrency, new EmployeeTransactions() { Employee = x.Key, Business = Business, Referrer = referrer }))];
        }

        [Default]
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("674430a6-b7c2-11ed-afa1-0242ac120002")]
        [Guide("Displays the payment status of each employee for quick reference.")]
        [Guide("The status indicator shows one of three possible states:")]
        [Guide("• `Paid` - The employee has a zero balance and is fully paid")]
        [Guide("• `Unpaid` - The employee has a positive balance and is owed money")]
        [Guide("• `Paid In Advance` - The employee has a negative balance from advance payments")]
        public EmployeeStatus[] GetStatus(ManagerServer.Model.Employee[] rows)
        {
            return [.. GetBalance(rows).Select(x => GetEmployeeStatusFromAmount(x.Item1))];
        }

        private EmployeeStatus GetEmployeeStatusFromAmount(decimal amount)
        {
            if (amount == 0) return EmployeeStatus.Paid;
            else if (amount > 0) return EmployeeStatus.Unpaid;
            else return EmployeeStatus.PaidInAdvance;
        }

        public enum EmployeeStatus
        {
            [Success] Paid,
            [Danger] Unpaid,
            [Primary] PaidInAdvance
        }

        protected override void OnFooterEndSection(Context context)
        {
            using (A(href: new EmployeesAmountToPay() { Business = Business }.ToUrl(), @class: "btn btn-xs")) Write(Strings.AmountToPay);

            base.OnFooterEndSection(context);
        }
    }
}
