using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxableSalesPerCustomer
{
    [ProtoContract]
    internal sealed class GetTaxableSalesPerCustomerBatch : GetObjectBatchEndpoint<Model.TaxableSalesPerCustomer, GetTaxableSalesPerCustomer, PostTaxableSalesPerCustomer, PutTaxableSalesPerCustomer, DeleteTaxableSalesPerCustomer>
    {
    }
}
