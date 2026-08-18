using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Attributes
{
    internal class SmallBottomButtonScreenshotAttribute : ManagerServer.Attributes.ScreenshotAttribute
    {
        public SmallBottomButtonScreenshotAttribute(string name)
        {
            using (Div(@class: "border rounded p-0.5 pb-4 inline-block pointer-events-none"))
            {
                using (Div(@class: "bg-neutral-100 p-3 border-b"))
                {
                    using (Div(@class: "flex items-center gap-4"))
                    {
                        using (Span(@class: "border text-xs rounded py-1 px-2 bg-white border-neutral-300 text-neutral-400"))
                        {
                            Keyword(name);
                        }
                    }
                }
            }
        }
    }
}