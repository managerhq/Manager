using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.InventoryWriteOffs
{
    [ProtoContract]
    internal sealed class GetInventoryWriteOffBatch : GetObjectBatchEndpoint<Model.InventoryWriteOff, GetInventoryWriteOff, PostInventoryWriteOff, PutInventoryWriteOff, DeleteInventoryWriteOff>
    {
    }
}
