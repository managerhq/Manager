using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Markdig;
using MailKit.Net.Smtp;
using MimeKit;
using System.Net;
using System.Net.Http;
using MailKit.Net.Proxy;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    internal sealed class SendEmail : BusinessHandler
    {
        [ProtoMember(1)] public string Referrer;
        [ProtoMember(2)] public Guid? Source;

        public sealed class FormData
        {
            public string To;
            public string Subject;
            public string Body;
            public string PDF;
            public string Filename;
            public Guid[] Attachments;
        }

        public override async Task Post()
        {
            var form = await Request.ReadFormAsync();
            var formData = new FormData()
            {
                To = form[nameof(FormData.To)],
                Subject = form[nameof(FormData.Subject)],
                Body = form[nameof(FormData.Body)],
                PDF = form[nameof(FormData.PDF)],
                Filename = form[nameof(FormData.Filename)],
                Attachments = form[nameof(FormData.Attachments)].ToString().Split(',').Where(x => Guid.TryParse(x, out Guid result)).Select(x => Guid.Parse(x)).ToArray()
            };

            var pipeline = new Markdig.MarkdownPipelineBuilder().UseSoftlineBreakAsHardlineBreak().Build();

            var to = (formData.To ?? string.Empty).Replace(',', ';').Split(';').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
            var subject = formData.Subject ?? string.Empty;
            var body = Markdig.Markdown.ToHtml(formData.Body ?? string.Empty, pipeline);

            var pdf = Convert.FromBase64String(formData.PDF);

            var emailSettings = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailSettings>();

            var key = Guid.CreateVersion7();

            var businessDetails = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BusinessDetails>();
            var name = businessDetails.Name;
            if (string.IsNullOrWhiteSpace(name)) name = Business;

            if (emailSettings.Protocol == ManagerServer.Model.Enums.Protocol.HTTP)
            {
                if (string.IsNullOrWhiteSpace(formData.To))
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync("Enter email address");
                    return;
                }

                try
                {
                    _ = to.Select(x => new MailboxAddress(null, x));
                }
                catch (Exception ex)
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync(@"""" + formData.To + @""": " + ex.Message);
                    return;
                }

                var uri = new UriBuilder(emailSettings.HttpServer);
                uri.Scheme = "https";
                uri.Port = -1;

                var httpClient = new HttpClient();
                var requestFormData = new MultipartFormDataContent();

                // .NET emits the Content-Disposition `name`/`filename` parameters as bare tokens
                // (e.g. name=from) whenever the value qualifies as one. Strict multipart parsers
                // such as undici (Node fetch) and Cloudflare Workers only look for a quoted
                // name="...", and reject everything else with
                // "Content-Disposition header in FormData part is missing a name".
                // Wrapping the value in quotes forces .NET to keep them on the wire.
                void AddField(string fieldName, string value)
                {
                    var content = new StringContent(value ?? string.Empty);
                    content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "\"" + fieldName + "\""
                    };
                    requestFormData.Add(content);
                }

                void AddFile(HttpContent content, string fieldName, string fileName)
                {
                    content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "\"" + fieldName + "\"",
                        FileName = "\"" + fileName + "\""
                    };
                    requestFormData.Add(content);
                }

                AddField("from", name);
                AddField("reply_to", emailSettings.HttpReplyTo);
#if DEBUG
                AddField("to", "lubos@hasko.au");
#else
                AddField("to", formData.To);
#endif
                AddField("subject", subject);
                AddField("body", body);

                if (pdf != null)
                {
                    var fileContent = new ByteArrayContent(pdf);
                    AddFile(fileContent, Guid.CreateVersion7().ToString(), subject + ".pdf");
                }

                if (formData.Attachments != null && formData.Attachments.Length > 0)
                {
                    foreach (var e in formData.Attachments)
                    {
                        var o = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Attachment>(e);
                        if (o != null)
                        {
                            if (o.Sha256 != null)
                            {
                                var stream = await ApplicationData.Storage.ReadAsync(o.Sha256);
                                if (stream != null)
                                {
                                    AddFile(new StreamContent(stream), Guid.CreateVersion7().ToString(), o.Name);
                                }
                            }
                            else
                            {
                                var buffer = ApplicationData.Businesses.GetBlob(Business, e);
                                if (buffer != null)
                                {
                                    AddFile(new ByteArrayContent(buffer), Guid.CreateVersion7().ToString(), o.Name);
                                }
                            }
                        }
                    }
                }

                var response = await httpClient.PostAsync(uri.ToString(), requestFormData);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    Response.StatusCode = 500;
                    var error = await response.Content.ReadAsStringAsync();
                    await Response.WriteAsync(error);
                    return;
                }
            }

            if (emailSettings.Protocol == ManagerServer.Model.Enums.Protocol.SMTP)
            {
                MailboxAddress sender = null;

                var senderEmailAddress = emailSettings.EmailAddress;
                if (!string.IsNullOrWhiteSpace(emailSettings.SmtpCredentials) && emailSettings.SmtpCredentials.Contains('@')) senderEmailAddress = emailSettings.SmtpCredentials;

                try
                {
                    sender = new MailboxAddress(name, senderEmailAddress);
                }
                catch (Exception ex)
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync(ex.Message);
                    return;
                }

                var mailMessage = new MimeMessage();

#if DEBUG
                mailMessage.To.Add(new MailboxAddress(null, "lubos@hasko.au"));
#else
                if (emailSettings.SendCopy)
                {
                    try
                    {
                        var bcc = new MailboxAddress(null, emailSettings.SendCopyEmail);
                        mailMessage.Bcc.Add(bcc);
                    }
                    catch (Exception ex)
                    {
                        Response.StatusCode = 500;
                        await Response.WriteAsync(ex.Message);
                        return;
                    }
                }

                foreach (var e in to)
                {
                    try
                    {
                        mailMessage.To.Add(new MailboxAddress(null, e));
                    }
                    catch (Exception ex)
                    {
                        Response.StatusCode = 500;
                        await Response.WriteAsync(@"""" + e + @""": " + ex.Message);
                        return;
                    }
                }

                if (!mailMessage.To.Any())
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync("A recipient must be specified.");
                    return;
                }
#endif

                if (emailSettings.ReceiveRepliesAtADifferentAddressThanYouSendFrom && !string.IsNullOrWhiteSpace(emailSettings.ReplyTo))
                {
                    foreach (var e in emailSettings.ReplyTo.Split(','))
                    {
                        mailMessage.ReplyTo.Add(new MailboxAddress(null, e));
                    }
                }

                var builder = new BodyBuilder();

                mailMessage.Subject = subject;
                builder.HtmlBody = body;

                if (pdf != null)
                {
                    builder.Attachments.Add(formData.Filename + ".pdf", pdf, ContentType.Parse("application/pdf"));
                }

                if (formData.Attachments != null && formData.Attachments.Length > 0)
                {
                    foreach (var e in formData.Attachments)
                    {
                        var o = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Attachment>(e);
                        if (o != null)
                        {
                            if (o.Sha256 != null)
                            {
                                var stream = await ApplicationData.Storage.ReadAsync(o.Sha256);
                                if (stream != null)
                                {
                                    builder.Attachments.Add(o.Name, stream);
                                }
                            }
                            else
                            {
                                var buffer = ApplicationData.Businesses.GetBlob(Business, e);
                                if (buffer != null)
                                {
                                    builder.Attachments.Add(o.Name, buffer);
                                }
                            }
                        }
                    }
                }

                mailMessage.Body = builder.ToMessageBody();

                using (var smtpClient = new SmtpClient())
                {
                    if (emailSettings.DoNotVerifyTLSCertificate) smtpClient.CheckCertificateRevocation = false;

                    smtpClient.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None) return true;
                        if (emailSettings.DoNotVerifyTLSCertificate) return true;
                        return false;
                    };

                    mailMessage.From.Add(sender);

                    try
                    {
                        var socks5Proxy = Environment.GetEnvironmentVariable("SOCKS5_PROXY");
                        if (!string.IsNullOrWhiteSpace(socks5Proxy))
                        {
                            var proxyUri = new Uri(socks5Proxy);
                            if (!string.IsNullOrWhiteSpace(proxyUri.UserInfo))
                                smtpClient.ProxyClient = new Socks5Client(proxyUri.Host, proxyUri.Port, new NetworkCredential(proxyUri.UserInfo.Split(':').First(), proxyUri.UserInfo.Split(':').Last()));
                            else
                                smtpClient.ProxyClient = new Socks5Client(proxyUri.Host, proxyUri.Port);
                        }
                        smtpClient.Connect(emailSettings.SmtpServer, (int)emailSettings.Port);
                    }
                    catch (Exception ex)
                    {
                        Response.StatusCode = 500;
                        await Response.WriteAsync(ex.Message);
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(emailSettings.SmtpCredentials))
                    {
                        try
                        {
                            smtpClient.Authenticate(emailSettings.SmtpCredentials, emailSettings.Password);
                        }
                        catch (Exception ex)
                        {
                            Response.StatusCode = 500;
                            await Response.WriteAsync(ex.Message);
                            return;
                        }
                    }

                    smtpClient.Timeout = (int)TimeSpan.FromSeconds(60).TotalMilliseconds;

                    try
                    {
                        smtpClient.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {
                        Response.StatusCode = 500;
                        await Response.WriteAsync(ex.Message);
                        return;
                    }
                }                
            }

            using (var c = ApplicationData.Businesses.SQLiteConnection(Business))
            {
                using var tx = c.BeginTransaction();
                tx.InsertOrReplace(new ManagerServer.ApplicationData.Email()
                {
                    Key = key,
                    User = this.GetUserName(),
                    Sender = name,
                    Recipient = string.Join(";", to),
                    Subject = subject,
                    Body = body,
                    Timestamp = DateTime.UtcNow.Ticks
                });

                tx.Commit();
            }

            Response.Headers["HX-Redirect"] = Referrer;
            //Response.Redirect(Referrer);
        }

        public sealed class JsonResponse
        {
            public string pdf_base64;
        }
    }
}