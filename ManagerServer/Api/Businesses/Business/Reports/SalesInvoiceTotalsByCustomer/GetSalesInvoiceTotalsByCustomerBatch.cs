using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.SalesInvoiceTotalsByCustomer
{
    [ProtoContract]
    internal sealed class GetSalesInvoiceTotalsByCustomerBatch : GetObjectBatchEndpoint<Model.SalesInvoiceTotalsByCustomer, GetSalesInvoiceTotalsByCustomer, PostSalesInvoiceTotalsByCustomer, PutSalesInvoiceTotalsByCustomer, DeleteSalesInvoiceTotalsByCustomer>
    {
    }
}
