using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.SupplierSummary
{
    [ProtoContract]
    internal sealed class GetSupplierSummary : GetObjectEndpoint<Model.SupplierSummary>
    {
    }
}
