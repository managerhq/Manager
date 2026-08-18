using ManagerServer.Helpers;
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.PurchaseOrder
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(PurchaseOrders))]
    [Title(nameof(Strings.EmailTemplate), nameof(Strings.PurchaseOrder))]
    [Guide("Email templates allow you to customize how purchase orders appear when sent to suppliers via email. This template defines the default subject line and message body that will be used when emailing purchase orders.")]
    [Guide("The email template system supports placeholders that automatically insert relevant information from your purchase order. These placeholders ensure that each email contains the correct details without manual entry.")]
    [Header("Customizing Your Template")]
    [Guide("In the **Subject** field, enter the subject line for your purchase order emails. You can include placeholders like the purchase order number or supplier name to make each email unique and easily identifiable.")]
    [Guide("The **Message Body** field contains the main content of your email. Use placeholders to automatically include purchase order details, amounts, and other relevant information. The message body supports basic formatting to help structure your content professionally.")]
    [Header("Using Placeholders")]
    [Guide("Placeholders are special codes that get replaced with actual data when the email is sent. For example, a placeholder for the purchase order number will be replaced with the actual number when you send the email. This ensures accuracy and saves time.")]
    [Guide("Common placeholders include supplier information, purchase order details, amounts, and dates. Each placeholder must be typed exactly as shown in the system to work correctly.")]
    [Header("Best Practices")]
    [Guide("Keep your subject line clear and include key information like the purchase order number. This helps suppliers quickly identify and prioritize your orders.")]
    [Guide("In the message body, include all necessary information such as delivery instructions, payment terms, and contact details. A well-structured email template reduces back-and-forth communication and ensures suppliers have everything they need.")]
    [LinkGuide("To learn about email settings and SMTP configuration, see:", typeof(SmtpServer.EmailSettingsForm))]
    [Fields(typeof(ManagerServer.Model.EmailTemplateForPurchaseOrder))]
    internal sealed class EmailTemplateForPurchaseOrderForm : NakedVueForm<ManagerServer.Model.EmailTemplateForPurchaseOrder>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            var o = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailTemplateForPurchaseOrder>();
            if (string.IsNullOrEmpty(o.Subject) && string.IsNullOrWhiteSpace(o.MessageBody)) return true;
            return false;
        }
    }
}