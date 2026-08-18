using Newtonsoft.Json;
using System.Collections;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [Guide("To reorder rows, simply click on a column name.")]
    internal abstract class NakedObjectsWithSorting : NakedObjectsWithViewRedirects
    {
        [InheritedProtoMember(220)] public Guid? SortBy;
        [InheritedProtoMember(221), JsonProperty("sortByDesc")] public bool SortDesc;
        [InheritedProtoMember(223), JsonProperty("sortBy")] public string SortByString;

        protected override void InnerGet4(Context context)
        {
            var columns = context.Get<Column[]>();
            Column sortColumn = null;
            
            if (SortBy.HasValue)
            {
                sortColumn = columns.SingleOrDefault(x => x.Key == SortBy.Value);
            }

            if (!string.IsNullOrWhiteSpace(SortByString))
            {
                sortColumn = columns.SingleOrDefault(x => x.Name == SortByString);
            }

            if (sortColumn != null)
            {
                Sort(context, sortColumn, SortDesc);
            }

            base.InnerGet4(context);
        }

        protected void Sort(Context context, Column sortColumn, bool sortDesc)
        {
            var rows = context.Get<Array>();
            if (!sortColumn.CanEnsureCells(rows)) return;
            sortColumn.EnsureCells(rows);
            var values = rows.OfType<object>().Select(x => new Tuple<object, object>(sortColumn.GetValue(x), x)).ToArray();

            if (sortColumn is Column<string> column && column.Attributes.OfType<PaddedSorting>().Any())
            {
                if (values.Any())
                {
                    var max = values.Select(x => x.Item1 as string ?? string.Empty).Max(x => x.Length);
                    values = values.Select(x => new Tuple<object, object>((x.Item1 as string ?? string.Empty).PadLeft(max), x.Item2)).ToArray();
                }
            }

            var elementType = rows.GetType().GetElementType();

            if (elementType.IsSubclassOf(typeof(ManagerServer.Model.Object)))
            {
                if (!sortDesc) rows = values.OrderBy(x => (x.Item2 as ManagerServer.Model.Object).IsInactive()).ThenBy(x => GetSortingValue(x.Item1)).Select(x => x.Item2).ToArray();
                else rows = values.OrderBy(x => (x.Item2 as ManagerServer.Model.Object).IsInactive()).ThenByDescending(x => GetSortingValue(x.Item1)).Select(x => x.Item2).ToArray();
            }
            else
            {
                if (!sortDesc) rows = values.OrderBy(x => GetSortingValue(x.Item1)).Select(x => x.Item2).ToArray();
                else rows = values.OrderByDescending(x => GetSortingValue(x.Item1)).Select(x => x.Item2).ToArray();
            }

            context.Set<Array>(new ArrayList(rows).ToArray(elementType));
        }

        private object GetSortingValue(object o)
        {
            if (o is string[] stringArray) return string.Join(',', stringArray);
            if (o is NamedObject namedObject) return namedObject.GetCodeAndName();
            return o;
        }

        protected override void OnColumnHeaderCell(Column column)
        {
            if (column.Key.HasValue)
            {
                var httpHandler = (NakedObjectsWithSorting)this.MemberwiseClone();
                httpHandler.SortBy = column.Key.Value;

                if (SortBy == column.Key.Value) httpHandler.SortDesc = !SortDesc;
                else httpHandler.SortDesc = false;

                using (A(href: httpHandler.ToUrl()))
                {
                    base.OnColumnHeaderCell(column);
                }

                if (SortBy == column.Key.Value)
                {
                    //Write("&nbsp;&nbsp;");
                    if (SortDesc) I(@class: "fas fa-caret-down ltr:ml-2 rtl:mr-2");
                    else I(@class: "fas fa-caret-up ltr:ml-2 rtl:mr-2");
                }
            }
            else
            {
                base.OnColumnHeaderCell(column);
            }
        }

        protected sealed class PaddedSorting : Attribute { }
    }
}
