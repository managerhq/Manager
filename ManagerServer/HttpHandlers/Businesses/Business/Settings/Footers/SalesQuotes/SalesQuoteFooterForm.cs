using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.SalesQuotes
{
    [ProtoContract]
    [Title(nameof(Strings.Footer))]
    [Guide("Configure footer text that appears at the bottom of sales quotes.")]
    [Guide("Use footers to add terms, conditions, or additional information to sales quotes.")]
    [Fields(typeof(ManagerServer.Model.SalesQuoteFooter))]
    internal sealed class SalesQuoteFooterForm : NakedVueForm<ManagerServer.Model.SalesQuoteFooter>
    {
    }
}