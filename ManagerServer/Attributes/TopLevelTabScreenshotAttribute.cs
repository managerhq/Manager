using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;

namespace ManagerServer.Attributes
{
    internal class TopLevelTabScreenshotAttribute : ManagerServer.Attributes.ScreenshotAttribute
    {
        public TopLevelTabScreenshotAttribute(string icon, string name)
        {
            using (Div(@class: "border rounded p-0.5 inline-block pointer-events-none"))
            {
                using (Div(@class: "bg-neutral-200 pb-6"))
                {
                    using (Div(@class: "overflow-visible bg-gradient-to-b from-white to-neutral-100 shadow shadow-neutral-400"))
                    {
                        using (Div(@class: "flex gap-2 items-center mx-8 p-6 text-neutral-500 whitespace-nowrap bg-neutral-200 shadow-inner shadow-neutral-400"))
                        {
                            I(@class: $"text-base fas {icon}");
                            using (Span(@class: "text-sm font-semibold"))
                            {
                                Keyword(name);
                            }
                        }
                    }
                }
            }
        }
    }
}