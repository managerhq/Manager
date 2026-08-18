using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.PurchaseQuotes
{
    [ProtoContract]
    [Title(nameof(Strings.Footer))]
    [Guide("Configure footer text that appears at the bottom of purchase quotes.")]
    [Guide("Use footers to add terms, conditions, or additional information to purchase quotes.")]
    [Fields(typeof(ManagerServer.Model.PurchaseQuoteFooter))]
    internal sealed class PurchaseQuoteFooterForm : NakedVueForm<ManagerServer.Model.PurchaseQuoteFooter>
    {
    }
}