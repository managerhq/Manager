using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryQuantitySummary
{
    [ProtoContract]
    internal sealed class GetInventoryQuantitySummaryBatch : GetObjectBatchEndpoint<Model.InventoryQuantitySummary, GetInventoryQuantitySummary, PostInventoryQuantitySummary, PutInventoryQuantitySummary, DeleteInventoryQuantitySummary>
    {
    }
}
