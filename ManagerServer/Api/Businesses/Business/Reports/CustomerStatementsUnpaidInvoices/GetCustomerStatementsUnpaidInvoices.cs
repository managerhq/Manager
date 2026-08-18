using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.CustomerStatementsUnpaidInvoices
{
    [ProtoContract]
    internal sealed class GetCustomerStatementsUnpaidInvoices : GetObjectEndpoint<Model.CustomerStatementsUnpaidInvoices>
    {
    }
}
