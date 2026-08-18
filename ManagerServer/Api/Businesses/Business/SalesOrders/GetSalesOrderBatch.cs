using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.SalesOrders
{
    [ProtoContract]
    internal sealed class GetSalesOrderBatch : GetObjectBatchEndpoint<Model.SalesOrder, GetSalesOrder, PostSalesOrder, PutSalesOrder, DeleteSalesOrder>
    {
    }
}
