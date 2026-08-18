using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryQuantityByLocation
{
    [ProtoContract]
    internal sealed class GetInventoryQuantityByLocationBatch : GetObjectBatchEndpoint<Model.InventoryQuantityByLocation, GetInventoryQuantityByLocation, PostInventoryQuantityByLocation, PutInventoryQuantityByLocation, DeleteInventoryQuantityByLocation>
    {
    }
}
