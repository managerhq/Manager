using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;

namespace ManagerServer.Attributes
{
    public class ColumnScreenshotAttribute : ManagerServer.Attributes.ScreenshotAttribute
    {
        public ColumnScreenshotAttribute(string name, object value)
        {
            using (Div(@class: "my-4"))
            {
                using (Div(@class: "border rounded p-0.5 pointer-events-none text-sm w-max"))
                {
                    using (Div(@class: "grid grid-cols-[1.25rem_auto_1.25rem] bg-neutral-200 gap-[1px]"))
                    {
                        for (int i = 0; i < 3; i++) using (Div(@class: "bg-neutral-100 h-2")) { }

                        using (Div(@class: "bg-neutral-100 border-t border-t-white")) { }
                        using (Div(@class: "bg-neutral-100 font-bold text-neutral-500 text-shadow px-4 py-2 border-t border-t-white"))
                        {
                            Keyword(name);
                        }
                        using (Div(@class: "bg-neutral-100 border-t border-t-white")) { }
                        if (value != null)
                        {
                            using (Div(@class: "bg-neutral-50 border-t border-t-white")) { }
                            using (Div(@class: "bg-neutral-50 border-t border-t-white text-center p-2 text-[#428bca]"))
                            {
                                Write(value.ToString());
                            }
                            using (Div(@class: "bg-neutral-50 border-t border-t-white")) { }
                            for (int i = 0; i < 3; i++) using (Div(@class: "bg-white h-2")) { }
                        }
                        else
                        {
                            for (int i = 0; i < 3; i++) using (Div(@class: "bg-neutral-50 border-t border-white h-2")) { }
                        }
                    }
                }
            }
        }        
    }
}