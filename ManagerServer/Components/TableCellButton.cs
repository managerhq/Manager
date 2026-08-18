using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerComponents
{
    public sealed class TableCellButton : ComponentBase
    {
        public required string Text;
        public required string Url;

        public override void BuildString(StringBuilder sb)
        {
            using (sb.A(href: Url, @class: "btn btn-sm"))
            {
                sb.Append(Text);
            }
        }
    }
}
