using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;

namespace ManagerServer.Attributes
{
    internal class DefaultButtonScreenshotAttribute : ManagerServer.Attributes.ScreenshotAttribute
    {
        public DefaultButtonScreenshotAttribute(string name)
        {
            using (Div(@class: "border rounded p-0.5 inline-block pointer-events-none"))
            {
                using (Div(@class: "bg-neutral-200 text-neutral-900 p-8 text-sm"))
                {
                    using (Span(@class: "bg-white font-semibold border border-neutral-300 text-neutral-700 rounded py-2 px-4"))
                    {
                        Keyword(name);
                    }
                }
            }
        }
    }
}