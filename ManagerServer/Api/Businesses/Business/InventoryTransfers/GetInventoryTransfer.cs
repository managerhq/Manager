using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.InventoryTransfers
{
    [ProtoContract]
    internal sealed class GetInventoryTransfer : GetObjectEndpoint<Model.InventoryTransfer>
    {
    }
}
