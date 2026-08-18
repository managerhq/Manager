using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;

namespace ManagerServer.Attributes
{
    public class AddBusinessDropdownScreenshotAttribute : ScreenshotAttribute
    {
        public AddBusinessDropdownScreenshotAttribute()
        {
            using (Div(@class: "border rounded p-0.5 inline-block pointer-events-none"))
            {
                using (Div(@class: "bg-neutral-200 text-neutral-900 p-8 text-sm"))
                {
                    using (Details())
                    {
                        using (Summary(@class: "border font-bold rounded-md py-3 px-4 bg-[#428bca] border-[#357ebd] text-white [box-shadow:_inset_0px_1px_0px_rgba(255,255,255,0.5)]"))
                        {
                            Keyword(nameof(Strings.AddBusiness));
                        }
                        using (Div(@class: "mt-2 py-2 rounded-md shadow-lg bg-white"))
                        {
                            using (Div(@class: "px-8 py-2 text-white bg-[#428BCA] whitespace-nowrap"))
                            {
                                Keyword(nameof(Strings.CreateNewBusiness));
                            }
                            Hr(@class: "my-2");
                            using (Div(@class: "px-8 py-2 text-gray-700 whitespace-nowrap"))
                            {
                                Keyword(nameof(Strings.ImportBusiness));
                            }
                        }
                    }
                }
            }
        }
    }
}