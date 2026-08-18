using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.Receipts
{
    [ProtoContract]
    [Title(nameof(Strings.Receipt), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("The receipt footer view allows you to preview how your custom footer will appear on receipts before applying changes.")]
    [Guide("This preview shows the footer exactly as it will be displayed when you issue receipts to customers, helping you ensure proper formatting and content.")]
    [LinkGuide("To edit the footer content, see:", typeof(ReceiptFooterForm))]
    internal class ReceiptFooterView : DefaultView<GetReceiptFooterView>
    {
    }
}
