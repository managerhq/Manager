using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.InventoryItems
{
    [ProtoContract]
    internal sealed class GetInventoryItemBatch : GetObjectBatchEndpoint<Model.InventoryItem, GetInventoryItem, PostInventoryItem, PutInventoryItem, DeleteInventoryItem>
    {
    }
}
