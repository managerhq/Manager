using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxAudit
{
    [ProtoContract]
    internal sealed class GetTaxAudit : GetObjectEndpoint<Model.TaxAudit>
    {
    }
}
