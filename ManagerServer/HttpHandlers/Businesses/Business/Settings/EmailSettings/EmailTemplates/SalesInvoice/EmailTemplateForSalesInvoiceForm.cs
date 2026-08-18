using ManagerServer.Helpers;
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.SalesInvoice
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(SalesInvoices))]
    [Title(nameof(Strings.EmailTemplate), nameof(Strings.SalesInvoice))]
    [Guide("Configure the email template for sending sales invoices to customers.")]
    [Guide("Customize subject line and message body with placeholders for dynamic content.")]
    [Fields(typeof(ManagerServer.Model.EmailTemplateForSalesInvoice))]
    internal sealed class EmailTemplateForSalesInvoiceForm : NakedVueForm<ManagerServer.Model.EmailTemplateForSalesInvoice>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            var o = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailTemplateForSalesInvoice>();
            if (string.IsNullOrEmpty(o.Subject) && string.IsNullOrWhiteSpace(o.MessageBody)) return true;
            return false;
        }
    }
}