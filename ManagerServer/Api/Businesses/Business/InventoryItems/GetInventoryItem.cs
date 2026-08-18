using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.InventoryItems
{
    [ProtoContract]
    internal sealed class GetInventoryItem : GetObjectEndpoint<Model.InventoryItem>
    {
    }
}
