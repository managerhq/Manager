using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.SalesInvoices
{
    [ProtoContract]
    internal sealed class GetSalesInvoice : GetObjectEndpoint<Model.SalesInvoice>
    {
    }
}
