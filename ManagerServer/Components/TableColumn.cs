using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerComponents
{
    public sealed class TableColumn : ComponentBase
    {
        public required string Text;
        public required TableCell[] Cells;
        public string Icon = string.Empty;
        public string Url = string.Empty;
        public bool? Desc;
        public bool Checkbox;
        public string Sum;
        public bool Center;
        public bool TabularNums;
        public bool Right;
        public bool MinWidth;
        public bool WhitespaceNoWrap;
        public bool Bold;

        // Icon classes
        private const string IconContainer = "text-center";
        private const string IconClasses = "text-base opacity-25";
        private const string IconStyle = "font-size: 16px";

        // Link classes
        private const string LinkClasses = "text-neutral-500";

        public string GetCellClasses()
        {
            var additions = new List<string>();

            if (Center) additions.Add("text-center");
            else if (Right) additions.Add("text-right");
            else additions.Add("text-start");

            if (MinWidth) additions.Add("w-px");
            if (WhitespaceNoWrap) additions.Add("whitespace-nowrap");            
            if (Bold) additions.Add("font-semibold");
            return string.Join(' ', additions);
        }

        public override void BuildString(StringBuilder sb)
        {
            using (sb.Th(@class: GetCellClasses()))
            {
                if (Icon != string.Empty)
                {
                    using (sb.Div(@class: IconContainer))
                    {
                        sb.I(@class: $"fas {Icon} {IconClasses}", style: IconStyle);
                    }
                }
                else if (Url != string.Empty)
                {
                    using (sb.A(href: Url, @class: LinkClasses)) sb.Append(Text);
                    if (Desc.HasValue)
                    {
                        if (Desc.Value) sb.I(@class: "fas fa-caret-down ms-2");
                        else sb.I(@class: "fas fa-caret-up ms-2");
                    }
                }
                else if (Checkbox)
                {                    
                    using (sb.Script())
                    {
                        sb.Append(@"function toggleColumn(headerCb) {
    const th = headerCb.closest('th,td');
    if (!th) return;
    const table = th.closest('table');
    if (!table) return;

    // figure out logical column index
    const row = th.parentElement;
    let targetIdx = 0, acc = 0;
    for (const c of row.children) {
        if (c === th) { targetIdx = acc; break; }
        acc += c.colSpan || 1;
    }

    // loop rows, find matching cell, toggle checkboxes
    table.querySelectorAll('tr').forEach(r => {
        let col = 0;
        for (const cell of r.children) {
            const span = cell.colSpan || 1;
            if (col <= targetIdx && targetIdx < col + span) {
                if (cell !== th) {
                    cell.querySelectorAll('input[type=checkbox]').forEach(cb => {
                        if (!cb.disabled) {
                            cb.checked = headerCb.checked;
                            cb.dispatchEvent(new Event('change', { bubbles: true }));
                        }
                    });
                }
                break;
            }
            col += span;
        }
    });
}");
                    }
                    sb.InputCheckbox(onclick: "toggleColumn(this)", @class: "form-check-input");
                }
                else
                {
                    sb.Append(Text);
                }
            }
        }
    }
}
