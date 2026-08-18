using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerComponents
{
    public sealed class Notice : ComponentBase
    {
        public required string Text;
        public string CancelUrl;

        public override void BuildString(StringBuilder sb)
        {
            using (sb.Div(@class: "card-header p-0"))
            {
                using (sb.Div(@class: "flex items-center"))
                {
                    if (CancelUrl != null)
                    {
                        using (sb.A(href: CancelUrl, @class: "py-4 px-6 text-(--muted-foreground) opacity-25 hover:opacity-50"))
                        {
                            sb.I(@class: "fas fa-xmark text-base");
                        }
                        using (sb.Div(@class: "vertical-divider"))
                        {
                        }
                    }
                    using (sb.Div(@class: "px-4"))
                    {
                        sb.Append(Text);
                    }
                }
            }
        }
    }
}