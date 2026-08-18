using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryPriceList
{
    [ProtoContract]
    internal sealed class GetInventoryPriceList : GetObjectEndpoint<Model.InventoryPriceList>
    {
    }
}
