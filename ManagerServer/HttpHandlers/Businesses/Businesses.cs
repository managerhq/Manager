using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer;
using ManagerServer.HttpHandlers.Businesses.Business;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses
{
    [ProtoContract]
    [Title(nameof(Strings.Businesses))]
    [Guide("The `Businesses` tab is the first screen you see when you open the app. It serves as your gateway to access and manage all your business entities.")]
    [TopLevelTabScreenshot(icon: "fa-building", name: nameof(Strings.Businesses))]
    [Guide("This screen displays a list of all the businesses you have added. To work with a specific business, simply click on its name.")]
    [Header("Managing Businesses")]
    [Guide("To create a new business, click the `Add Business` button and select `Create New Business` from the drop-down menu.")]
    [LinkGuide("Learn more:", typeof(CreateNewBusiness))]
    [Guide("To import an existing business from a backup file, click the `Add Business` button, then select `Import Business`.")]
    [LinkGuide("Learn more:", typeof(ImportBusiness))]
    [Guide("To delete a business, click the `Remove Business` button. Be careful—this action cannot be undone.")]
    [LinkGuide("Learn more:", typeof(RemoveBusiness))]
    [Header("Data Management")]
    [Guide("Regular backups are essential to protect your data. If you use `Desktop Edition`, you must manually back up your businesses. `Cloud Edition` automatically backs up your data, but you can still create manual backups for extra security.")]
    [LinkGuide("Learn more:", typeof(Backup))]
    [Guide("Over time, as you add and delete transactions, customers, suppliers, and other data, your business file may become larger than necessary. You can compact the file size by clicking on the file size displayed next to any business name.")]
    [LinkGuide("Learn more:", typeof(Vacuum))]
    [Guide("If you use `Desktop Edition`, your data is stored in the default application data folder. The location varies by operating system, but you can move it by clicking the `Change Folder` button. This allows you to store your data in cloud-synced folders like Dropbox, OneDrive, Google Drive, or iCloud for automatic backup.")]
    [Header("User Access and Permissions")]
    [Guide("If you're logged in as an `Administrator` on `Cloud Edition` or `Server Edition`, you can see all businesses. Non-administrator users only see businesses assigned to them by the administrator through the `Users` tab.")]
    [LinkGuide("Learn more:", typeof(Users.Users))]
    [Header("Troubleshooting")]
    [Guide("Manager may refuse to open a business database if it has become corrupted.")]
    [LinkGuide("Learn more:", typeof(Corrupt))]
    [Guide("Manager cannot open business databases created with newer versions of the software. You must update your Manager version first.")]
    [LinkGuide("Learn more:", typeof(NewerVersionRequired))]
    public sealed class Businesses : Template
    {
        protected override async Task InnerGet()
        {
            try
            {
                ApplicationData.Businesses.Refresh();
            }
            catch (Exception ex)
            {
                using (Div(@class: "text-center p-8 text-red-800"))
                {
                    using (P()) Write(ex.InnerException?.Message ?? ex.Message);
                    using (P()) Write(ex.InnerException?.ToString() ?? ex.ToString());
                }
                return;
            }


            var businesses = ApplicationData.Businesses.GetAll();
            var currentUser = this.GetCurrentUser();

            if (currentUser != null && currentUser.Type == ManagerServer.Model.UserType.Restricted)
            {
                var currentUserBusinesses = currentUser.Businesses ?? new string[0];
                businesses = businesses.Where(x => currentUserBusinesses.Contains(x)).ToArray();
            }

            using (Div(@class: "p-8")) using (Div(@class: "flex flex-col space-y-4 max-w-prose mx-auto"))
            {
                using (Div(@class: "flex items-end gap-8 justify-between"))
                {
                    using (Div(@class: "font-semibold text-shadow text-lg text-neutral-400 px-3"))
                    {
                        using (Div(@class: "flex gap-4 items-center"))
                        {
                            using (Span()) Write(Strings.Businesses);
                            WriteHelp(ApplicationData.Businesses.GetAll().Length == 0);
                        }
                    }

                    if (currentUser == null || currentUser.Type == ManagerServer.Model.UserType.Administrator)
                    {
                        using (Div(@class: "flex gap-2 justify-end items-center print:hidden"))
                        {
                            using (Details(@class: "dropdown"))
                            {
                                using (Summary(@class: "btn btn-primary"))
                                {
                                    Write(Strings.AddBusiness);
                                }
                                using (Div(@class: "dropdown-menu"))
                                {
                                    using (A(href: new CreateNewBusiness().ToUrl(), @class: "dropdown-item"))
                                    {
                                        Write(Strings.CreateNewBusiness);
                                    }
                                    Hr(@class: "my-2");
                                    using (A(href: new ImportBusiness().ToUrl(), @class: "dropdown-item"))
                                    {
                                        Write(Strings.ImportBusiness);
                                    }
                                }
                            }

                            if (businesses.Any())
                            {
                                using (A(href: new RemoveBusiness().ToUrl(), @class: "btn btn-danger")) Write(Strings.RemoveBusiness);
                            }

                            using (A(href: new RemovedBusinesses().ToUrl(), @class: "opacity-25 hover:opacity-50 text-(--foreground)"))
                            {
                                I(@class: "fas fa-trash text-base");
                            }
                        }
                    }
                }

                if (businesses.Count() > 1)
                {
                    using (Div(@class: "flex flex-col space-y-4"))
                    {
                        Hr();
                        InputText(placeholder: Strings.Search, autofocus: true, @class: "p-1 rounded outline-none placeholder-shown:opacity-50", id: "search");
                    }
                }

                using (Table(@class: "font-semibold w-full"))
                {
                    if (!businesses.Any())
                    {
                        using (Tr())
                        {
                            using (Td(colspan: 3, @class: "p-0")) Hr();
                        }
                        using (Td(colspan: 3, style: "padding: 10px 0px"))
                        {
                            using (Div(@class: "text-center font-semibold text-neutral-300 p-32 text-xl"))
                            {
                                Write(Strings.Empty);
                            }
                        }
                        using (Tr())
                        {
                            using (Td(colspan: 3, @class: "p-0")) Hr();
                        }
                    }
                    else
                    {
                        char? lastChar = null;
                        foreach (var e in businesses.OrderBy(x => x))
                        {
                            var base64 = Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(e)).TrimEnd(new char[] { '=' }).Replace('+', '-').Replace('/', '_');

                            var name = e;
                            if (string.IsNullOrEmpty(name)) name = "-";
                            var currentChar = name.ToUpper()[0];
                            if (lastChar != currentChar)
                            {
                                using (Tr())
                                {
                                    using (Td(colspan: 3, @class: "p-0")) Hr();
                                }
                            }
                            using (Tr(@class: "business"))
                            {
                                using (Td(@class: "p-4 text-neutral-400 w-0"))
                                {
                                    if (lastChar != currentChar) Write(currentChar.ToString());
                                }
                                using (Td(style: "padding: 10px"))
                                {
                                    using (A(href: new Start() { Business = e }.ToUrl(), style: "font-size: 14px", @class: "block font-semibold p-2")) Write(name);
                                }
                                using (Td(@class: "whitespace-nowrap text-right rtl:text-left text-neutral-300"))
                                {
                                    var length = await ApplicationData.Businesses.GetFileSize(e);
                                    using (Span(id: "size_" + base64))
                                    {
                                        if (length < 1024 * 1024)
                                        {
                                            using (A(new Vacuum() { Business = e }.ToUrl(), @class: "text-neutral-400")) Write(length / 1024 + " KB");
                                        }
                                        else
                                        {
                                            using (A(new Vacuum() { Business = e }.ToUrl(), @class: "text-neutral-500", style: "cursor: pointer")) Write(length / 1024 / 1024 + " MB");
                                        }
                                    }
                                }
                            }
                            if (lastChar != currentChar)
                            {
                                lastChar = currentChar;
                            }
                        }
                        using (Tr())
                        {
                            using (Td(colspan: 3, @class: "p-0")) Hr();
                        }
                    }
                }
            }

        }
    }
}
