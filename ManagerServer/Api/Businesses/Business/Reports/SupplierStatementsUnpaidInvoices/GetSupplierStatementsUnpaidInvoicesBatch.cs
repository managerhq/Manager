using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.SupplierStatementsUnpaidInvoices
{
    [ProtoContract]
    internal sealed class GetSupplierStatementsUnpaidInvoicesBatch : GetObjectBatchEndpoint<Model.SupplierStatementsUnpaidInvoices, GetSupplierStatementsUnpaidInvoices, PostSupplierStatementsUnpaidInvoices, PutSupplierStatementsUnpaidInvoices, DeleteSupplierStatementsUnpaidInvoices>
    {
    }
}
