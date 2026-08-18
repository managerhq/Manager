using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryQuantityByLocation
{
    [ProtoContract]
    internal sealed class GetInventoryQuantityByLocation : GetObjectEndpoint<Model.InventoryQuantityByLocation>
    {
    }
}
