using System.Linq;
using System.Threading.Tasks;
using System.IO;
using ManagerServer.Globalization;
using ManagerServer;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses
{
    [ProtoContract]
    [Title(nameof(Strings.RemoveBusiness))]
    [Guide("The `Remove Business` screen allows you to delete an existing business from Manager. To remove a business, select it from the dropdown menu and click the `Remove Business` button.")]
    [DangerButtonScreenshot(nameof(Strings.RemoveBusiness))]
    [Header("Data Safety")]
    [Guide("Manager does not permanently delete your data. When you remove a business, it is moved to the `Trash` folder within your data folder. This ensures your business data remains recoverable if needed.")]
    [Header("Restoring Removed Businesses")]
    [Guide("To restore a previously removed business, navigate to the `Trash` folder in your data folder and move the business file back to the main data folder. The business will then appear in your business list again.")]
    [Guide("If you are using `Cloud Edition`, you cannot directly access the `Trash` folder because your data is stored in the cloud. To restore a removed business in `Cloud Edition`, visit https://cloud.manager.io, log in to your account, and click the `Restore Business` button.")]
    internal sealed class RemoveBusiness : Template
    {
        [ProtoMember(1)] public string Error;

        protected override Task InnerGet()
        {
            var currentUser = this.GetCurrentUser();
            if (currentUser != null && currentUser.Type == ManagerServer.Model.UserType.Restricted)
            {
                using (Div(style: "padding: 50px; background-color: #fff; box-shadow: 0 1px 4px rgba(0, 0, 0, 0.067); border: 1px solid #ccc; text-align: center"))
                {
                    using (Div(style: "font-size: 24px; font-weight: bold; color: #333; padding-top: 20px")) Write("You are not authorised");
                    using (Div(style: "font-weight: bold; padding-top: 20px; line-height: 175%")) Write("You are not authorised to access this part of the system. Only administrators of <u>" + Request.Host + "</u> are allowed.");
                }
            }
            else
            {
                using (Div(@class: "p-8 mx-auto max-w-prose"))
                {
                    using (Div(@class: "card"))
                    {
                        using (Div(@class: "card-body p-8"))
                        {
                            using (Div(@class: "flex flex-col space-y-4"))
                            {
                                using (Div(@class: "text-xl font-bold")) Write(Strings.RemoveBusiness);

                                Hr();

                                using (Div())
                                {
                                    using (Label()) Write(Strings.Business);
                                    using (Select(name: "Name", @class: "form-select", form: nameof(Strings.RemoveBusiness)))
                                    {
                                        Option();
                                        foreach (var e in ApplicationData.Businesses.GetAll().OrderBy(x => x))
                                        {
                                            Option(value: e, text: e);
                                        }
                                    }
                                }

                                Hr();

                                using (Div(@class: "flex gap-4 items-center"))
                                {
                                    FormDangerButton(nameof(Strings.RemoveBusiness));
                                    using (A(href: new Businesses().ToUrl(), @class: "btn")) Write(Strings.Cancel);
                                }

                                if (!string.IsNullOrEmpty(Error))
                                {
                                    using (Div(@class: "text-red-500 font-bold")) Write(Error);
                                }
                            }
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        protected override async Task InnerPost()
        {
            this.EnsureCurrentUserNotRestricted();
            var businessName = Request.Form["Name"].ToString();
            if (string.IsNullOrWhiteSpace(businessName))
            {
                Response.Redirect(new RemoveBusiness().ToUrl());
                return;
            }

            try
            {
                await ApplicationData.RemoveBusiness(businessName);
            }
            catch (IOException ex)
            {
                Response.Redirect(new RemoveBusiness() { Error = ex.Message }.ToUrl());
                return;
            }

            Response.Redirect(new Businesses().ToUrl());
        }
    }
}
