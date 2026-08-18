using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [Key("copy-to-clipboard")]
    [Title(nameof(Strings.Copy_to_clipboard))]
    [Guide("Click the `Copy_to_clipboard` button located in the bottom-right corner to copy the screen's contents into a different spreadsheet program.")]
    [SmallBottomButtonScreenshot(nameof(Strings.Copy_to_clipboard))]
    internal abstract class NakedObjectsWithCopyToClipboard : NakedObjectsWithReportView
    {
        public sealed class DoNotCopyToClipboard : Attribute { }

        protected void WriteCopyToClipboardButton(Context context)
        {
            var rows = context.Get<Array>();
            if (rows == null) return;

            using (Div())
            {
                var columns = context.Get<Column[]>().Where(x => x.Visible && x.CanEnsureCells(rows) && x.CanConvertToPlainText && !x.Attributes.OfType<DoNotCopyToClipboard>().Any()).ToArray();

                var tsv = new StringBuilder();
                foreach (var e in columns)
                {
                    e.EnsureCells(rows);
                    tsv.Append(Escape(e.Label));
                    tsv.Append('\t');
                }
                foreach (var e in rows)
                {
                    tsv.AppendLine();
                    foreach (var e2 in columns)
                    {
                        tsv.Append(Escape(e2.GetValueAsPlainText(e)));
                        tsv.Append('\t');
                    }
                }

                Textarea(id: "export-textarea", style: "display: none", text: tsv.ToString());
                using (Button(id: "export-button", @class: "btn btn-xs", onclick: "javascript:copyToClipboard()")) Write(Strings.Copy_to_clipboard);
                using (Script())
                {
                    Write($@"
function copyToClipboard() {{
    writeToClipboard(document.getElementById('export-textarea').value);
    document.getElementById('export-button').disabled = true;
    document.getElementById('export-button').value = {Strings.Copied.EncodeJsString()};
    setTimeout(function() {{
        document.getElementById('export-button').value = {Strings.Copy_to_clipboard.EncodeJsString()};
        document.getElementById('export-button').disabled = false;
    }}, 3000);
}}");
                }
            }
        }

        protected override void OnFooterEndSection(Context context)
        {
            WriteCopyToClipboardButton(context);

            base.OnFooterEndSection(context);
        }

        private string Escape(string s)
        {
            var text = s;
            if (string.IsNullOrWhiteSpace(text)) return text;
            if (text.Contains('"')) text = text.Replace(@"""", @"""""");
            if (text.Contains('\t')) text = text.Replace('\t', ' ');
            if (text.Contains('\n')) text = $@"""{text}""";
            return text;
        }
    }
}
