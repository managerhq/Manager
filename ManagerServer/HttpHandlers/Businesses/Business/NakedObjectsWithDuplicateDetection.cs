using System.Collections;
using System.Linq;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithDuplicateDetection : NakedObjectsWithNegativeAmountDetection
    {
        [InheritedProtoMember(215)] public bool ShowDuplicates;

        protected override void InnerGet4(Context context)
        {
            if (this is NakedObjectsWithAdvancedQueries nakedObjectsWithAdvancedSearch && nakedObjectsWithAdvancedSearch.AdvancedSearch.HasValue)
            {
                base.InnerGet4(context);
                return;
            }

            var uniqueColumn = context.Get<Column[]>().SingleOrDefault(x => x.Visible && x is Column<string> && x.Attributes.OfType<WarnIfNotUniqueAttribute>().Any());
            if (uniqueColumn != null)
            {
                var rows = context.Get<Array>();
                uniqueColumn.EnsureCells(rows);

                var duplicates = new ArrayList(rows.Cast<object>()
                    .Select(x => new Tuple<string, object>(uniqueColumn.GetValueAsPlainText(x)?.Trim(), x))
                    .Where(x => !string.IsNullOrWhiteSpace(x.Item1))
                    .GroupBy(x => x.Item1)
                    .Where(x => x.Count() > 1)
                    .SelectMany(x => x)
                    .Select(x => x.Item2)
                    .ToArray())
                    .ToArray(rows.GetType().GetElementType());

                if (duplicates.Length > 0)
                {
                    context.Set(new DuplicateFilter());

                    if (ShowDuplicates)
                    {
                        var excluded = rows.Length - duplicates.Length;
                        //context.Set(new DuplicateFilter() { Excluded = excluded, Value = Strings.ThereAreDuplicatesInThisView });

                        context.Set<Array>(duplicates);
                        context.Set(new Total() { Value = duplicates.Length });
                    }
                }
                else
                {
                    ShowDuplicates = false;
                }
            }

            base.InnerGet4(context);
        }

        private sealed class DuplicateFilter
        {
            public int Excluded;
            public string Value;
        }

        protected override void OnColumnCell(Column column, object row)
        {
            if (ShowDuplicates)
            {
                if (column.Attributes.OfType<WarnIfNotUniqueAttribute>().Any())
                {
                    using (Span(@class: "text-red-500"))
                    {
                        I(@class: "fas fa-circle-exclamation");
                        Write(" ");
                        base.OnColumnCell(column, row);
                    }
                    return;
                }
            }
            base.OnColumnCell(column, row);
        }

        protected override void OnAfterHeader(Context context)
        {
            var duplicateInfo = context.Get<DuplicateFilter>();
            if (duplicateInfo != null)
            {
                if (!ShowDuplicates)
                {
                    using (Div(@class: "card-header"))
                    {
                        using (Div(@class: "flex gap-2 items-center"))
                        {
                            var httpHandler = (NakedObjectsWithDuplicateDetection)this.MemberwiseClone();
                            httpHandler.ShowDuplicates = true;

                            I(@class: "fas fa-fw fa-circle-exclamation text-base opacity-25");
                            using (A(href: httpHandler.ToUrl(), @class: "font-semibold"))
                            {
                                Write(Strings.ThereAreDuplicatesInThisView);
                            }
                        }
                    }
                }
                else
                {
                    /*
                    using (Div(@class: "bg-yellow-50 p-4 border border-t-0"))
                    {
                        Write(string.Format(Strings.HiddenRowsCount, "<b>" + duplicateInfo.Excluded + @"</b>", "<b>" + duplicateInfo.Value + @"</b>"));
                        Write("&nbsp;&nbsp;");
                        var httpHandler = (NakedObjectsWithDuplicateDetection)this.MemberwiseClone();
                        httpHandler.ShowDuplicates = false;
                        using (A(href: httpHandler.ToUrl(), @class: "font-bold")) Write(Strings.Undo);
                    }
                    */
                }
            }

            base.OnAfterHeader(context);
        }
    }
}
