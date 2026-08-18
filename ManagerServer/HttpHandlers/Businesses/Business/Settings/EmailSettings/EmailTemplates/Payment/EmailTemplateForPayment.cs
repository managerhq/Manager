using ManagerServer.Helpers;
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.Payment
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Payments))]
    [Title(nameof(Strings.EmailTemplate), nameof(Strings.Payment))]
    [Guide("This email template is used when sending payment remittances to suppliers. Payment remittances are documents that inform suppliers about payments you have made to them.")]
    [Guide("You can customize both the subject line and message body to match your business communication style. The email will be automatically populated with payment details when sent.")]
    [Header("Using Placeholders")]
    [Guide("Use placeholders to include dynamic information in your emails. Placeholders are automatically replaced with actual data when the email is sent. Common placeholders include payment amount, payment date, supplier name, and invoice references.")]
    [Guide("Place placeholders in both the subject line and message body where you want specific payment information to appear. The system will replace these with the appropriate values for each payment.")]
    [Header("Best Practices")]
    [Guide("Keep your payment remittance emails professional and informative. Include key details such as the payment amount, payment method, and which invoices are being paid.")]
    [Guide("Consider adding your company's contact information in case suppliers have questions about the payment. A clear and complete payment remittance helps maintain good supplier relationships.")]
    [Fields(typeof(ManagerServer.Model.EmailTemplateForPayment))]
    internal sealed class EmailTemplateForPaymentForm : NakedVueForm<ManagerServer.Model.EmailTemplateForPayment>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            var o = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailTemplateForPayment>();
            if (string.IsNullOrEmpty(o.Subject) && string.IsNullOrWhiteSpace(o.MessageBody)) return true;
            return false;
        }
    }
}