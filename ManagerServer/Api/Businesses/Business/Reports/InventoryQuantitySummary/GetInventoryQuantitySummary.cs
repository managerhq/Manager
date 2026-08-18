using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryQuantitySummary
{
    [ProtoContract]
    internal sealed class GetInventoryQuantitySummary : GetObjectEndpoint<Model.InventoryQuantitySummary>
    {
    }
}
