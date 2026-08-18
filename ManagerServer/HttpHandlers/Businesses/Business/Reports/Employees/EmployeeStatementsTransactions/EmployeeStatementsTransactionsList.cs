using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.EmployeeStatementsTransactions
{
    [ProtoContract]
    [Title(nameof(Strings.EmployeeStatementsTransactions))]
    [Guide("*Employee Statements - Transactions* provides a detailed overview of all transactions between your business and its employees, helping you track payslips, expense claims, payments, and any other entries that affect each employee's clearing account balance.")]
    [Guide("This report shows a complete transaction history for each employee, including payslips issued, expense claims submitted, payments made, and any other transactions that affect amounts owed to or by the employee.")]
    [Guide("To create a new employee statement report, go to the **Reports** tab, click **Employee Statements - Transactions**, then click the **New Report** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.EmployeeStatementsTransactions), name: nameof(Strings.NewReport))]
    internal sealed class EmployeeStatementsTransactionsList : Table<EmployeeStatementsTransactionsList.Record>
    {
        protected override ManagerComponents.HeaderButton GetPrimaryButton()
        {
            return new ManagerComponents.HeaderButton()
            {
                Text = Strings.SetPeriod,
                Url = new EmployeeStatementsTransactionsForm() { Business = Business, Referrer = this.ToUrl(), Key = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.EmployeeStatementsTransactions)) }.ToUrl()
            };
        }

        protected override Record[] GetObjects()
        {
            var from = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmployeeStatementsTransactions>().FromDate;
            var to = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmployeeStatementsTransactions>().GetToDate();

            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.Date <= to && x.GeneralLedgerAccount.IsEmployeeClearingAccount)
                .GroupBy(x => x.Employee)
                .Where(x => x.Any(y => y.Date >= from))
                .OrderBy(x => x.Key.NameWithCode)
                .Select(x => new Record()
                {
                    FromDate = from,
                    ToDate = to,
                    Employee = x.Key,
                    Transactions = x.Count(y => y.Date >= from),
                    Balance = new Tuple<decimal, Currency>(-x.Sum(y => y.AccountAmount), x.First().AccountCurrency)
                }).ToArray();
        }

        protected override BusinessTemplate GetView(Record o, string referrer)
        {
            return new EmployeeStatementsTransactionsView()
            {
                Business = Business,
                Key = o.Employee.Key,
                Referrer = referrer
            };
        }

        public record Record
        {
            [Center, WhitespaceNoWrap, MinWidth]
            public DateTime FromDate { get; set; }

            [Center, WhitespaceNoWrap, MinWidth]
            public DateTime ToDate { get; set; }

            public ManagerServer.Model.Employee Employee { get; set; }

            [Center, WhitespaceNoWrap, MinWidth, Sum, TabularNums]
            public int Transactions { get; set; }

            [Bold, Right, WhitespaceNoWrap, MinWidth, Sum, TabularNums]
            public Tuple<decimal, Currency> Balance { get; set; }
        }
    }
}
