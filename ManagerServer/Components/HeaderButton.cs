using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerComponents
{
    public sealed class HeaderButton : ComponentBase
    {
        public string Text;
        public string Icon;
        public string Url;
        public string Form;
        public ButtonStyle Style = ButtonStyle.Default;

        public override void BuildString(StringBuilder sb)
        {
            var btnClass = "btn";
            if (Style == ButtonStyle.Success) btnClass += " btn-success";
            if (Style == ButtonStyle.Danger) btnClass += " btn-danger";
            if (Style == ButtonStyle.Primary) btnClass += " btn-primary";

            if (Form != null)
            {
                using (sb.Form(method: "POST", action: Url, id: Form, hxBoost: "true", hxDisabledElt: $"#{Form} button"))
                {
                    using (sb.Button(@class: btnClass))
                    {
                        sb.Append(Text);
                        sb.I(@class: "htmx-indicator fas fa-circle-notch fa-spin ms-2 !hidden");
                    }
                }
            }
            else if (Url != null)
            {
                using (sb.A(href: Url, @class: btnClass))
                {
                    if (Icon != null)
                    {
                        sb.I(@class: $"fas fa-fw {Icon} mx-4");
                    }
                    else
                    {
                        sb.Append(Text);
                    }
                }
            }
            else
            {
                using (sb.Button(@class: btnClass))
                {
                    if (Icon != null)
                    {
                        sb.I(@class: $"fas fa-fw {Icon} mx-4");
                    }
                    else
                    {
                        sb.Append(Text);
                    }
                }
            }
        }

        public enum ButtonStyle
        {
            Default,
            Secondary,
            Info,
            Success,
            Danger,
            Primary
        }
    }
}