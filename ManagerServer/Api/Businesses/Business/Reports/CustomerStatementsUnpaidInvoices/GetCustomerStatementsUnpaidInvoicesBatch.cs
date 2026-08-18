using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.CustomerStatementsUnpaidInvoices
{
    [ProtoContract]
    internal sealed class GetCustomerStatementsUnpaidInvoicesBatch : GetObjectBatchEndpoint<Model.CustomerStatementsUnpaidInvoices, GetCustomerStatementsUnpaidInvoices, PostCustomerStatementsUnpaidInvoices, PutCustomerStatementsUnpaidInvoices, DeleteCustomerStatementsUnpaidInvoices>
    {
    }
}
