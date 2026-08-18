using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;

namespace ManagerServer.Attributes
{
    internal class SettingsItemScreenshotAttribute : ManagerServer.Attributes.ScreenshotAttribute
    {
        public SettingsItemScreenshotAttribute(string icon, string name, string innerIcon = null, string innerName = null)
        {
            using (Div(@class: "flex pointer-events-none space-x-6 rtl:space-x-reverse items-center"))
            {
                using (Div(@class: "border rounded p-0.5 inline-block ltr:pr-8 rtl:pl-8"))
                {
                    using (Div(@class: "bg-neutral-200 ltr:pl-8 rtl:pr-8"))
                    {
                        using (Div(@class: "border-x border-neutral-300 bg-neutral-100 h-2"))
                        {
                        }
                        using (Div(@class: "border-y ltr:border-l rtl:border-r border-neutral-300 bg-white p-4 flex gap-2 items-center"))
                        {
                            I(@class: "fas fa-cog text-neutral-400");
                            using (Span(@class: "font-semibold text-sm text-[#428bca]"))
                            {
                                Keyword(nameof(Strings.Settings));
                            }
                        }
                        using (Div(@class: "border-x border-neutral-300 bg-neutral-100 h-2"))
                        {
                        }
                    }
                }

                using (Span(@class: "rtl:hidden")) I(@class: "fas fa-circle-chevron-right text-lg text-neutral-300");
                using (Span(@class: "ltr:hidden")) I(@class: "fas fa-circle-chevron-left text-lg text-neutral-300");

                using (Div(@class: "border rounded inline-block pointer-events-none"))
                {
                    using (Div(@class: "basis-72 flex items-center gap-2 p-4"))
                    {
                        I(@class: $"text-xl text-neutral-400 fas fa-fw {icon}");
                        using (Span(@class: "text-sm text-[#428bca]"))
                        {
                            Keyword(name);
                        }
                    }
                }

                if (innerName != null && innerIcon != null)
                {
                    using (Span(@class: "rtl:hidden")) I(@class: "fas fa-circle-chevron-right text-lg text-neutral-300");
                    using (Span(@class: "ltr:hidden")) I(@class: "fas fa-circle-chevron-left text-lg text-neutral-300");

                    using (Div(@class: "border rounded inline-block pointer-events-none"))
                    {
                        using (Div(@class: "basis-72 flex items-center gap-2 p-4"))
                        {
                            I(@class: $"text-xl text-neutral-400 fas fa-fw {innerIcon}");
                            using (Span(@class: "text-sm text-[#428bca]"))
                            {
                                Keyword(innerName);
                            }
                        }
                    }
                }
            }
        }
    }
}