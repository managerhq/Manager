using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.SupplierStatementsUnpaidInvoices
{
    [ProtoContract]
    internal sealed class GetSupplierStatementsUnpaidInvoices : GetObjectEndpoint<Model.SupplierStatementsUnpaidInvoices>
    {
    }
}
