using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxablePurchasesPerSupplier
{
    [ProtoContract]
    internal sealed class GetTaxablePurchasesPerSupplier : GetObjectEndpoint<Model.TaxablePurchasesPerSupplier>
    {
    }
}
