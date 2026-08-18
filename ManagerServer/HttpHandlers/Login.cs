using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ProtoBuf;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    [Title(nameof(Strings.Login))]
    [Guide("Enter your username to access Manager. If you already have an account, type your username and click `Next`.")]
    [Guide("If you need access to Manager or have forgotten your credentials, contact your system administrator for assistance.")]
    internal sealed class Login : LoginTemplate
    {
        [ProtoMember(1)] public string Username;
        [ProtoMember(2)] public bool InvalidUsername;

        protected override void InnerInnerGet()
        {
            var username = Username;
            if (string.IsNullOrWhiteSpace(username))
            {
                if (Request.Cookies["session"] != null)
                {
                    var session = Request.Cookies["session"].ToString();
                    if (session != null && !string.IsNullOrWhiteSpace(session))
                    {
                        var userCookie = UserCookie.Deserialize(session);
                        if (userCookie != null)
                        {
                            username = userCookie.Username;
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                var exists = ApplicationData.Users.AnyExist().GetAwaiter().GetResult();
                if (!exists)
                {
                    username = "administrator";
                }
            }

            using (Div())
            {
                using (Label()) Write(Strings.Username);
                InputText(name: nameof(FormData.Username), value: username, autofocus: true, @class: "form-control");
            }

            if (InvalidUsername)
            {
                using (Div(@class: "text-red-600 font-bold")) Write(Strings.InvalidUsername);
            }

            using (Div(@class: "flex gap-4 items-center"))
            {
                using (PrimaryButton())
                {
                    I(@class: "htmx-indicator me-2 fas fa-circle-notch fa-spin !hidden");
                    Write(Strings.Next);
                }
            }
        }

        public sealed class FormData
        {
            public string Username;
        }

        protected override async Task InnerPost()
        {
            if (!Request.HasFormContentType) return;

            var form = await Request.ReadFormAsync();
            var formData = new FormData()
            {
                Username = form[nameof(FormData.Username)],
            };

            if (string.IsNullOrWhiteSpace(formData.Username))
            {
                Response.Redirect(new Login().ToUrl());
                return;
            }

            var userRecord = await ApplicationData.Users.GetByUsernameAsync(formData.Username);

            if (userRecord != null && string.IsNullOrWhiteSpace(userRecord.Password))
            {
                if (string.Equals(formData.Username, "administrator", System.StringComparison.OrdinalIgnoreCase))
                {
                    Response.Redirect(new CreatePassword().ToUrl());
                    return;
                }
            }

            if (userRecord == null)
            {
                Response.Redirect(new Login() { Username = formData.Username, InvalidUsername = true }.ToUrl());
                return;
            }

            Response.Redirect(new LoginPassword() { Username = formData.Username }.ToUrl());
            return;
        }
    }
}
