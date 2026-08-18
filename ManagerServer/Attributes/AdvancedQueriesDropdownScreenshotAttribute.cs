using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;

namespace ManagerServer.Attributes
{
    internal class AdvancedQueriesDropdownScreenshotAttribute : ManagerServer.Attributes.ScreenshotAttribute
    {
        public AdvancedQueriesDropdownScreenshotAttribute()
        {
            using (Div(@class: "border rounded p-0.5 inline-block pointer-events-none"))
            {
                using (Div(@class: "bg-neutral-100 p-8 text-sm"))
                {
                    using (Details())
                    {
                        using (Summary(@class: "list-item text-neutral-500"))
                        {
                            Keyword(nameof(Strings.AdvancedQueries));
                        }
                        using (Div(@class: "mt-2 py-2 rounded-md shadow-lg bg-white"))
                        {
                            using (Div(@class: "px-8 py-2 text-white bg-[#428BCA] whitespace-nowrap"))
                            {
                                Keyword(nameof(Strings.NewAdvancedQuery));
                            }
                        }
                    }
                }
            }
        }
    }
}