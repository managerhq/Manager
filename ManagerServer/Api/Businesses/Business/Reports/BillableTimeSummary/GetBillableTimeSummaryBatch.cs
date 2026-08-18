using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.BillableTimeSummary
{
    [ProtoContract]
    internal sealed class GetBillableTimeSummaryBatch : GetObjectBatchEndpoint<Model.BillableTimeSummary, GetBillableTimeSummary, PostBillableTimeSummary, PutBillableTimeSummary, DeleteBillableTimeSummary>
    {
    }
}
