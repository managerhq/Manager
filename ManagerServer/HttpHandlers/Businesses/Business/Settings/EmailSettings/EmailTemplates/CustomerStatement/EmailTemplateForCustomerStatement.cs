using ManagerServer.Helpers;
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.CustomerStatement
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Customers))]
    [Title(nameof(Strings.EmailTemplate), nameof(Strings.CustomerStatement))]
    [Guide("Customer statement email templates allow you to customize the emails sent when delivering *customer statements* to your customers.")]
    [Guide("This template will be used whenever you email a customer statement from the **Customer Statements** report.")]
    [Header("Template Configuration")]
    [Guide("You can customize both the subject line and message body of the email. The template supports placeholders that will be automatically replaced with actual values when sending the email.")]
    [Guide("Common placeholders include customer name, statement date, and business details. These placeholders ensure each email is personalized with the correct information.")]
    [Fields(typeof(ManagerServer.Model.EmailTemplateForCustomerStatement))]
    internal sealed class EmailTemplateForCustomerStatementForm : NakedVueForm<ManagerServer.Model.EmailTemplateForCustomerStatement>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            var o = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailTemplateForCustomerStatement>();
            if (string.IsNullOrEmpty(o.Subject) && string.IsNullOrWhiteSpace(o.MessageBody)) return true;
            return false;
        }
    }
}