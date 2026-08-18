using ManagerServer.Helpers;
using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.EmailTemplates.Payslip
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Payslips))]
    [Title(nameof(Strings.EmailTemplate), nameof(Strings.Payslip))]
    [Guide("Email templates allow you to standardize communications when sending payslips to employees. This ensures consistent, professional messaging while saving time through automation.")]
    [Guide("The email template will be used whenever you email a payslip directly from the software. You can customize both the subject line and message body to match your organization's communication style.")]
    [Header("Using Placeholders")]
    [Guide("Placeholders automatically insert specific information into your emails. When the email is sent, placeholders are replaced with actual data from the payslip and employee records.")]
    [Guide("Common placeholders include employee names, pay periods, and payment amounts. The available placeholders are shown when editing the template, making it easy to create personalized messages.")]
    [Header("Best Practices")]
    [Guide("Keep your subject line clear and descriptive so employees can easily identify payslip emails in their inbox. Include key information like the pay period or payment date.")]
    [Guide("In the message body, provide essential details while keeping the tone professional yet friendly. Consider including information about where employees can direct questions about their pay.")]
    [Fields(typeof(ManagerServer.Model.EmailTemplateForPayslip))]
    internal sealed class EmailTemplateForPayslipForm : NakedVueForm<ManagerServer.Model.EmailTemplateForPayslip>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            var o = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailTemplateForPayslip>();
            if (string.IsNullOrEmpty(o.Subject) && string.IsNullOrWhiteSpace(o.MessageBody)) return true;
            return false;
        }
    }
}