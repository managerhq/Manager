using System;
using System.Text;
using ManagerServer.Attributes;

namespace ManagerServer.Attributes
{
    public class HeroButtonScreenshotAttribute : ScreenshotAttribute
    {
        public HeroButtonScreenshotAttribute(string title, string name)
        {
            using (Div(@class: "border rounded p-0.5 inline-block pointer-events-none"))
            {
                using (Div(@class: "mt-4 ms-4 pe-8 items-center bg-neutral-100 p-4 border-neutral-200 border-s border-t rounded-ss-lg flex gap-4 shadow-[inset_0px_1px_0px_#fff]"))
                {
                    using (Span(@class: "text-base font-bold text-neutral-300 [text-shadow:_1px_1px_0px_#fff]"))
                    {
                        Keyword(title);
                    }
                    using (Span(@class: "text-neutral-300 hover:text-neutral-400"))
                    {
                        Write(@"<i class=""fas fa-circle-question"" style=""font-size: 16px""></i>");
                    }
                    using (Span(@class: "bg-white text-neutral-700 border border-neutral-300 font-semibold rounded-md py-2 px-4 text-sm"))
                    {
                        Keyword(name);
                    }
                }
            }
        }
    }
}