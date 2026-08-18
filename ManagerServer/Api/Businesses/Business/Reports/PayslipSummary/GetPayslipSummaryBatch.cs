using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.PayslipSummary
{
    [ProtoContract]
    internal sealed class GetPayslipSummaryBatch : GetObjectBatchEndpoint<Model.PayslipSummary, GetPayslipSummary, PostPayslipSummary, PutPayslipSummary, DeletePayslipSummary>
    {
    }
}
