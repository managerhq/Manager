using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerComponents
{
    public sealed class Panel : ComponentBase
    {
        public bool IsActionBar;
        public string Text;
        public string HelpUrl = string.Empty;
        public HeaderButton PrimaryButton;
        public List<ComponentBase> StartElements = new();
        public List<ComponentBase> CenterElements = new();
        public List<ComponentBase> EndElements = new();
        
        // Icon classes
        private const string ActionBarIcon = "fas fa-fw fa-turn-down-right text-neutral-300";
        private const string ActionBarIconStyle = "font-size: 32px";
        private const string HelpIcon = "text-neutral-300 hover:text-neutral-400 fas fa-circle-question";
        
        // Text classes
        private const string LinkStyle = "font-size: 16px";

        public override void BuildString(StringBuilder sb)
        {
            using (sb.Div(@class: "card-header flex justify-between print:hidden"))
            {
                using (sb.Div(@class: "flex items-center gap-6"))
                {
                    using (sb.Div(@class: "flex items-center gap-2"))
                    {
                        if (IsActionBar)
                        {
                            sb.I(@class: ActionBarIcon, style: ActionBarIconStyle);
                        }
                        if (Text != null)
                        {
                            using (sb.Div(@class: "card-title"))
                            {
                                sb.Append(Text);
                            }
                        }
                        if (HelpUrl != string.Empty)
                        {
                            using (sb.A(href: HelpUrl, target: "_blank", style: LinkStyle))
                            {
                                sb.I(@class: HelpIcon);
                            }
                        }
                    }
                    if (PrimaryButton != null)
                    {
                        PrimaryButton.BuildString(sb);
                    }

                    using (sb.Div(@class: "flex items-center gap-4"))
                    {
                        foreach (var e in StartElements)
                        {
                            e.BuildString(sb);
                        }
                    }
                }
                using (sb.Div(@class: "flex items-center gap-6"))
                {
                    foreach (var e in CenterElements)
                    {
                        e.BuildString(sb);
                    }
                }
                using (sb.Div(@class: "flex items-center gap-2"))
                {
                    foreach (var e in EndElements)
                    {
                        using (sb.Div())
                        {
                            e.BuildString(sb);
                        }
                    }
                }
            }
        }
    }
}
