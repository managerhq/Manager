using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.SalesOrders
{
    [ProtoContract]
    internal sealed class GetSalesOrder : GetObjectEndpoint<Model.SalesOrder>
    {
    }
}
