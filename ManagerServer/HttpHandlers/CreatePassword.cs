using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Obsolete.Obsolete88;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    internal sealed class CreatePassword : LoginTemplate
    {
        [ProtoMember(2)] public bool PasswordMismatch;
        [ProtoMember(3)] public bool PasswordEmpty;

        protected override void InnerInnerGet()
        {
            var admin = ApplicationData.Users.GetByUsernameAsync("administrator").GetAwaiter().GetResult();
            if (admin == null || !string.IsNullOrWhiteSpace(admin.Password))
            {
                Response.Redirect(new Login().ToUrl());
                return;
            }

            using (Div())
            {
                using (Label()) Write(Strings.NewPassword);
                InputPassword(name: nameof(FormData.Password), autofocus: true, @class: "form-control");
            }

            using (Div())
            {
                using (Label()) Write(Strings.ConfirmPassword);
                InputPassword(name: nameof(FormData.ConfirmPassword), @class: "form-control");
            }

            if (PasswordMismatch)
            {
                using (Div(@class: "text-red-600 font-bold")) Write(Strings.PasswordsDoNotMatch);
            }

            if (PasswordEmpty)
            {
                using (Div(@class: "text-red-600 font-bold")) Write(Strings.PasswordRequired);
            }

            using (Div(@class: "flex gap-4 items-center"))
            {
                using (PrimaryButton())
                {
                    I(@class: "htmx-indicator me-2 fas fa-circle-notch fa-spin !hidden");
                    Write(Strings.CreatePassword);
                }
                using (DefaultLink(new Businesses.Businesses().ToUrl())) Write(Strings.Cancel);
            }
        }

        public sealed class FormData
        {
            public string Password;
            public string ConfirmPassword;
        }

        protected override async Task InnerPost()
        {
            if (!Request.HasFormContentType) return;

            var user = await ApplicationData.Users.GetByUsernameAsync("administrator");
            if (user == null) return;
            if (!string.IsNullOrWhiteSpace(user.Password)) return;

            var form = await Request.ReadFormAsync();
            var formData = new FormData()
            {
                Password = form[nameof(FormData.Password)],
                ConfirmPassword = form[nameof(FormData.ConfirmPassword)]
            };

            if (string.IsNullOrWhiteSpace(formData.Password))
            {
                Response.Redirect(new CreatePassword { PasswordEmpty = true }.ToUrl());
                return;
            }

            if (formData.Password != formData.ConfirmPassword)
            {
                Response.Redirect(new CreatePassword { PasswordMismatch = true }.ToUrl());
                return;
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(formData.Password);
            await ApplicationData.Users.Save(user);
            var session = await ApplicationData.Users.CreateSession(user);
            Response.Cookies.Append("session", session, new Microsoft.AspNetCore.Http.CookieOptions() { Expires = DateTime.UtcNow.AddYears(1), HttpOnly = true, SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax });

            Response.Redirect(new Businesses.Businesses().ToUrl());
        }
    }
}
