using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerComponents
{
    public sealed class PanelLabel : ComponentBase
    {
        public required string Text;

        public override void BuildString(StringBuilder sb)
        {
            using (sb.Div(@class: "text-neutral-300 font-semibold"))
            {
                sb.Append(Text);
            }
        }
    }
}