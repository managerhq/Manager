using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerComponents
{
    public sealed class CopyToClipboardButton : ComponentBase
    {
        public required string Text;

        public override void BuildString(StringBuilder sb)
        {
            using (sb.Script())
            {
                sb.Append(@"function writeToClipboardUsingButton(btn, textToCopy) {
    btn.disabled = true;
    writeToClipboard(textToCopy);
    setTimeout(function () { btn.disabled = false }, 3000);
}

function writeToClipboard(textToCopy) {
    // navigator clipboard api needs a secure context (https)
    if (navigator.clipboard && window.isSecureContext) {
        // navigator clipboard api method'
        return navigator.clipboard.writeText(textToCopy);
    } else {
        // text area method
        let textArea = document.createElement(""textarea"");
        textArea.value = textToCopy;
        // make the textarea out of viewport
        textArea.style.position = ""fixed"";
        textArea.style.left = ""-999999px"";
        textArea.style.top = ""-999999px"";
        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();
        return new Promise((res, rej) => {
            // here the magic happens
            document.execCommand('copy') ? res() : rej();
            textArea.remove();
        });
    }
}

function tableToTSV(table) {
    const rows = Array.from(table.querySelectorAll('tr'));
    const grid = [];
    const norm = s =>
        s.replace(/\u00A0/g, ' ')
            .replace(/[\t\r\n]+/g, ' ')
            .replace(/\s+/g, ' ')
            .trim();

    rows.forEach((tr, r) => {
        grid[r] = grid[r] || [];
        let c = 0;
        Array.from(tr.cells).forEach(cell => {
            while (grid[r][c] !== undefined) c++;
            const text = norm(cell.innerText || cell.textContent || '');
            const cs = cell.colSpan || 1;
            const rs = cell.rowSpan || 1;

            for (let rr = 0; rr < rs; rr++) {
                const row = (grid[r + rr] = grid[r + rr] || []);
                for (let cc = 0; cc < cs; cc++) {
                    row[c + cc] = (rr === 0 && cc === 0) ? text : '';
                }
            }
            c += cs;
        });
    });

    // Trim trailing empty cells per row
    grid.forEach((row, i) => {
        let end = row.length;
        while (end > 0 && (row[end - 1] === '' || row[end - 1] == null)) end--;
        grid[i] = row.slice(0, end);
    });

    return grid.map(row => row.map(v => v ?? '').join('\t')).join('\n');
}");
            }

            new FooterButton() { Text = Text, OnClick = "writeToClipboardUsingButton(this, tableToTSV(document.getElementsByTagName('table')[0]))" }.BuildString(sb);
        }
    }
}
