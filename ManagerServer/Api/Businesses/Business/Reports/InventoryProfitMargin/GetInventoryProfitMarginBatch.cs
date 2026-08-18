using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryProfitMargin
{
    [ProtoContract]
    internal sealed class GetInventoryProfitMarginBatch : GetObjectBatchEndpoint<Model.InventoryProfitMargin, GetInventoryProfitMargin, PostInventoryProfitMargin, PutInventoryProfitMargin, DeleteInventoryProfitMargin>
    {
    }
}
