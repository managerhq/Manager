using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.SalesInvoiceTotalsByCustomer
{
    [ProtoContract]
    internal sealed class GetSalesInvoiceTotalsByCustomer : GetObjectEndpoint<Model.SalesInvoiceTotalsByCustomer>
    {
    }
}
