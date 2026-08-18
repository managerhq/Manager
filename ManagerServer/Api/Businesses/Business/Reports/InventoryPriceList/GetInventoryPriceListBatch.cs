using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryPriceList
{
    [ProtoContract]
    internal sealed class GetInventoryPriceListBatch : GetObjectBatchEndpoint<Model.InventoryPriceList, GetInventoryPriceList, PostInventoryPriceList, PutInventoryPriceList, DeleteInventoryPriceList>
    {
    }
}
