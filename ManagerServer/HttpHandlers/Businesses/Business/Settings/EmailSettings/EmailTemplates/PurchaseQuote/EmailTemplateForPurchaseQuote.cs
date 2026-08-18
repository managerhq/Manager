using ManagerServer.Helpers;
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.PurchaseQuote
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(PurchaseQuotes))]
    [Title(nameof(Strings.EmailTemplate), nameof(Strings.PurchaseQuote))]
    [Guide("Configure the email template used when sending purchase quotes to internal recipients within your organization.")]
    [Guide("Purchase quotes can be emailed directly from the system to team members or departments who need to review or approve them.")]
    [Header("Template Customization")]
    [Guide("Customize the **Subject** line to clearly identify purchase quote emails in recipients' inboxes.")]
    [Guide("Design the **Message Body** using plain text or HTML formatting to include relevant information about the purchase quote.")]
    [Guide("Use placeholders to automatically insert dynamic content such as quote numbers, supplier names, dates, and amounts.")]
    [Fields(typeof(ManagerServer.Model.EmailTemplateForPurchaseQuote))]
    internal sealed class EmailTemplateForPurchaseQuoteForm : NakedVueForm<ManagerServer.Model.EmailTemplateForPurchaseQuote>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            var o = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailTemplateForPurchaseQuote>();
            if (string.IsNullOrEmpty(o.Subject) && string.IsNullOrWhiteSpace(o.MessageBody)) return true;
            return false;
        }
    }
}