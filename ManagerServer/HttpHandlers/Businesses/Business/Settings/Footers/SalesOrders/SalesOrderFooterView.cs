using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.SalesOrders
{
    [ProtoContract]
    [Title(nameof(Strings.SalesOrder), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("This screen displays a preview of your *sales order footer* as it will appear on printed or emailed sales orders.")]
    [Guide("Use this view to verify that your footer content is formatted correctly and contains all necessary information before sending sales orders to customers.")]
    [LinkGuide("To edit the footer content, see:", typeof(SalesOrderFooterForm))]
    internal class SalesOrderFooterView : DefaultView<GetSalesOrderFooterView>
    {
    }
}
