using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ChartOfAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.Account), nameof(Strings.Billable_time_invoiced))]
    [Guide("This form allows to rename built-in `Billable_time_invoiced` account.")]
    [Guide("To access this form, go to `Settings`, then `ChartOfAccounts`, then click `Edit` button for `Billable_time_invoiced` account.")]
    [Guide("The form contains the following fields:")]
    [Fields(typeof(ProfitAndLossStatementAccountBillableTimeInvoiced))]
    [Guide("Click `Update` button to save your changes.")]
    [Guide("This account cannot be deleted, it is automatically added to your `ChartOfAccounts` when you have recorded at least one billable time.")]
    [LinkGuide("For more information see:", typeof(BillableTime.BillableTime))]
    internal sealed class ProfitAndLossStatementAccountBillableTimeInvoicedForm : NakedVueForm<ProfitAndLossStatementAccountBillableTimeInvoiced>
    {
    }
}