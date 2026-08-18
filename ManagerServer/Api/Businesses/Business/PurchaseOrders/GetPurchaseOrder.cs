using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.PurchaseOrders
{
    [ProtoContract]
    internal sealed class GetPurchaseOrder : GetObjectEndpoint<Model.PurchaseOrder>
    {
    }
}
