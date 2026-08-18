using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.InventoryTransfers
{
    [ProtoContract]
    internal sealed class GetInventoryTransferBatch : GetObjectBatchEndpoint<Model.InventoryTransfer, GetInventoryTransfer, PostInventoryTransfer, PutInventoryTransfer, DeleteInventoryTransfer>
    {
    }
}
