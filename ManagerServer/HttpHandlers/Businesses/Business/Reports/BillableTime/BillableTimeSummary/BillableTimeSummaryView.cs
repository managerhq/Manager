using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.BillableTimeSummary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.BillableTimeSummary
{
    [ProtoContract]
    [Title(nameof(Strings.BillableTimeSummary))]
    [Guide("The Billable Time Summary report shows movements in billable time balances.")]
    [Guide("It tracks new billable time, invoiced amounts, and write-offs by customer.")]
    [LinkGuide("For more information see:", typeof(BillableTimeSummaryForm))]
    internal sealed class BillableTimeSummaryView : DefaultView<GetBillableTimeSummaryView>
    {
    }
}