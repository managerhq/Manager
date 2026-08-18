using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxSummary
{
    [ProtoContract]
    internal sealed class GetTaxSummary : GetObjectEndpoint<Model.TaxSummary>
    {
    }
}
