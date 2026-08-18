using System.Collections;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithNegativeAmountDetection : NakedObjectsWithSorting
    {
        [InheritedProtoMember(216)] public bool ShowNegatives;

        protected override void InnerGet4(Context context)
        {
            if (this is NakedObjectsWithAdvancedQueries nakedObjectsWithAdvancedSearch && nakedObjectsWithAdvancedSearch.AdvancedSearch.HasValue)
            {
                base.InnerGet4(context);
                return;
            }

            var nonNegativeColumn = context.Get<Column[]>().SingleOrDefault(x => x.Visible && x.Attributes.OfType<WarnIfNegativeAttribute>().Any());
            if (nonNegativeColumn != null)
            {
                var rows = context.Get<Array>();
                nonNegativeColumn.EnsureCells(rows);

                var negatives = rows.Cast<object>().Select(x => new Tuple<decimal, object>(GetDecimal(nonNegativeColumn.GetValue(x)), x)).Where(x => x.Item1 < 0m).ToArray();

                if (negatives.Any())
                {
                    context.Set(new NegativeFilter() { Column = nonNegativeColumn.Label });

                    if (ShowNegatives)
                    {                        
                        rows = new ArrayList(negatives.Select(x => x.Item2).ToArray()).ToArray(rows.GetType().GetElementType());
                        context.Set<Array>(rows);
                        context.Set<Total>(new Total() { Value = rows.Length });
                    }
                }
                else
                {
                    ShowNegatives = false;
                }
            }

            base.InnerGet4(context);
        }

        private sealed class NegativeFilter
        {
            public string Column;
        }

        private decimal GetDecimal(object value)
        {
            if (value is Tuple<decimal, BusinessTemplate> tuple1) return tuple1.Item1;
            if (value is Tuple<decimal, Currency, BusinessTemplate> tuple2) return tuple2.Item1;
            return 0m;
        }

        protected override void OnAfterHeader(Context context)
        {
            var duplicateInfo = context.Get<NegativeFilter>();
            if (duplicateInfo != null)
            {
                var httpHandler = (NakedObjectsWithNegativeAmountDetection)this.MemberwiseClone();
                httpHandler.ShowNegatives = !this.ShowNegatives;

                using (A(href: httpHandler.ToUrl(), @class: "card-header flex gap-4 items-center"))
                {
                    if (this.ShowNegatives)
                    {
                        I(@class: "fas fa-toggle-on text-xl opacity-50");
                    }
                    else
                    {
                        I(@class: "fas fa-toggle-off text-xl opacity-25");
                    }

                    using (Div(@class: "flex gap-2 items-center"))
                    {
                        using (Div(@class: "font-semibold")) Write(Strings.HasWhere);
                        using (Span(@class: "bg-(--input) text-(--input-foreground) border-2 border-(--input-border) px-2 py-1 rounded")) Write(duplicateInfo.Column);
                        using (Span()) Write(Strings.IsLessThan);
                        using (Span(@class: "bg-(--input) text-(--input-foreground) border-2 border-(--input-border) px-2 py-1 rounded")) Write("0");
                    }
                }
            }

            base.OnAfterHeader(context);
        }
    }
}
