using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.ProductionOrders
{
    [ProtoContract]
    internal sealed class GetProductionOrder : GetObjectEndpoint<Model.ProductionOrder>
    {
    }
}
