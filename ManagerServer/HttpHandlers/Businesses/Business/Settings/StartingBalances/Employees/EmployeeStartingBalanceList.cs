using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.Employees
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Employees))]
    [Guid("8a0278fe-c373-41d1-853d-bd46ebf35605")]
    [Title(nameof(Strings.StartingBalances), nameof(Strings.Employees))]
    [Guide("This screen allows you to set up starting balances for employees you have created under the **Employees** tab.")]
    [Guide("Starting balances represent the amounts owed to or by employees at the beginning of your record-keeping in this system.")]
    [Guide("To create a new starting balance for an employee, click the **New Starting Balance** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.Employees), name: nameof(Strings.NewStartingBalance))]
    [Guide("You will be taken to the starting balance entry form where you can enter the details for the selected employee.")]
    [LinkGuide("For more information, see:", typeof(EmployeeStartingBalanceForm))]
    internal sealed class EmployeeStartingBalanceList : NakedObjectsWithAutomaticRows<EmployeeStartingBalance>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewStartingBalance);
        }

        [Default]
        [Guid("2e0dd796-ae08-43a9-8ca5-ad3df160e85e")]
        public NamedObject[] GetEmployee(EmployeeStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Employee>(x.Employee)).ToArray();
        }

        [Default, Right, Bold, Sum]
        [Guid("1d31846b-4a7c-4111-9500-10f0820afc46")]
        public Tuple<decimal, Currency>[] GetBalance(EmployeeStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).SingleOrDefault(x => !x.IsBalancing)?.GetTransactionAmountWithCurrency()).ToArray();
        }
    }
}