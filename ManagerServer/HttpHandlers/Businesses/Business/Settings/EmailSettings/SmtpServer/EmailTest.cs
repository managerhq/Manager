using System.IO;
using System.Collections.Generic;
using ManagerServer.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.Net.Http;
using ManagerServer.Model;
using MailKit.Net.Proxy;
using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.SmtpServer
{
    [ProtoContract]
    internal sealed class EmailTest : BusinessHandler
    {
        public override async Task Post()
        {
            var formData2 = await Request.ReadFormAsync();
            var json = formData2["Json"].ToString();

            var formData = Newtonsoft.Json.JsonConvert.DeserializeObject<ManagerServer.Model.EmailSettings>(json);

            var name = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BusinessDetails>().Name;
            if (string.IsNullOrWhiteSpace(name)) name = Business;

            if (formData.Protocol == ManagerServer.Model.Enums.Protocol.HTTP)
            {
                if (string.IsNullOrWhiteSpace(formData.HttpServer))
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync("HTTP server is empty. Please enter valid web service URL.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(formData.HttpReplyTo))
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync("Reply to is empty. Please enter valid email address.");
                    return;
                }

                UriBuilder uri;
                try
                {
                    uri = new UriBuilder(formData.HttpServer);
                }
                catch (UriFormatException ex)
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync(ex.Message);
                    return;
                }

                if (!formData.HttpServer.StartsWith("http://"))
                {
                    uri.Scheme = "https";
                }
                uri.Port = -1;

                var httpClient = new HttpClient();
                var requestFormData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("from", name),
                    new KeyValuePair<string, string>("reply_to", formData.HttpReplyTo),
                    new KeyValuePair<string, string>("to", formData.HttpReplyTo),
                    new KeyValuePair<string, string>("subject", Strings.TestMessage),
                    new KeyValuePair<string, string>("body", Strings.TestMessage),
                });

                try
                {
                    var httpResponse = await httpClient.PostAsync(uri.Uri, requestFormData);
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        var responseBody = await httpResponse.Content.ReadAsStringAsync();
                        Response.StatusCode = 500;
                        await Response.WriteAsync($"{(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}" + (string.IsNullOrWhiteSpace(responseBody) ? "" : $": {responseBody}"));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync(ex.Message);
                    return;
                }
            }
            else
            {
                var email = formData.SmtpCredentials;
                if (formData.SmtpCredentials != null && !formData.SmtpCredentials.Contains('@')) email = formData.EmailAddress;

                if (string.IsNullOrWhiteSpace(email))
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync("Email address is empty. Please enter valid email address.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(formData.SmtpServer))
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync("Hostname field is empty. Please enter valid hostname address.");
                    return;
                }

                var password = formData.Password;
                if (string.IsNullOrWhiteSpace(password)) password = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.EmailSettings>().Password;

                var message = new MimeMessage();
                try
                {
                    message.From.Add(new MailboxAddress(name, email));
                    message.To.Add(new MailboxAddress(name, email));
                    if (!string.IsNullOrWhiteSpace(formData.ReplyTo)) message.ReplyTo.Add(new MailboxAddress(name, formData.ReplyTo));
                }
                catch (ParseException ex)
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync(ex.Message);
                    return;
                }
                message.Subject = Strings.TestMessage;
                message.Body = new TextPart("plain") { Text = Strings.TestMessage };

                using (var client = new SmtpClient())
                {
                    if (formData.DoNotVerifyTLSCertificate) client.CheckCertificateRevocation = false;

                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None) return true;
                        if (formData.DoNotVerifyTLSCertificate) return true;
                        return false;
                    };

                    client.Timeout = (int)TimeSpan.FromSeconds(20).TotalMilliseconds;

                    var socks5Proxy = Environment.GetEnvironmentVariable("SOCKS5_PROXY");
                    if (!string.IsNullOrWhiteSpace(socks5Proxy))
                    {
                        var proxyUri = new Uri(socks5Proxy);
                        if (!string.IsNullOrWhiteSpace(proxyUri.UserInfo))
                            client.ProxyClient = new Socks5Client(proxyUri.Host, proxyUri.Port, new NetworkCredential(proxyUri.UserInfo.Split(':').First(), proxyUri.UserInfo.Split(':').Last()));
                        else
                            client.ProxyClient = new Socks5Client(proxyUri.Host, proxyUri.Port);
                    }

                    try
                    {
                        client.Connect(formData.SmtpServer, (int)formData.Port);
                    }
                    catch (Exception ex)
                    {
                        Response.StatusCode = 500;
                        await Response.WriteAsync(ex.Message);
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(formData.SmtpCredentials))
                    {
                        try
                        {
                            client.Authenticate(formData.SmtpCredentials, password);
                        }
                        catch (Exception ex)
                        {
                            Response.StatusCode = 500;
                            await Response.WriteAsync(ex.Message);
                            return;
                        }
                    }

                    try
                    {
                        client.Send(message);
                    }
                    catch (Exception ex)
                    {
                        Response.StatusCode = 500;
                        await Response.WriteAsync(ex.Message);
                        return;
                    }

                    client.Disconnect(true);
                }                
            }

            await Response.WriteAsync(Strings.TestEmailSuccessfullySent);
        }
    }
}