using MailKit.Net.Proxy;
using ManagerServer.Globalization;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    internal sealed class ForgotPassword : LoginTemplate
    {
        [ProtoMember(1)] public string Username;
        [ProtoMember(2)] public bool InvalidUsername;
        [ProtoMember(3)] public bool EmailSent;
        [ProtoMember(4)] public bool NoEmailAddress;

        protected override void InnerInnerGet()
        {
            var smtp = HttpContext.RequestServices.GetService<Services.SmtpSettings>();
            if (smtp == null)
            {
                Response.Redirect(new Login().ToUrl());
                return;
            }

            if (EmailSent)
            {
                using (Div(@class: "text-green-600 font-bold")) Write(Strings.PasswordResetEmailSent);

                using (Div(@class: "flex gap-4 items-center"))
                {
                    using (DefaultLink(new Login().ToUrl())) Write(Strings.ReturnToLogin);
                }
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

            InputHidden(name: nameof(FormData.Origin), id: "origin");

            using (Div(@class: "flex gap-4 items-center"))
            {
                using (PrimaryButton())
                {
                    I(@class: "htmx-indicator me-2 fas fa-circle-notch fa-spin !hidden");
                    Write(Strings.SendResetLink);
                }
                using (DefaultLink(new Login().ToUrl())) Write(Strings.Cancel);
            }

            using (Script()) Write("document.getElementById('origin').value = window.location.origin;");
        }

        public sealed class FormData
        {
            public string Username;
            public string Origin;
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
            var origin = form[nameof(FormData.Origin)].ToString().Trim();

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

            var token = new byte[32];
            RandomNumberGenerator.Fill(token);

            userRecord.PasswordResetToken = token;
            userRecord.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await ApplicationData.Users.Save(userRecord);

            var baseUrl = !string.IsNullOrWhiteSpace(origin) ? origin : $"{Request.Scheme}://{Request.Host}";
            var resetUrl = $"{baseUrl}{new ResetPassword { Username = username, Token = token }.ToUrl()}";

            await SendResetEmail(smtp, userRecord.EmailAddress, userRecord.Username, baseUrl, resetUrl);

            Response.Redirect(new ForgotPassword { EmailSent = true }.ToUrl());
        }

        private async Task SendResetEmail(Services.SmtpSettings smtp, string toEmail, string username, string origin, string resetUrl)
        {
            var message = new MimeKit.MimeMessage();
            message.From.Add(new MimeKit.MailboxAddress("Manager", smtp.FromAddress));
            message.To.Add(new MimeKit.MailboxAddress(null, toEmail));
            message.Subject = Strings.ResetPassword;

            var bodyBuilder = new MimeKit.BodyBuilder();
            bodyBuilder.HtmlBody = $"<p>A password reset was requested for username <b>{System.Net.WebUtility.HtmlEncode(username)}</b> on <b>{System.Net.WebUtility.HtmlEncode(origin)}</b>.</p>"
                + $"<p>Click the link below to reset your password. This link will expire in 1 hour.</p>"
                + $"<p><a href=\"{resetUrl}\">{Strings.ResetPassword}</a></p>"
                + $"<p>If you did not request this, you can safely ignore this email.</p>";
            bodyBuilder.TextBody = $"A password reset was requested for username \"{username}\" on {origin}.\n\n"
                + $"Click the link below to reset your password. This link will expire in 1 hour.\n\n"
                + $"{resetUrl}\n\n"
                + $"If you did not request this, you can safely ignore this email.";
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();

            client.CheckCertificateRevocation = false; // this doesn't seem to work on IPv6-only network if set True

            var socks5Proxy = Environment.GetEnvironmentVariable("SOCKS5_PROXY");
            if (!string.IsNullOrWhiteSpace(socks5Proxy))
            {
                var proxyUri = new Uri(socks5Proxy);
                if (!string.IsNullOrWhiteSpace(proxyUri.UserInfo))
                    client.ProxyClient = new Socks5Client(proxyUri.Host, proxyUri.Port, new NetworkCredential(proxyUri.UserInfo.Split(':').First(), proxyUri.UserInfo.Split(':').Last()));
                else
                    client.ProxyClient = new Socks5Client(proxyUri.Host, proxyUri.Port);
            }

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
