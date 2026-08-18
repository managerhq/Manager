using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.SalesInvoiceTotalsByItem
{
    [ProtoContract]
    internal sealed class GetSalesInvoiceTotalsByItemBatch : GetObjectBatchEndpoint<Model.SalesInvoiceTotalsByItem, GetSalesInvoiceTotalsByItem, PostSalesInvoiceTotalsByItem, PutSalesInvoiceTotalsByItem, DeleteSalesInvoiceTotalsByItem>
    {
    }
}
