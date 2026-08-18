using ManagerServer.Globalization;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers
{
    abstract class LoginTemplate : Template
    {
        protected virtual void InnerInnerGet()
        {
        }

        protected sealed override async Task InnerGet()
        {
            using (Style())
            {
                CssRule(":root.dark img", "filter: sepia(45%) hue-rotate(160deg) invert(90%) grayscale(100%)");
            }

            var logoExists = await ApplicationData.Assets.ExistsAsync("logo.png");

            using (Div(@class: "p-8 mx-auto max-w-prose"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-body p-8"))
                    {
                        using (Div(@class: "flex flex-col space-y-4"))
                        {
                            if (logoExists)
                            {
                                using (Div(@class: "flex justify-center pb-4")) Img(src: new Logo().ToUrl(), style: "max-width: 100%");
                            }
                            else
                            {
                                using (Div(@class: "text-xl font-bold")) Write(Strings.Login);
                                Hr();
                            }

                            using (Form(@class: "flex flex-col space-y-4", method: "POST", hxBoost: true, hxDisabledElt: "button"))
                            {
                                InnerInnerGet();
                            }
                        }
                    }
                }
            }

        }
    }
}
