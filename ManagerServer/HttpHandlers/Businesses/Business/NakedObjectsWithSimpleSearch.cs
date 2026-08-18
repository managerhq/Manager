using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using Newtonsoft.Json;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [Guide("To quickly find what you need, enter the term into the search box to swiftly narrow down the list of rows.")]
    internal abstract class NakedObjectsWithSimpleSearch : NakedObjectsWithDuplicateDetection
    {
        [InheritedProtoMember(230), JsonProperty("term")] public string Term;

        protected override void InnerGet4(Context context)
        {
            if (!string.IsNullOrWhiteSpace(Term))
            {
                var rows = context.Get<Array>();
                var columns = context.Get<Column[]>();

                var visibleColumns = columns.Where(x => x.Visible && x.CanEnsureCells(rows)).ToArray();
                foreach (var e in visibleColumns)
                {
                    e.EnsureCells(rows);
                }

                var keywords = Term.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                var rowsAfterSearch = new ArrayList();

                foreach (var e in rows)
                {
                    var text = string.Join(' ', visibleColumns.Select(x => x.GetValueAsPlainText(e)));
                    if (keywords.All(x => text.IndexOf(x, StringComparison.OrdinalIgnoreCase) != -1))
                    {
                        rowsAfterSearch.Add(e);
                    }
                }

                var excluded = rows.Length - rowsAfterSearch.Count;
                if (excluded > 0) Features.Set(new Excluded() { Value = excluded });
                context.Set(new Total() { Value = rowsAfterSearch.Count });
                rows = rowsAfterSearch.ToArray(rows.GetType().GetElementType());
                context.Set(rows);
            }

            base.InnerGet4(context);
        }

        protected override void OnHeaderEndSection(Context context)
        {            
            using (Form(action: this.ToUrl(), method: "POST", @class: "flex items-center gap-2"))
            {
                var advancedSearchHandler = (NakedObjectsWithSimpleSearch)this.MemberwiseClone();
                InputText(name: "Term", @class: "form-control min-w-[12ch]", placeholder: Strings.Search, autocapitalize: false, autocomplete: "off", autocorrect: false, spellcheck: false, value: Term);
                using (Button(@class: "btn"))
                {
                    Write(Strings.Search);
                }
            }

            base.OnHeaderEndSection(context);
        }

        protected override void OnAfterHeader(Context context)
        {
            var excluded = Features.Get<Excluded>();
            if (excluded != null)
            {
                using (Div(@class: "card-header"))
                {
                    Write(string.Format(Strings.HiddenRowsCount, "<b>" + excluded.Value + @"</b>", "<b>" + Term + @"</b>"));
                    Write("&nbsp;&nbsp;");
                    var httpHandler = (NakedObjectsWithSimpleSearch)this.MemberwiseClone();
                    httpHandler.Skip = 0;
                    httpHandler.Term = null;
                    using (A(href: httpHandler.ToUrl(), @class: "font-semibold")) Write(Strings.Undo);
                }
            }
            base.OnAfterHeader(context);
        }

        protected override void OnAfterFooter(Context context)
        {
            if (!string.IsNullOrWhiteSpace(Term))
            {
                Script(src: "resources/mark/mark-min.js");
                using (Script())
                {
                    if (!string.IsNullOrWhiteSpace(Term))
                    {
                        Write(@$"var instance = new Mark(""td"");");
                        Write($"instance.mark({Term.EncodeJsString()});");
                    }
                }
            }
            base.OnAfterFooter(context);
        }

        protected override async Task InnerPost()
        {
            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                if (form.ContainsKey(nameof(Term)))
                {
                    Term = form[nameof(Term)];
                    Skip = 0;
                    Response.Redirect(this.ToUrl());
                    return;
                }
            }

            await base.InnerPost();
        }

        public sealed class Excluded
        {
            public int Value;
        }
    }
}
