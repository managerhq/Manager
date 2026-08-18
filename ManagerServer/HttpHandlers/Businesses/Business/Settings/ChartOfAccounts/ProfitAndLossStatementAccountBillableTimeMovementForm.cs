using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ChartOfAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.Account), nameof(Strings.BillableTime_Movement))]
    [Guide("This form allows to rename built-in `BillableTime_Movement` account.")]
    [Guide("To access this form, go to `Settings`, then `ChartOfAccounts`, then click `Edit` button for `BillableTime_Movement` account.")]
    [Guide("The form contains the following fields:")]
    [Fields(typeof(ProfitAndLossStatementAccountBillableTimeMovement))]
    [Guide("Click `Update` button to save your changes.")]
    [Guide("This account cannot be deleted, it is automatically added to your `ChartOfAccounts` when you have recorded at least one billable time.")]
    [LinkGuide("For more information see:", typeof(BillableTime.BillableTime))]
    internal sealed class ProfitAndLossStatementAccountBillableTimeMovementForm : NakedVueForm<ProfitAndLossStatementAccountBillableTimeMovement>
    {
    }
}