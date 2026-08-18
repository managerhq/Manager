using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.CustomerPortal.Orders
{
    [ProtoContract]
    class CustomerPortalOrder : View<ManagerServer.Api.Businesses.Business.SalesOrders.GetSalesOrderView>
    {
    }
}
