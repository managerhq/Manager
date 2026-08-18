using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.PurchaseQuotes
{
    [ProtoContract]
    [Title(nameof(Strings.PurchaseQuote), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("This screen displays the current footer text that appears at the bottom of all your purchase quotes.")]
    [Guide("The footer typically contains important information such as terms and conditions, payment details, or legal disclaimers that apply to your purchase quotes.")]
    [Guide("You can review how the footer will appear on actual purchase quotes before making any changes.")]
    [LinkGuide("To edit the footer content, see:", typeof(PurchaseQuoteFooterForm))]
    internal class PurchaseQuoteFooterView : DefaultView<GetPurchaseQuoteFooterView>
    {
    }
}
