using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.SalesInvoiceTotalsByItem
{
    [ProtoContract]
    internal sealed class GetSalesInvoiceTotalsByItem : GetObjectEndpoint<Model.SalesInvoiceTotalsByItem>
    {
    }
}
