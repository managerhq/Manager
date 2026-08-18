using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Settings.Footers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.SalesQuotes
{
    [ProtoContract]
    [Title(nameof(Strings.SalesQuote), nameof(Strings.Footer), nameof(Strings.View))]
    [Guide("This page displays the current footer that will appear on your sales quotes. The footer is shown exactly as it will appear when you print or email sales quotes to customers.")]
    [Guide("Use this preview to verify that your footer content, formatting, and layout appear correctly before sending quotes to customers.")]
    [LinkGuide("To edit the footer content, see:", typeof(SalesQuoteFooterForm))]
    internal class SalesQuoteFooterView : DefaultView<GetSalesQuoteFooterView>
    {
    }
}
