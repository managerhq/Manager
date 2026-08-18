using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerComponents
{
    public sealed class InputGroup : ComponentBase
    {
        public List<ComponentBase> Children = new();

        public override void BuildString(StringBuilder sb)
        {
            using (sb.Div(@class: "input-group flex items-center"))
            {
                foreach (var e in Children)
                {
                    e.BuildString(sb);
                }
            }
        }
    }
}
