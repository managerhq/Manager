using ManagerServer.Helpers;
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.SalesQuote
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(SalesQuotes))]
    [Title(nameof(Strings.EmailTemplate), nameof(Strings.SalesQuote))]
    [Guide("Configure the email template for sending sales quotes to customers.")]
    [Guide("Customize subject line and message body with placeholders for dynamic content.")]
    [Fields(typeof(ManagerServer.Model.EmailTemplateForSalesQuote))]
    internal sealed class EmailTemplateForSalesQuoteForm : NakedVueForm<ManagerServer.Model.EmailTemplateForSalesQuote>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            var o = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailTemplateForSalesQuote>();
            if (string.IsNullOrEmpty(o.Subject) && string.IsNullOrWhiteSpace(o.MessageBody)) return true;
            return false;
        }
    }
}