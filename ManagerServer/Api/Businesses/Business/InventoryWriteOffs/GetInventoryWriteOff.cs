using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.InventoryWriteOffs
{
    [ProtoContract]
    internal sealed class GetInventoryWriteOff : GetObjectEndpoint<Model.InventoryWriteOff>
    {
    }
}
