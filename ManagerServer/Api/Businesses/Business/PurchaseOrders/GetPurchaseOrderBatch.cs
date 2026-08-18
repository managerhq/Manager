using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.PurchaseOrders
{
    [ProtoContract]
    internal sealed class GetPurchaseOrderBatch : GetObjectBatchEndpoint<Model.PurchaseOrder, GetPurchaseOrder, PostPurchaseOrder, PutPurchaseOrder, DeletePurchaseOrder>
    {
    }
}
