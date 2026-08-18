using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerComponents
{
    public sealed class FooterButton : ComponentBase
    {
        public required string Text;
        public string Url;
        public string OnClick;
        public List<Tuple<string, string>> Menu = new();

        public override void BuildString(StringBuilder sb)
        {
            if (Url != null)
            {
                using (sb.A(href: Url, @class: "btn btn-xs"))
                {
                    sb.Append(Text);
                }
            }
            else if (OnClick != null)
            {
                using (sb.Button(onclick: OnClick, @class: "btn btn-xs"))
                {
                    sb.Append(Text);
                }
            }
            else if (Menu.Count > 0)
            {
                using (sb.Details(@class: "dropdown"))
                {
                    using (sb.Summary(@class: "btn btn-xs"))
                    {
                        sb.Append(Text);
                    }
                    using (sb.Div(@class: "dropdown-menu"))
                    {
                        foreach (var e in Menu)
                        {
                            if (e == null)
                            {
                                sb.Hr();
                            }
                            else
                            {
                                using (sb.A(href: e.Item2, @class: "dropdown-item")) sb.Append(e.Item1);
                            }
                        }
                    }
                }
            }
            else
            {
                using (sb.Button(@class: "btn btn-xs", disabled: true))
                {
                    sb.Append(Text);
                }
            }
        }
    }
}
