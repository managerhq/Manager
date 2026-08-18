using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerComponents
{
    public sealed class Footer : ComponentBase
    {
        public List<ComponentBase> StartElements = new();
        public List<ComponentBase> EndElements = new();

        // Layout classes
        private const string FlexBetween = "flex justify-between";
        private const string FlexItemsGap = "flex items-center gap-2";

        public override void BuildString(StringBuilder sb)
        {
            using (sb.Div(@class: "card-header"))
            {
                using (sb.Div(@class: FlexBetween))
                {
                    using (sb.Div(@class: FlexItemsGap))
                    {
                        foreach (var e in StartElements) e.BuildString(sb);
                    }
                    using (sb.Div(@class: FlexItemsGap))
                    {
                        foreach (var e in EndElements) e.BuildString(sb);
                    }
                }
            }
        }
    }
}