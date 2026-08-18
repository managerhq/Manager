using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerComponents
{
    public sealed class TableCell : ComponentBase
    {
        public TableCellButton CellButton;
        public string TextValue;
        public Tuple<string, byte[]> Checkbox;
        public string Icon;
        public bool Inactive;

        // Checkbox styling for dark mode support

        public string GetCellClasses()
        {
            if (Inactive) return "*:opacity-35";
            return string.Empty;
        }

        public override void BuildString(StringBuilder sb)
        {
            if (TextValue != null)
            {
                using (sb.Span())
                {
                    sb.Append(TextValue);
                }
            }
            else if (CellButton != null)
            {
                CellButton.BuildString(sb);
            }
            else if (Icon != null)
            {
                sb.I(@class: $"fas {Icon} opacity-50 text-base");
            }
            else if (Checkbox != null)
            {
                if (Checkbox.Item2 == null || Checkbox.Item2.Length == 0)
                {
                    sb.InputCheckbox(disabled: true, @class: "form-check-input");
                }
                else
                {
                    sb.InputCheckbox(form: Checkbox.Item1, name: Checkbox.Item1, value: Convert.ToBase64String(Checkbox.Item2), @class: "form-check-input");
                }
            }
        }
    }
}