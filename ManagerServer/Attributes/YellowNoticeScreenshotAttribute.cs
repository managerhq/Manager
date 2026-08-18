using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Attributes
{
    internal class YellowNoticeScreenshotAttribute : ManagerServer.Attributes.ScreenshotAttribute
    {
        public YellowNoticeScreenshotAttribute(string text)
        {
            using (Div(@class: "border rounded p-0.5 inline-block pointer-events-none"))
            {
                using (Div(@class: "flex items-center gap-2 bg-yellow-50 p-4"))
                {
                    I(@class: "fas fa-fw fa-circle-exclamation text-neutral-400 text-lg");
                    using (Span(@class: "font-semibold text-sm text-sky-600"))
                    {
                        Keyword(text);
                    }
                }
            }
        }
    }
}