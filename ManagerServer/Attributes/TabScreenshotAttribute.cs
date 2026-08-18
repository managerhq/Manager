using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Attributes
{
    internal class TabScreenshotAttribute : ManagerServer.Attributes.ScreenshotAttribute
    {
        public TabScreenshotAttribute(string icon, string name)
        {
            using (Div(@class: "border rounded p-0.5 inline-block pointer-events-none ltr:pr-8 rtl:pl-8"))
            {
                using (Div(@class: "bg-neutral-200 ltr:pl-8 rtl:pr-8"))
                {
                    using (Div(@class: "border-x border-neutral-300 bg-neutral-100 h-2"))
                    {
                    }
                    using (Div(@class: "border-y ltr:border-l rtl:border-r border-neutral-300 bg-white p-4 flex gap-2 items-center"))
                    {
                        I(@class: $"fas {icon} text-neutral-400");
                        using (Span(@class: "font-semibold text-sm text-[#428bca]"))
                        {
                            Keyword(name);
                        }
                    }
                    using (Div(@class: "border-x border-neutral-300 bg-neutral-100 h-2"))
                    {
                    }
                }
            }
        }
    }
}