using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxSummary
{
    [ProtoContract]
    internal sealed class GetTaxSummaryBatch : GetObjectBatchEndpoint<Model.TaxSummary, GetTaxSummary, PostTaxSummary, PutTaxSummary, DeleteTaxSummary>
    {
    }
}
