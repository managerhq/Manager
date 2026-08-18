using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryValueSummary
{
    [ProtoContract]
    internal sealed class GetInventoryValueSummaryBatch : GetObjectBatchEndpoint<Model.InventoryValueSummary, GetInventoryValueSummary, PostInventoryValueSummary, PutInventoryValueSummary, DeleteInventoryValueSummary>
    {
    }
}
