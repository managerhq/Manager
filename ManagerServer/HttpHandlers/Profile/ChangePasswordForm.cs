using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpFramework;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using System.Threading.Tasks;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Profile
{
    [ProtoContract]
    [Title(nameof(Strings.Password))]
    [Guide("Any logged-in user can change their own password by clicking their name in the top-right corner to access the password screen.")]
    [Guide("The password screen displays your username and allows you to enter a new password to update your account credentials.")]
    internal sealed class ChangePasswordForm : Template
    {
        protected override Task InnerGet()
        {
            using (Div(@class: "p-8 mx-auto max-w-prose"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-body p-8"))
                    {
                        var user = this.GetCurrentUser();
                        if (user != null)
                        {
                            using (PostForm())
                            {
                                using (Div(@class: "flex flex-col space-y-4"))
                                {
                                    using (Div(@class: "text-xl font-bold")) Write(user.Name);
                                    Hr();

                                    using (Div())
                                    {
                                        using (Label()) Write(Strings.Username);
                                        InputText(@readonly: true, value: user.Username, @class: "form-control");
                                    }

                                    using (Div())
                                    {
                                        using (Label()) Write(Strings.Password);
                                        InputPassword(name: nameof(FormData.Password), placeholder: " ************", @class: "form-control");
                                    }

                                    Hr();

                                    using (Div()) using (Button(@class: "btn btn-success")) Write(Strings.Update);
                                }
                            }

                            if (user.Sessions != null && user.Sessions.Count > 0)
                            {
                                using (Div(@class: "flex flex-col space-y-4 mt-8"))
                                {
                                    using (Div()) using (Label()) Write(Strings.Where_you_are_logged_in);
                                    using (Table(@class: "w-full"))
                                    {
                                        using (Tr())
                                        {
                                            using (Th(@class: "ltr:text-left rtl:text-right")) Write(Strings.Device);
                                            using (Th()) { }
                                            using (Th(@class: "w-px")) { }
                                        }
                                        foreach (var e in user.Sessions.OrderByDescending(x => x.Timestamp))
                                        {
                                            using (Tr())
                                            {
                                                var userCookie = UserCookie.Deserialize(Request.Cookies["session"].ToString());
                                                if (userCookie?.UserSession == e.Key)
                                                {
                                                    using (Td(@class: "font-bold")) Write(Strings.ThisComputer);
                                                }
                                                else
                                                {
                                                    using (Td(@class: "userAgent")) Write(e.UserAgent);
                                                }
                                                using (Td(@class: "text-center"))
                                                {
                                                    Write(e.Location);
                                                }
                                                using (Td())
                                                {
                                                    using (PostForm())
                                                    {
                                                        InputHidden(name: nameof(FormData.Session), value: e.Key.ToString());
                                                        using (DefaultButton()) Write(Strings.Logout);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            Script(src: "resources/uaparser/ua-parser.js");
                            using (Script())
                            {
                                Write("var elements =  document.getElementsByClassName('userAgent');");
                                Write("for (var i = 0; i < elements.length; i++) {");
                                Write("var e = elements[i];");
                                Write("var parser = new UAParser(e.innerHTML);");
                                Write("var result = parser.getResult();");
                                Write("e.innerHTML = result.browser.name+' '+result.browser.version+' &mdash; '+result.os.name+' '+result.os.version;");
                                Write("}");
                            }
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        public sealed class FormData
        {
            public string Password;
            public Guid? Session;
        }

        protected override async Task InnerPost()
        {
            var form = await Request.ReadFormAsync();
            var formData = new FormData()
            {
                Password = form[nameof(FormData.Password)],
                Session = Guid.TryParse(form[nameof(FormData.Session)], out Guid sessionKey) ? sessionKey : null
            };
            var user = this.GetCurrentUser();

            if (user != null)
            {
                var userRecord = await ApplicationData.Users.GetByUsernameAsync(user.Username);
                if (userRecord == null) { Response.Redirect(new Businesses.Businesses().ToUrl()); return; }

                if (formData.Session.HasValue)
                {
                    userRecord.Sessions = userRecord.Sessions.Where(x => x.Key != formData.Session.Value).ToList();
                    await ApplicationData.Users.Save(userRecord);
                    Response.Redirect(this.ToUrl());
                    return;
                }

                if (!string.IsNullOrWhiteSpace(formData.Password))
                {
                    userRecord.Password = BCrypt.Net.BCrypt.HashPassword(formData.Password);
                    await ApplicationData.Users.Save(userRecord);
                }
            }
            Response.Redirect(new Businesses.Businesses().ToUrl());
        }
    }
}