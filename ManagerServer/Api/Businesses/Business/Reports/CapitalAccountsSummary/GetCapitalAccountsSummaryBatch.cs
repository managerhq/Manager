using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.CapitalAccountsSummary
{
    [ProtoContract]
    internal sealed class GetCapitalAccountsSummaryBatch : GetObjectBatchEndpoint<Model.CapitalAccountsSummary, GetCapitalAccountsSummary, PostCapitalAccountsSummary, PutCapitalAccountsSummary, DeleteCapitalAccountsSummary>
    {
    }
}
