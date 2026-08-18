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
using System.IO;
using System.Security.Cryptography;

namespace ManagerServer.HttpHandlers.Users
{
    [ProtoContract]
    [Title(nameof(Strings.User), nameof(Strings.Edit))]
    [Guide("Use this form to create new users or edit existing user accounts. Each user requires a unique username and password to access the system.")]
    [Guide("User accounts control who can log in to your accounting system and what they can do once logged in.")]
    [Header("Security Considerations")]
    [Guide("When creating user accounts, follow these best practices:")]
    [Guide("- Use strong, unique passwords for each user")]
    [Guide("- Enable multi-factor authentication for sensitive accounts")]
    [Guide("- Regularly review and update user permissions")]
    [Guide("- Remove access for users who no longer need it")]
    [Header("User Types")]
    [Guide("Choose the appropriate user type based on the access level required:")]
    [Guide("`Administrator` users have full system access and can manage all businesses and settings.")]
    [Guide("`Restricted` users can only access specific businesses assigned to them, making this ideal for accountants or staff who work with limited clients.")]
    internal sealed class UserForm : Template
    {
        [ProtoMember(1)] public string Username;
        [ProtoMember(2)] public bool InvalidUsername;

        protected override async Task InnerGet()
        {
            var currentUser = this.GetCurrentUser();
            if (currentUser == null || currentUser.Type != ManagerServer.Model.UserType.Administrator)
            {
                Response.Redirect(new Users().ToUrl());
                return;
            }

            var user = await ApplicationData.Users.GetByUsernameAsync(Username);
            if (!string.IsNullOrWhiteSpace(Username))
            {
                if (user == null)
                {
                    Response.Redirect(new Users().ToUrl());
                    return;
                }
            }
            else
            {
                user = new UserRecord();
            }

            if (user.Businesses == null) user.Businesses = new string[0];

            Script("resources/jquery/jquery-1-8-2-min.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
            Script("resources/select2/select2.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());

            using (Div(@class: "p-8 mx-auto max-w-prose"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-body p-8"))
                    {
                        {
                            using (PostForm())
                            {
                                using (Div(@class: "flex flex-col space-y-4"))
                                {
                                    using (Div(@class: "text-xl font-bold")) Write(Strings.User);

                                    Hr();

                                    using (Div())
                                    {
                                        using (Label()) Write(Strings.Name);
                                        InputText(name: nameof(FormData.Name), placeholder: Strings.Unnamed, value: user.Name, autofocus: true, @class: "form-control");
                                    }
                                    using (Div())
                                    {
                                        var adminEmail = Environment.GetEnvironmentVariable("MANAGER_ADMINISTRATOR_EMAIL", EnvironmentVariableTarget.Process);
                                        var emailReadonly = !string.IsNullOrWhiteSpace(adminEmail) && string.Equals(user.Username, "administrator", StringComparison.OrdinalIgnoreCase);
                                        using (Label()) Write(Strings.EmailAddress);
                                        InputText(name: nameof(FormData.EmailAddress), value: user.EmailAddress, @class: "form-control", @readonly: emailReadonly);
                                    }
                                    using (Div())
                                    {
                                        using (Label()) Write(Strings.Username);
                                        InputText(name: nameof(FormData.Username), value: user.Username, @class: "form-control", @readonly: Username != null);
                                        if (InvalidUsername)
                                        {
                                            using (Div(@class: "text-red-600 font-bold")) Write(Strings.InvalidUsername);
                                        }
                                    }

                                    using (Div())
                                    {
                                        using (Label()) Write(Strings.Password);
                                        InputPassword(name: nameof(FormData.Password), placeholder: (Username != null ? "************" : null), @class: "form-control");
                                    }

                                    if (Username == GetCurrentUser().Username)
                                    {
                                        InputHidden(name: nameof(FormData.Type), value: user.Type.ToString());
                                    }
                                    else
                                    {
                                        using (Div())
                                        {
                                            using (Label()) Write(Strings.Role);
                                            using (Div()) using (Select(name: nameof(FormData.Type)))
                                            {
                                                Option(value: ManagerServer.Model.UserType.Administrator.ToString(), text: Strings.Administrator, selected: user.Type == ManagerServer.Model.UserType.Administrator);
                                                Option(value: ManagerServer.Model.UserType.Restricted.ToString(), text: Strings.RestrictedUser, selected: user.Type == ManagerServer.Model.UserType.Restricted);
                                            }
                                        }

                                        using (Div(id: "businesses", style: (user.Type == ManagerServer.Model.UserType.Administrator ? "display: none" : null)))
                                        {
                                            using (Label()) Write(Strings.Businesses);
                                            using (Div()) using (Select(name: nameof(FormData.Businesses), multiple: true))
                                            {
                                                foreach (var e in ApplicationData.Businesses.GetAll())
                                                {
                                                    Option(value: Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(e)), text: e, selected: (user.Businesses != null && user.Businesses.Contains(e)));
                                                }
                                            }
                                        }
                                    }

                                    using (Div())
                                    {
                                        using (Label())
                                        {
                                            var bytes = new byte[16];
                                            RandomNumberGenerator.Fill(bytes);

                                            InputCheckbox(name: nameof(FormData.MultifactorAuthentication), @class: "form-check-input", value: (user.MultifactorAuthentication ?? new Guid(bytes)).ToString(), @checked: user.MultifactorAuthentication.HasValue);
                                            using (Span(@class: "mx-2")) Write(Strings.EnforceMultifactorAuthentication);
                                        }
                                    }

                                    Hr();

                                    using (Div(@class: "flex gap-4"))
                                    {
                                        if (Username == null)
                                        {
                                            using (Button(@class: "btn btn-primary")) Write(Strings.Create);
                                        }
                                        if (Username != null)
                                        {
                                            using (Button(@class: "btn btn-success")) Write(Strings.Update);
                                            if (Username != this.GetCurrentUser().Username)
                                            {
                                                using (A(href: new DeleteUser() { Username = Username }.ToUrl(), @class: "btn btn-danger")) Write(Strings.Delete);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            using (Script())
            {
                Write("$(function () {");
                Write("$('form select').select2({ width: '100%' });");
                Write("$('form select[name=Type]').on('change', function(e) { if (e.val == 'Restricted') { $('#businesses').show(); $('#guides').show(); } else { $('#businesses').hide(); $('#guides').hide(); } });");
                Write("});");
            }
        }

        public sealed class FormData
        {
            public string Name;
            public string EmailAddress;
            public string Username;
            public string Password;
            public ManagerServer.Model.UserType Type;
            public string[] Businesses;
            public Guid? MultifactorAuthentication;
        }

        protected override async Task InnerPost()
        {
            if (this.GetCurrentUser().Type != ManagerServer.Model.UserType.Administrator)
            {
                Response.Redirect(new Users().ToUrl());
                return;
            }

            var form = await Request.ReadFormAsync();

            var formData = new FormData()
            {
                Name = form[nameof(FormData.Name)],
                EmailAddress = form[nameof(FormData.EmailAddress)],
                Username = form[nameof(FormData.Username)],
                Password = form[nameof(FormData.Password)],
                MultifactorAuthentication = Guid.TryParse(form[nameof(FormData.MultifactorAuthentication)], out Guid result) ? result : null,
                Type = Enum.TryParse<ManagerServer.Model.UserType>(form[nameof(FormData.Type)], out ManagerServer.Model.UserType userType) ? userType : ManagerServer.Model.UserType.Restricted,
                Businesses = form[nameof(FormData.Businesses)].ToString().Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => System.Text.UTF8Encoding.UTF8.GetString(Convert.FromBase64String(x))).ToArray()
            };

            formData.Username = formData.Username.ToLowerInvariant().Trim();

            if (Username == null)
            {
                if (string.IsNullOrWhiteSpace(formData.Username) || formData.Username.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
                {
                    Response.Redirect(new UserForm() { InvalidUsername = true }.ToUrl());
                    return;
                }

                if (await ApplicationData.Users.GetByUsernameAsync(formData.Username) != null)
                {
                    Response.Redirect(new UserForm() { InvalidUsername = true }.ToUrl());
                    return;
                }
            }

            var userRecord = await ApplicationData.Users.GetByUsernameAsync(Username) ?? new UserRecord();
            userRecord.Name = formData.Name;
            userRecord.EmailAddress = formData.EmailAddress;
            if (Username == null) userRecord.Username = formData.Username;
            userRecord.Type = formData.Type;
            userRecord.Businesses = formData.Businesses?.ToArray();
            userRecord.MultifactorAuthentication = formData.MultifactorAuthentication;

            // Reset verification when multi-factor authentication is disabled
            if (!userRecord.MultifactorAuthentication.HasValue) userRecord.Verified = false;

            if (!string.IsNullOrWhiteSpace(formData.Password))
            {
                userRecord.Password = BCrypt.Net.BCrypt.HashPassword(formData.Password);
                userRecord.Sessions.Clear();
            }
            await ApplicationData.Users.Save(userRecord);
            Response.Redirect(new Users().ToUrl());
        }
    }
}