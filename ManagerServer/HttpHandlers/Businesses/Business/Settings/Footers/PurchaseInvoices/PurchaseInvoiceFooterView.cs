using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.PurchaseInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.PurchaseInvoice), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("This screen displays the current footer that will appear at the bottom of all purchase invoices.")]
    [Guide("The footer preview shows exactly how it will look when printed or sent to suppliers, including any custom text, payment terms, or company information you have configured.")]
    [LinkGuide("To edit the footer content, see:", typeof(PurchaseInvoiceFooterForm))]
    internal class PurchaseInvoiceFooterView : DefaultView<GetPurchaseInvoiceFooterView>
    {
    }
}
