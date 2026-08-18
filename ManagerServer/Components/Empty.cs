using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerComponents
{
    public sealed class Empty : ComponentBase
    {
        public required string Text;

        public override void BuildString(StringBuilder sb)
        {
            using (sb.Div(@class: "card-inset p-24 text-center"))
            {
                using (sb.Span(@class: "card-title text-xl"))
                {
                    sb.Append(Text);
                }
            }
        }
    }
}
