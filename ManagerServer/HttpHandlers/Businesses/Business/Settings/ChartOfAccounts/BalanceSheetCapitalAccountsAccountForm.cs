using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ChartOfAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.Account), nameof(Strings.CapitalAccounts))]
    [Guide("This form allows to rename built-in `CapitalAccounts` account.")]
    [Guide("To access this form, go to `Settings`, then `ChartOfAccounts`, then click `Edit` button for `CapitalAccounts` account.")]
    [Guide("The form contains the following fields:")]
    [Fields(typeof(BalanceSheetCapitalAccountsAccount))]
    [Guide("Click `Update` button to save your changes.")]
    [Guide("This account cannot be deleted, it is automatically added to your `ChartOfAccounts` when you have created at least one capital account.")]
    [LinkGuide("For more information see:", typeof(CapitalAccounts.CapitalAccounts))]
    internal sealed class BalanceSheetCapitalAccountsAccountForm : NakedVueForm<BalanceSheetCapitalAccountsAccount>
    {
    }
}