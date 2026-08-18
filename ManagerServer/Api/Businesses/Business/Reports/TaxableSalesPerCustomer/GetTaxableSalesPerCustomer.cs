using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxableSalesPerCustomer
{
    [ProtoContract]
    internal sealed class GetTaxableSalesPerCustomer : GetObjectEndpoint<Model.TaxableSalesPerCustomer>
    {
    }
}
