using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxReconciliation
{
    [ProtoContract]
    internal sealed class GetTaxReconciliation : GetObjectEndpoint<Model.TaxReconciliation>
    {
    }
}
