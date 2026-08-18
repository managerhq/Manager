using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;

namespace ManagerServer.Attributes
{
    public class SelectAccountScreenshotAttribute : ScreenshotAttribute
    {
        public SelectAccountScreenshotAttribute(string accountName = null, string prepend = null)
        {
            using (Div(@class: "border rounded p-0.5 inline-block pointer-events-none"))
            {
                using (Div(@class: "p-4 text-sm bg-neutral-50"))
                {
                    using (Div(@class: "my-2"))
                    {
                        using (Label(@class: "font-bold")) Keyword(nameof(Strings.Account));
                    }

                    using (Div(@class: "flex gap-1"))
                    {
                        using (Div(@class: "bg-white shadow-inner border border-gray-300 text-gray-900 rounded p-2 flex gap-8"))
                        {
                            using (Span())
                            {
                                Keyword(accountName);
                            }
                            using (Div(@class: "flex gap-2 items-center"))
                            {
                                I(@class: "fas fa-xmark text-gray-400");
                                I(@class: "fas fa-caret-down text-gray-400");
                            }
                        }

                        if (prepend != null)
                        {
                            using (Div(@class: "flex items-center items-stretch"))
                            {
                                using (Div(@class: "bg-neutral-100 border border-gray-300 p-2 rounded-s"))
                                {
                                    Keyword(prepend);
                                }

                                using (Div(@class: "bg-white shadow-inner border border-s-0 border-gray-300 text-gray-900 rounded-e p-2 ps-16 flex items-center"))
                                {
                                    I(@class: "fas fa-caret-down text-gray-400");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}