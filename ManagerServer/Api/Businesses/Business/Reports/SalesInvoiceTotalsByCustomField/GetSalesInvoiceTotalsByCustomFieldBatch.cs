using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.SalesInvoiceTotalsByCustomField
{
    [ProtoContract]
    internal sealed class GetSalesInvoiceTotalsByCustomFieldBatch : GetObjectBatchEndpoint<Model.SalesInvoiceTotalsByCustomField, GetSalesInvoiceTotalsByCustomField, PostSalesInvoiceTotalsByCustomField, PutSalesInvoiceTotalsByCustomField, DeleteSalesInvoiceTotalsByCustomField>
    {
    }
}
