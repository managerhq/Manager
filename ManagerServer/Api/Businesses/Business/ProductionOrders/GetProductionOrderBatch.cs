using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.ProductionOrders
{
    [ProtoContract]
    internal sealed class GetProductionOrderBatch : GetObjectBatchEndpoint<Model.ProductionOrder, GetProductionOrder, PostProductionOrder, PutProductionOrder, DeleteProductionOrder>
    {
    }
}
