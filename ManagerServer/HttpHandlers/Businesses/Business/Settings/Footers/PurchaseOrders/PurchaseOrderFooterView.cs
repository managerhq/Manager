using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.PurchaseOrders
{
    [ProtoContract]
    [Title(nameof(Strings.PurchaseOrder), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("This screen displays the footer that appears at the bottom of your purchase orders. You can preview how the footer will look when printed or sent to suppliers.")]
    [LinkGuide("For more information, see:", typeof(PurchaseOrderFooterForm))]
    internal class PurchaseOrderFooterView : DefaultView<GetPurchaseOrderFooterView>
    {
    }
}
