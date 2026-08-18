using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;

namespace ManagerServer.Attributes
{
    internal class DefaultTabsAndCustomizeScreenshotAttribute : ManagerServer.Attributes.ScreenshotAttribute
    {
        public DefaultTabsAndCustomizeScreenshotAttribute()
        {
            using (Div(@class: "border rounded p-0.5 inline-block pointer-events-none ltr:pr-8 rtl:pl-8"))
            {
                using (Div(@class: "bg-neutral-200 ltr:pl-8 rtl:pr-8"))
                {
                    using (Div(@class: "border-x border-neutral-300 bg-neutral-100 h-2"))
                    {
                    }
                    using (Div(@class: "border-y ltr:border-l rtl:border-r border-neutral-300 bg-white p-3 flex gap-2 items-center"))
                    {
                        I(@class: "fas fa-presentation text-neutral-400");
                        using (Span(@class: "font-semibold text-sm text-[#428bca]")) Keyword(nameof(Strings.Summary));
                    }
                    using (Div(@class: "border-x border-b border-neutral-300 bg-neutral-50 p-3 flex gap-2 items-center"))
                    {
                        I(@class: "fas fa-balance-scale text-neutral-400");
                        using (Span(@class: "font-semibold text-sm text-[#428bca]")) Keyword(nameof(Strings.JournalEntries));
                        using (Span(@class: "whitespace-nowrap border font-normal tabular-nums py-0.5 px-2 bg-white rounded-lg text-neutral-300 border-neutral-200 text-xs")) Write("0");
                    }
                    using (Div(@class: "border-x border-b border-neutral-300 bg-neutral-50 p-3 flex gap-2 items-center"))
                    {
                        I(@class: "fas fa-print text-neutral-400");
                        using (Span(@class: "font-semibold text-sm text-[#428bca]")) Keyword(nameof(Strings.Reports));
                    }
                    using (Div(@class: "border-x border-b border-neutral-300 bg-neutral-50 p-3 flex gap-2 items-center"))
                    {
                        I(@class: "fas fa-cog text-neutral-400");
                        using (Span(@class: "font-semibold text-sm text-[#428bca]")) Keyword(nameof(Strings.Settings));
                    }
                    using (Div(@class: "border-e border-neutral-300 bg-neutral-200 text-center pb-4"))
                    {
                        using (Div(@class: "py-4 font-semibold text-[#428bca] text-sm"))
                        {
                            Keyword(nameof(Strings.Customize));
                        }

                        I(@class: "fas fa-hand-pointer text-neutral-400 text-xl");
                    }                    
                }
            }
        }
    }
}