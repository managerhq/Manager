using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.EmailSettings))]
    [Guide("*Email Settings* allow you to configure Manager to send emails directly from the program without needing a separate email client.")]
    [Guide("This eliminates the need to manually copy and paste transaction details into your email software.")]
    [Guide("Once configured, you can email invoices, quotes, statements, and reports to customers and suppliers with just a few clicks.")]
    [SettingsItemScreenshot("fa-at", nameof(Strings.EmailSettings))]
    [Header("Setting Up Email")]
    [Guide("Setting up email requires two main steps:")]
    [Guide("First, configure your *SMTP server* settings to connect Manager to your email provider.")]
    [LinkGuide("Learn more about SMTP configuration:", typeof(SmtpServer.EmailSettingsForm))]
    [Guide("Second, optionally create *email templates* to standardize your email communications.")]
    [Header("Using Email Templates")]
    [Guide("Templates save time by pre-filling common email subjects and messages for different transaction types.")]
    [Guide("You can create templates for invoices, quotes, statements, and other documents you regularly send.")]
    [LinkGuide("Learn more about email templates:", typeof(EmailTemplates.EmailTemplates))]
    internal sealed class EmailSettings : NakedNamespaces
    {
    }
}
