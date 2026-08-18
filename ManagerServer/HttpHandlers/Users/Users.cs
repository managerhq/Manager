using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Users
{
    [ProtoContract]
    [Title(nameof(Strings.Users))]
    [Guide("The `Users` screen enables administrators to manage user accounts by adding, editing, or removing users.")]
    [Guide("Administrators can assign specific roles or permissions, controlling access to different sections of the accounting data.")]
    [Guide("This feature is crucial for businesses looking to delegate accounting tasks while restricting access to sensitive business data.")]
    [TopLevelTabScreenshot("fa-people-group", nameof(Strings.Users))]
    [Header("Creating New Users")]
    [Guide("To create a new user, click the `New User` button.")]
    [PrimaryButtonScreenshot(nameof(Strings.NewUser))]
    [LinkGuide("For more information, see:", typeof(UserForm))]
    [Header("User Roles and Permissions")]
    [Guide("If you create a user with the `Administrator` role, they will gain full access to the system, including all businesses and other users.")]
    [Guide("If you create a `Restricted User`, their username will display the list of businesses they can access.")]
    [Guide("When a restricted user has businesses assigned to them, they will see these businesses under their `Businesses` tab. However, by default they will have no access to any features within those businesses.")]
    [Guide("Click on a business listed under their username to configure their `User Permissions` for that specific business.")]
    [LinkGuide("For more information, see:", typeof(Businesses.Business.Settings.UserPermissions.UserPermissionsForm))]
    [Header("Testing User Access")]
    [Guide("After setting up a restricted user, you can verify what they have access to by clicking the `Impersonate` button.")]
    [Guide("This action will log you into their account immediately, allowing you to see exactly what they can access.")]
    [DefaultButtonScreenshot(nameof(Strings.Impersonate))]
    [Guide("To return to your administrator account, click the `Logout` button located in the top-right corner.")]
    [Header("Custom Branding")]
    [Guide("You can upload your company logo to display on the login screen. Click the image icon next to the `New User` button to upload your logo.")]
    [LinkGuide("For more information, see:", typeof(LogoUpload))]
    internal sealed class Users : Template
    {
        protected override Task InnerGet()
        {
            var referrer = this.ToUrl();

            using (Div(@class: "p-8")) using (Div(@class: "max-w-prose mx-auto"))
            {
                var currentUser = this.GetCurrentUser();
                if (Edition.IsDesktop)
                {
                    using (Div(@class: "card"))
                    {
                        using (Div(@class: "card-body p-8 flex flex-col gap-4"))
                        {
                            using (Div()) Write(Strings.MultiUserAccessNotAvailableInDesktopEdition);
                            using (Div()) Write(Strings.TryCloudEditionForMultiUserAccessAndOtherBenefits);

                            using (Div(@class: "flex")) using (A(href: "https://www.manager.cloud", @class: "btn btn-primary", target: "_blank")) Write(Strings.LearnMore);
                        }
                    }
                }
                else if (currentUser.Type == ManagerServer.Model.UserType.Restricted)
                {
                    using (Div(@class: "card"))
                    {
                        using (Div(@class: "card-body p-8"))
                        {
                            using (Div(style: "font-size: 24px; font-weight: bold; color: #333; padding-top: 20px")) Write("You are not authorised");
                            using (Div(style: "font-weight: bold; padding-top: 20px; line-height: 175%")) Write("You are not authorised to access this part of the system. Only administrators of <u>" + Request.Host + "</u> are allowed.");
                        }
                    }
                }
                else
                {
                    var users = ApplicationData.Users.GetAllAsync().GetAwaiter().GetResult();
                    users = users.OrderByDescending(x => x.Username == "administrator").ThenByDescending(x => x.Type == ManagerServer.Model.UserType.Administrator).ThenBy(x => x.Name).ToArray();

                    using (Div(@class: "flex justify-between items-end"))
                    {
                        using (Div(@class: "font-bold text-lg text-neutral-400 px-3"))
                        {
                            using (Div(@class: "flex gap-4 items-center"))
                            {
                                using (Span()) Write(Strings.Users);
                                WriteHelp();
                            }
                        }
                        using (Div(@class: "flex gap-4 justify-end items-center print:hidden"))
                        {
                            using (A(href: new UserForm().ToUrl(), @class: "btn btn-primary")) Write(Strings.NewUser);

                            using (A(href: new LogoUpload().ToUrl(), @class: "opacity-25 hover:opacity-50 text-(--foreground)"))
                            {
                                I(@class: "fas fa-image text-base");
                            }
                        }
                    }                    

                    var businesses = ApplicationData.Businesses.GetAll();
                    var usernames = users.Select(x => x.Username).Where(x => !string.IsNullOrWhiteSpace(x)).GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

                    using (Table(style: "width: 100%; font-size: 14px; color: #333; margin-top: 10px"))
                    {
                        foreach (var e in users)
                        {
                            using (Tr())
                            {
                                using (Td(colspan: 3, style: "padding: 0px")) Hr();
                            }
                            using (Tr())
                            {
                                using (Td(style: "width: 1px; padding: 10px; vertical-align: top"))
                                {
                                    I(@class: "fas fa-user-circle text-base", style: "opacity: " + (e.Type == ManagerServer.Model.UserType.Administrator ? "0.4" : "0.2"));
                                }
                                using (Td(style: "padding: 10px"))
                                {
                                    using (Div())
                                    {
                                        using (A(href: new UserForm() { Username = e.Username }.ToUrl(), style: "font-size: 14px", @class: "font-semibold"))
                                        {
                                            if (string.IsNullOrWhiteSpace(e.Name)) Write(Strings.Unnamed);
                                            else Write(e.Name);
                                        }

                                        using (Span(style: "font-size: 12px; margin-left: 10px", @class: "font-semibold"))
                                        {
                                            if (!string.IsNullOrWhiteSpace(e.Username) && usernames.ContainsKey(e.Username) && usernames[e.Username] > 1)
                                            {
                                                using (Span(style: "color: red")) Write(e.Username);
                                            }
                                            else
                                            {
                                                using (Span(style: "color: #999")) Write(e.Username);
                                            }
                                        }
                                    }
                                    if (e.Type == ManagerServer.Model.UserType.Restricted)
                                    {
                                        using (Div(style: "padding-top: 3px"))
                                        {
                                            var userBusinesses = e.Businesses?.Where(x => businesses.Contains(x)).ToArray();
                                            if (userBusinesses != null && userBusinesses.Any())
                                            {
                                                for (int i = 0; i < userBusinesses.Length; i++)
                                                {
                                                    using (Div(style: "font-size: 12px; line-height: 16px; padding: 0px; margin: 0px"))
                                                    {
                                                        if (ManagerServer.Globalization.Languages.IsRightToLeft())
                                                        {
                                                            using (Span(style: "color: #ccc; margin-left: 5px; font-size: 16px"))
                                                            {
                                                                if (i == userBusinesses.Length - 1) Write("&#9496;");
                                                                else Write("&#9508;");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            using (Span(style: "color: #ccc; margin-right: 5px; font-size: 16px"))
                                                            {
                                                                if (i == userBusinesses.Length - 1) Write("&#9492;");
                                                                else Write("&#9500;");
                                                            }
                                                        }
                                                        using (A(href: new Businesses.Business.Settings.UserPermissions.GoToUserPermissions() { Business = userBusinesses[i], Username = e.Username, Referrer = referrer }.ToUrl())) Write(userBusinesses[i]);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                using (Div(style: "font-size: 12px; line-height: 16px; padding: 0px; margin: 0px"))
                                                {
                                                    if (ManagerServer.Globalization.Languages.IsRightToLeft()) using (Span(style: "color: #ccc; margin-left: 5px; font-size: 16px")) Write("&#9496;");
                                                    else using (Span(style: "color: #ccc; margin-right: 5px; font-size: 16px")) Write("&#9492;");

                                                    using (Span(style: "color: #999")) Write("No businesses");
                                                }
                                            }
                                        }
                                    }
                                }
                                using (Td(@class: "text-end", style: "color: #999; vertical-align: top; padding-top: 10px"))
                                {
                                    if (e.Type == ManagerServer.Model.UserType.Administrator)
                                    {
                                        using (Div(@class: "font-semibold text-xs")) Write(Strings.Administrator);
                                    }
                                    else
                                    {
                                        using (A(new Impersonate() { Username = e.Username }.ToUrl(), @class: "btn btn-xs")) Write(Strings.Impersonate);
                                    }
                                }
                            }
                        }
                        using (Tr())
                        {
                            using (Td(colspan: 3, style: "padding: 0px")) Hr();
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
