using ManagerServer.Helpers;
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.Receipt
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Receipts))]
    [Title(nameof(Strings.EmailTemplate), nameof(Strings.Receipt))]
    [Guide("Email templates for receipts allow you to automatically send payment confirmations to customers when you record their payments in the system.")]
    [Guide("This template will be used whenever you email a receipt directly from the *Receipts* tab, ensuring consistent communication with your customers.")]
    [Header("Template Configuration")]
    [Guide("The **Subject** field determines what appears in the email subject line. You can include placeholders that will be automatically replaced with actual values when the email is sent.")]
    [Guide("The **Message Body** contains the main content of your email. Use placeholders to automatically insert receipt details, customer information, and payment amounts.")]
    [Header("Available Placeholders")]
    [Guide("Placeholders are special codes that get replaced with actual data when sending emails. Common placeholders include customer name, receipt number, payment amount, and payment date.")]
    [Guide("To see all available placeholders, click the **Insert Placeholder** button when editing the template. This will show you a complete list of dynamic content you can include.")]
    [Fields(typeof(ManagerServer.Model.EmailTemplateForReceipt))]
    internal sealed class EmailTemplateForReceiptForm : NakedVueForm<ManagerServer.Model.EmailTemplateForReceipt>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            var o = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailTemplateForReceipt>();
            if (string.IsNullOrEmpty(o.Subject) && string.IsNullOrWhiteSpace(o.MessageBody)) return true;
            return false;
        }
    }
}