using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Attributes
{
    internal class PrimaryButtonScreenshotAttribute : ManagerServer.Attributes.ScreenshotAttribute
    {
        public PrimaryButtonScreenshotAttribute(string name)
        {
            using (Div(@class: "border rounded p-0.5 inline-block pointer-events-none"))
            {
                using (Div(@class: "bg-neutral-200 text-neutral-900 p-8 text-sm"))
                {
                    using (Span(@class: "border font-semibold rounded-md py-3 px-4 bg-[#428bca] border-[#357ebd] text-white"/*, style: "box-shadow: inset 0px 1px 0px rgba(255,255,255,0.5)"*/))
                    {
                        Keyword(name);
                    }
                }
            }
        }
    }
}