using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ManagerServer.Globalization;
using ManagerServer.HttpHandlers.Businesses.Business;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using static ManagerServer.ApplicationData;
using ManagerServer.Model.Master;

namespace ManagerServer.HttpHandlers.Businesses
{
    [ProtoContract]
    [Title(nameof(Strings.CreateNewBusiness))]
    [Guide("Manager allows you to create and manage multiple businesses within a single installation. Each business maintains its own separate accounting records, customers, suppliers, and settings.")]
    [Header("Getting Started")]
    [Guide("To create a new business, first navigate to the `Businesses` tab.")]
    [TopLevelTabScreenshot(icon: "fa-building", name: nameof(Strings.Businesses))]
    [Guide("Click the `Add Business` button and select `Create New Business` from the dropdown menu.")]
    [AddBusinessDropdownScreenshot]
    [Guide("Enter a meaningful name in the `Business Name` field. This name will help you identify this business when you have multiple businesses in Manager.")]
    [Guide("If available, select your country from the `Country` dropdown. This will automatically configure tax codes, chart of accounts, and other settings appropriate for your location.")]
    [Guide("Click the `Create New Business` button to complete the setup.")]
    [PrimaryButtonScreenshot(nameof(Strings.CreateNewBusiness))]
    [Header("Default Tabs")]
    [Guide("After creating your business, you'll be taken to the `Summary` tab.")]
    [TabScreenshot("fa-presentation", nameof(Strings.Summary))]
    [Guide("Four tabs are displayed by default:")]
    [Guide("• `Summary` — Overview of your business's financial position")]
    [LinkGuide("Learn more:", typeof(Business.Summary.SummaryView))]
    [Guide("• `JournalEntries` — Record accounting transactions")]
    [LinkGuide("Learn more:", typeof(Business.JournalEntries.JournalEntries))]
    [Guide("• `Reports` — Generate financial statements and other reports")]
    [LinkGuide("Learn more:", typeof(Business.Reports.Reports))]
    [Guide("• `Settings` — Configure accounts, preferences, and business details")]
    [LinkGuide("Learn more:", typeof(Business.Settings.Settings))]
    [Header("Basic vs Full Features")]
    [Guide("These default tabs provide a minimal double-entry accounting system. You can set up your `Chart of Accounts`, enter transactions through `Journal Entries`, and generate financial statements.")]
    [Guide("This basic configuration is ideal for accountants who need to quickly prepare financial statements from existing data.")]
    [Guide("Most businesses will benefit from enabling additional features such as sales invoicing, inventory tracking, purchase orders, and customer management.")]
    [Header("Customizing Your Business")]
    [Guide("To enable additional features, click the `Customize` button located in the navigation area.")]
    [DefaultTabsAndCustomizeScreenshot]
    [Guide("This opens a comprehensive list of available modules and features. You can enable only the features your business needs, keeping the interface clean and focused.")]
    [Guide("Features can be enabled or disabled at any time without losing data. This allows your system to grow with your business needs.")]
    [LinkGuide("For detailed information about customizing tabs, see:", typeof(TabsForm))]
    internal sealed class CreateNewBusiness : Template
    {
        [ProtoMember(1)] public string Error;

        protected override Task InnerGet()
        {
            using (Div(@class: "p-8 mx-auto max-w-prose"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-body p-8"))
                    {
                        var currentUser = this.GetCurrentUser();
                        if (currentUser != null && currentUser.Type == ManagerServer.Model.UserType.Restricted)
                        {
                            using (Div(style: "font-size: 24px; font-weight: bold; color: #333; padding-top: 20px")) Write("You are not authorised");
                            using (Div(style: "font-weight: bold; padding-top: 20px; line-height: 175%")) Write("You are not authorised to access this part of the system. Only administrators of <u>" + Request.Host + "</u> are allowed.");
                        }
                        else
                        {
                            using (Div(@class: "flex flex-col space-y-4"))
                            {
                                using (Div(@class: "text-xl font-bold")) Write(Strings.CreateNewBusiness);

                                Hr();

                                using (Div())
                                {
                                    using (Label()) Write(Strings.BusinessName);
                                    InputText(name: nameof(FormData.Name), form: nameof(Strings.CreateNewBusiness), @class: "form-control", placeholder: Strings.Unnamed, autofocus: true);
                                }                                

                                Hr();

                                using (Div(@class: "flex gap-4 items-center"))
                                {
                                    FormPrimaryButton(nameof(Strings.CreateNewBusiness));
                                    using (A(href: new Businesses().ToUrl(), @class: "btn")) Write(Strings.Cancel);
                                }

                                if (!string.IsNullOrWhiteSpace(Error))
                                {
                                    using (Div(@class: "text-red-600 font-bold")) Write(Error);
                                }
                            }
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        public sealed class FormData
        {
            public string Name;
        }

        protected override async Task InnerPost()
        {
            this.EnsureCurrentUserNotRestricted();
            var form = await Request.ReadFormAsync();
            var formData = new FormData()
            {
                Name = form[nameof(FormData.Name)]
            };

            var name = formData.Name;
            if (string.IsNullOrWhiteSpace(name)) name = Strings.Unnamed;

            if (await ApplicationData.Businesses.FileExists(name))
            {
                Response.Redirect(new CreateNewBusiness() { Error = "Business name already exists." }.ToUrl());
                return;
            }

            if (!ApplicationData.Businesses.IsValidName(name))
            {
                Response.Redirect(new CreateNewBusiness() { Error = "The name you have entered contains a folder separator (e.g., '/' or '\\'). Folder separators are not allowed in names. Please remove any '/' or '\\' characters and try again." }.ToUrl());
                return;
            }

            await ApplicationData.Businesses.CreateAsync(name);

            using (var db = ApplicationData.Businesses.SQLiteConnection(name))
            {
                db.Pragma("auto_vacuum = INCREMENTAL");
                using (var tx = db.BeginTransaction())
                {
                    tx.CreateTable<ManagerServer.ApplicationData.Object>();
                    tx.CreateTable<ManagerServer.ApplicationData.Blob>();
                    tx.CreateTable<ManagerServer.ApplicationData.Change>();
                    tx.CreateTable<ManagerServer.ApplicationData.Email>();
                    tx.CreateTable<ManagerServer.ApplicationData.Image>();
                    tx.Commit();
                }
            }

            ApplicationData.Businesses.Refresh();            

            Response.Redirect(new Start() { Business = name }.ToUrl());
        }
    }
}