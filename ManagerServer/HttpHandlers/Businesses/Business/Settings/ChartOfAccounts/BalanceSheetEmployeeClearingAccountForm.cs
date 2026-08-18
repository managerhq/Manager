using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ChartOfAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.Account), nameof(Strings.EmployeeClearingAccount))]
    [Guide("This form allows to rename built-in `EmployeeClearingAccount` account.")]
    [Guide("To access this form, go to `Settings`, then `ChartOfAccounts`, then click `Edit` button for `EmployeeClearingAccount` account.")]
    [Guide("The form contains the following fields:")]
    [Fields(typeof(BalanceSheetEmployeeClearingAccount))]
    [Guide("Click `Update` button to save your changes.")]
    [Guide("This account cannot be deleted, it is automatically added to your `ChartOfAccounts` when you have created at least one employee.")]
    [LinkGuide("For more information see:", typeof(Employees.Employees))]
    internal sealed class BalanceSheetEmployeeClearingAccountForm : NakedVueForm<BalanceSheetEmployeeClearingAccount>
    {
    }
}