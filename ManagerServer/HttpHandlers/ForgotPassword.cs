using MailKit.Net.Proxy;
using ManagerServer.Globalization;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    internal sealed class ForgotPassword : LoginTemplate
    {
        [ProtoMember(1)] public string Username;
        [ProtoMember(2)] public bool InvalidUsername;
        [ProtoMember(4)] public bool NoEmailAddress;

        protected override void InnerInnerGet()
        {
            var smtp = HttpContext.RequestServices.GetService<Services.SmtpSettings>();
            if (smtp == null)
            {
                Response.Redirect(new Login().ToUrl());
                return;
            }

            using (Div())
            {
                using (Label()) Write(Strings.Username);
                InputText(name: nameof(FormData.Username), value: Username, autofocus: true, @class: "form-control");
            }

            if (InvalidUsername)
            {
                using (Div(@class: "text-red-600 font-bold")) Write(Strings.InvalidUsername);
            }

            if (NoEmailAddress)
            {
                using (Div(@class: "text-red-600 font-bold")) Write(Strings.NoEmailAddress);
            }

            using (Div(@class: "flex gap-4 items-center"))
            {
                using (PrimaryButton())
                {
                    I(@class: "htmx-indicator me-2 fas fa-circle-notch fa-spin !hidden");
                    Write(Strings.SendResetCode);
                }
                using (DefaultLink(new Login().ToUrl())) Write(Strings.Cancel);
            }
        }

        public sealed class FormData
        {
            public string Username;
        }

        protected override async Task InnerPost()
        {
            var smtp = HttpContext.RequestServices.GetService<Services.SmtpSettings>();
            if (smtp == null)
            {
                Response.Redirect(new Login().ToUrl());
                return;
            }

            if (!Request.HasFormContentType) return;

            var form = await Request.ReadFormAsync();
            var username = form[nameof(FormData.Username)].ToString().Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(username))
            {
                Response.Redirect(new ForgotPassword().ToUrl());
                return;
            }

            var userRecord = await ApplicationData.Users.GetByUsernameAsync(username);

            if (userRecord == null)
            {
                Response.Redirect(new ForgotPassword { Username = username, InvalidUsername = true }.ToUrl());
                return;
            }

            if (string.IsNullOrWhiteSpace(userRecord.EmailAddress))
            {
                Response.Redirect(new ForgotPassword { Username = username, NoEmailAddress = true }.ToUrl());
                return;
            }

            // The email carries a code the user types back in, not a link. The server has no reliable way to
            // know its own public address — behind a reverse proxy Request.Host is the proxy-to-Kestrel hop,
            // and anything the client tells us can be forged — so there is no trustworthy URL to email.
            var code = Helpers.PasswordResetCode.Generate();

            userRecord.PasswordResetToken = Helpers.PasswordResetCode.Normalize(code);
            userRecord.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await ApplicationData.Users.Save(userRecord);

            await SendResetEmail(smtp, userRecord.EmailAddress, userRecord.Username, code);

            Response.Redirect(new ResetPassword { Username = username, CodeSent = true }.ToUrl());
        }

        private async Task SendResetEmail(Services.SmtpSettings smtp, string toEmail, string username, string code)
        {
            var message = new MimeKit.MimeMessage();
            message.From.Add(new MimeKit.MailboxAddress("Manager", smtp.FromAddress));
            message.To.Add(new MimeKit.MailboxAddress(null, toEmail));
            message.Subject = Strings.ResetPassword;

            var bodyBuilder = new MimeKit.BodyBuilder();
            bodyBuilder.HtmlBody = $"<p>A password reset was requested for username <b>{System.Net.WebUtility.HtmlEncode(username)}</b>.</p>"
                + $"<p>Enter this code on the password reset screen:</p>"
                + $"<p style=\"font-size: 24px; font-family: monospace; letter-spacing: 2px\"><b>{System.Net.WebUtility.HtmlEncode(code)}</b></p>"
                + $"<p>The code will expire in 1 hour.</p>"
                + $"<p>If you did not request this, you can safely ignore this email.</p>";
            bodyBuilder.TextBody = $"A password reset was requested for username \"{username}\".\n\n"
                + $"Enter this code on the password reset screen:\n\n"
                + $"    {code}\n\n"
                + $"The code will expire in 1 hour.\n\n"
                + $"If you did not request this, you can safely ignore this email.";
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();

            client.CheckCertificateRevocation = false; // this doesn't seem to work on IPv6-only network if set True

            if (smtp.UseSsl)
                await client.ConnectAsync(smtp.Host, smtp.Port, MailKit.Security.SecureSocketOptions.SslOnConnect);
            else
                await client.ConnectAsync(smtp.Host, smtp.Port, MailKit.Security.SecureSocketOptions.StartTls);

            if (!string.IsNullOrEmpty(smtp.Username))
                await client.AuthenticateAsync(smtp.Username, smtp.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
