using System.Linq;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Employees
{
    [ProtoContract]
    [Title(nameof(Strings.Employees), nameof(Strings.AmountToPay))]
    [Guide("The **Amount to Pay** screen displays a summary of all outstanding amounts owed to employees in your organization.")]
    [Guide("This comprehensive view includes unpaid wages, salary payments, expense reimbursements, and any other amounts payable to employees that have been recorded but not yet paid.")]
    [Header("Understanding the Display")]
    [Guide("Each employee with an outstanding balance is listed along with the total amount owed to them in their designated currency.")]
    [Guide("The amounts shown represent the current balance of all unpaid transactions for each employee.")]
    [Guide("Only employees with outstanding balances appear on this screen.")]
    [Header("Making Payments")]
    [Guide("To pay an employee, click the **New Payment** button at the top of the screen to create a payment transaction that will reduce the outstanding balance.")]
    [Guide("The payment will be linked to the employee and automatically update their balance.")]
    [Header("Viewing Transaction Details")]
    [Guide("Click on any amount to view the detailed transactions that make up the employee's outstanding balance.")]
    [Guide("This will show you all unpaid *payslips*, *expense claims*, and other transactions contributing to the total amount owed.")]
    internal sealed class EmployeesAmountToPay : ObjectTable<EmployeesAmountToPay.Record>
    {
        protected override ManagerComponents.HeaderButton GetPrimaryButton()
        {
            return new ManagerComponents.HeaderButton()
            {
                Text = Strings.NewPayment,
                Url = new Payments.PaymentForm() { Business = Business, EmployeeClearingAccount = true, Referrer = this.ToUrl() }.ToUrl()
            };
        }

        protected override BusinessTemplate GetEdit(Record o, string referrer)
        {
            return new EmployeeForm() { Business = Business, Key = o.Employee.Key, Referrer = this.ToUrl() };
        }

        protected override Record[] GetObjects()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.GeneralLedgerAccount.IsEmployeeClearingAccount)
                .GroupBy(x => x.Employee)
                .Select(x => new Record()
                {
                    Employee = x.Key,
                    AmountToPay = x.Sum(y => y.AccountAmount) * -1m,
                    Currency = x.First().AccountCurrency
                })
                .Where(x => x.AmountToPay > 0m)
                .ToArray();            
        }

        protected override bool IsInactive(Record row) => row.Employee.IsInactive();

        [Guid("3bee96ff-2f47-43b4-99b9-9977dc182880")]
        public ManagerServer.Model.Employee GetEmployee(Record o) => o.Employee;

        [Right, Sum, Bold, WhitespaceNoWrap]
        [Guid("c0414e34-bc88-48f6-a4e5-f379245c0af6")]
        public Tuple<decimal, string, string> GetAmountToPay(Record o)
        {
            return new Tuple<decimal, string, string>(o.AmountToPay, o.AmountToPay.ToCurrencyString(o.Currency, CurrencySymbol.Short), new EmployeeTransactions() { Business = Business, Employee = o.Employee.Key, Referrer = this.ToUrl() }.ToUrl());
        }

        public sealed class Record
        {
            public ManagerServer.Model.Employee Employee;
            public decimal AmountToPay;
            public ManagerServer.Model.Currency Currency;
        }
    }
}