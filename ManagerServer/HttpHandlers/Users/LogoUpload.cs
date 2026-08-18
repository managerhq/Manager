using System;
using System.Linq;
using ManagerServer.Globalization;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer;

namespace ManagerServer.HttpHandlers.Users
{
    [ProtoContract]
    [Title(nameof(Strings.Logo))]
    [Guide("The `Logo` screen allows you to upload a logo which your users will see on your login screen.")]
    [Header("Accessing the Logo Screen")]
    [Guide("To access the `Logo` screen, go to the `Users` tab.")]
    [TopLevelTabScreenshot("fa-people-group", nameof(Strings.Users))]
    [Guide("Then click the image icon next to the `New User` button.")]
    [Header("Uploading Your Logo")]
    [Guide("Select your image file and click the `Update` button.")]
    [SuccessButtonScreenshot(nameof(Strings.Update))]
    [Guide("Your image must be in PNG format and less than 1 MB in size.")]
    internal sealed class LogoUpload : Template
    {
        [ProtoMember(1)] public string Error;

        protected override async Task InnerGet()
        {
            var currentUser = this.GetCurrentUser();
            if (currentUser.Type != ManagerServer.Model.UserType.Administrator)
            {
                Response.Redirect("/");
                return;
            }

            using (Style())
            {
                CssRule(":root.dark img", "filter: sepia(45%) hue-rotate(160deg) invert(90%) grayscale(100%)");
            }

            using (Div(@class: "p-8 mx-auto max-w-prose"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-body p-8"))
                    {
                        using (Form(method: "POST", enctype: HttpFramework.Enctype.multipartformdata))
                        {
                            using (Div(@class: "flex flex-col space-y-4"))
                            {
                                using (Div(@class: "text-xl font-bold")) Write(Strings.Logo);

                                Hr();

                                if (!await ApplicationData.Assets.ExistsAsync("logo.png"))
                                {
                                    InputFile(name: "Logo", @class: "form-file");
                                }
                                else
                                {
                                    using (Div(@class: "flex justify-center")) Img(src: new Logo().ToUrl(), style: "max-width: 100%");
                                }

                                Hr();

                                using (Div(@class: "flex gap-4 items-center"))
                                {
                                    if (!await ApplicationData.Instance.Assets.ExistsAsync("logo.png"))
                                    {
                                        using (SuccessButton()) Write(Strings.Update);
                                    }
                                    else
                                    {
                                        using (DangerButton()) Write(Strings.Delete);
                                    }
                                }

                                if (Error != null)
                                {
                                    using (Div(@class: "text-red-500 font-bold")) Write(Error);
                                }
                            }
                        }
                    }
                }
            }

        }

        protected override async Task InnerPost()
        {
            if (this.GetCurrentUser().Type != ManagerServer.Model.UserType.Administrator)
            {
                Response.Redirect("/");
                return;
            }

            if (await ApplicationData.Assets.ExistsAsync("logo.png"))
            {
                await ApplicationData.Assets.DeleteAsync("logo.png");
                Response.Redirect(new Users().ToUrl());
                return;
            }

            if (!Request.HasFormContentType)
            {
                Response.Redirect(new LogoUpload() { Error = "Incorrect Content-Type" }.ToUrl());
                return;
            }

            var form = await Request.ReadFormAsync();

            if (!form.Files.Any())
            {
                Response.Redirect(new LogoUpload() { Error = "No file" }.ToUrl());
                return;
            }

            if (!form.Files[0].FileName.ToLowerInvariant().EndsWith(".png"))
            {
                Response.Redirect(new LogoUpload() { Error = "The file needs to have PNG extension" }.ToUrl());
                return;
            }

            if (form.Files[0].Length > (1024 * 1024 * 1024))
            {
                Response.Redirect(new LogoUpload() { Error = "Please ensure the file is less than 1 MB in size" }.ToUrl());
                return;
            }

            using (var source = form.Files[0].OpenReadStream())
            {
                await ApplicationData.Assets.WriteAsync("logo.png", source);
            }

            Response.Redirect(this.ToUrl());
        }
    }
}