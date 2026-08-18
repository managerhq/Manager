using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Emails
{
    [ProtoContract]
    [Title(nameof(Strings.Email))]
    [Guide("The `Email` view displays the complete details of an email that has been sent from your business.")]
    [Guide("This view provides a comprehensive look at all the information associated with a sent email, including its content and metadata.")]
    [Header("Email Information")]
    [Guide("The email view shows essential details at the top of the page:")]
    [Guide("• `From` - The sender's email address used to send this email")]
    [Guide("• `To` - The recipient's email address")]
    [Guide("• `Date` - When the email was sent, displayed in your local time zone")]
    [Guide("• `Subject` - The email subject line, displayed as the main heading")]
    [Header("Email Content")]
    [Guide("The body of the email is displayed below the header information, showing exactly what was sent to the recipient.")]
    [Guide("If the email contains formatted content or embedded documents, you can click any `View` buttons within the email to expand and see the full content.")]
    [Header("Attachments")]
    [Guide("Any files attached to the email are listed at the bottom of the view.")]
    [Guide("Click on any attachment name to download or view the attached file.")]
    [Guide("The paperclip icon indicates that this email contains one or more attachments.")]
    [LinkGuide("To see all sent emails, go to:", typeof(Emails))]
    internal sealed class EmailView : DefaultView<ManagerServer.Api.Businesses.Business.Emails.GetEmailView>
    {
        protected override IEmailTemplate GetEmailTemplate()
        {
            using var c = ApplicationData.Businesses.SQLiteConnection(Business);
            var email = c.Get<ManagerServer.ApplicationData.Email>(Key);
            if (email == null) return null;
            return new ForwardSubjectTemplate("FW: " + (email.Subject ?? string.Empty));
        }

        protected override bool CanHaveAttachments()
        {
            return false;
        }

        private sealed class ForwardSubjectTemplate : IEmailTemplate
        {
            private readonly string _subject;
            public ForwardSubjectTemplate(string subject) { _subject = subject; }
            public string GetSubject() => _subject;
            public string GetBody() => null;
        }
    }
}
