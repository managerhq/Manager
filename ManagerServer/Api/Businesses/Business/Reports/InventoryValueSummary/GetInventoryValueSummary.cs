using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryValueSummary
{
    [ProtoContract]
    internal sealed class GetInventoryValueSummary : GetObjectEndpoint<Model.InventoryValueSummary>
    {
    }
}
