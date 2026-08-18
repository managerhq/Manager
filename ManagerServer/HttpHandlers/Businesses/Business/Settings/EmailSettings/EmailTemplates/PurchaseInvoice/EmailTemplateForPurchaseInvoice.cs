using ManagerServer.Helpers;
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.PurchaseInvoice
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(PurchaseInvoices))]
    [Title(nameof(Strings.EmailTemplate), nameof(Strings.PurchaseInvoice))]
    [Guide("Configure the email template that will be used when sending purchase invoices to internal recipients such as employees or managers for review and approval.")]
    [Guide("This template is specifically for internal distribution and differs from templates used for customer-facing documents.")]
    [Header("Template Customization")]
    [Guide("Customize the subject line to clearly identify the purchase invoice and its purpose. Use placeholders to automatically insert relevant information like invoice number, supplier name, or amount.")]
    [Guide("The message body can include detailed information about the purchase invoice, payment terms, and any special instructions for internal processing.")]
    [Guide("Available placeholders will dynamically populate with actual data when the email is sent, ensuring each message contains the correct invoice-specific information.")]
    [Fields(typeof(ManagerServer.Model.EmailTemplateForPurchaseInvoice))]
    internal sealed class EmailTemplateForPurchaseInvoiceForm : NakedVueForm<ManagerServer.Model.EmailTemplateForPurchaseInvoice>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            var o = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailTemplateForPurchaseInvoice>();
            if (string.IsNullOrEmpty(o.Subject) && string.IsNullOrWhiteSpace(o.MessageBody)) return true;
            return false;
        }
    }
}