using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Attributes
{
    internal class CheckboxScreenshotAttribute : ManagerServer.Attributes.ScreenshotAttribute
    {
        public CheckboxScreenshotAttribute(string name)
        {
            using (Div(@class: "border rounded p-0.5 inline-block pointer-events-none"))
            {
                using (Div(@class: "p-4 text-sm bg-neutral-50"))
                {
                    using (Div(@class: "my-2 flex gap-1.5"))
                    {
                        Write($@"<input type=""checkbox"" checked=""checked"">");
                        using (Label(@class: "font-semibold")) Keyword(name);
                    }
                }
            }
        }
    }
}