using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxTransactions
{
    [ProtoContract]
    internal sealed class GetTaxTransactions : GetObjectEndpoint<Model.TaxTransactions>
    {
    }
}
