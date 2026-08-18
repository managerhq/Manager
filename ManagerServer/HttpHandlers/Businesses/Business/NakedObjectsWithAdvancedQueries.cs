using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using Newtonsoft.Json;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [Key("advanced-queries")]
    [Title(nameof(Strings.AdvancedQueries))]
    [Guide("`AdvancedQueries` is a powerful feature in Manager.io that allows you to select, sort, filter, and organize your data on any tabular screen, offering nearly unlimited reporting possibilities.")]
    [Guide("This feature is particularly useful when combined with `CustomFields`, allowing for customized data management tailored to your business needs.")]
    [Header("Accessing Advanced Queries")]
    [Guide("To access `AdvancedQueries`, navigate to the tab with the data you need, such as the `SalesInvoices` tab.")]
    [Guide("Click on `AdvancedQueries`, located in the top-right corner next to the search box.")]
    [Guide("Then click `NewAdvancedQuery` to start creating an advanced query.")]    
    [Header("Understanding Query Fields")]
    [Guide("When creating an `AdvancedQuery` you will see the following fields:")]
    [Guide("`Name` field allows you to specify a descriptive name for your advanced query. This makes it easy to identify and reuse the query later. Choose a name that clearly describes what the query does, such as 'High Value Invoices' or 'Monthly Sales by Customer'.")]
    [Guide("`Select` field determines which columns will appear in your results. Choose only the columns you need to see. You can reorder columns by dragging them up or down using the arrow handles. The order you set here determines how columns appear from left to right in your results.")]
    [Guide("`HasWhere` field enables filtering to show only records that meet specific criteria. When checked, you can add one or more filter conditions. For example, filter invoices by amount, date range, customer, or status. Multiple conditions work together (AND logic) to narrow down results.")]
    [Guide("`HasOrderBy` field controls how your results are sorted. When enabled, you can specify one or more columns to sort by, choosing ascending or descending order for each. Results are sorted by the first column, then by the second column for matching values, and so on.")]
    [Guide("`HasGroupBy` field groups similar records together and can calculate totals for each group. This is useful for summary views like 'Sales by Customer' or 'Expenses by Category'. When grouping is enabled, numeric columns automatically show totals for each group.")]
    [Header("Example: Finding High-Value Invoices")]
    [Guide("As an example, imagine you want to see all sales invoices above $1,000. Here's how you could set this up:")]
    [Guide("Go to the `SalesInvoices` tab.")]
    [TabScreenshot("fa-file-invoice", nameof(Strings.SalesInvoices))]
    [Guide("Click `AdvancedQueries` dropdown, then click on `NewAdvancedQuery`.")]
    [AdvancedQueriesDropdownScreenshot]
    [Guide("In `Select` field, choose the columns you would like to show for each invoice. For example you can choose:")]
    [Guide("- `IssueDate`")]
    [Guide("- `Customer`")]
    [Guide("- `InvoiceAmount`")]
    [Guide("- `Status`")]
    [Guide("Check `HasWhere` checkbox and select `InvoiceAmount` field, then select `IsMoreThan` and enter amount 1000.")]
    [Guide("Click `Create` button to create your `AdvancedQuery`.")]
    [PrimaryButtonScreenshot(name: nameof(Strings.Create))]
    [Guide("This will take you back to `SalesInvoices` tab with your advanced query being created and selected which means you should see only sales invoices where `InvoiceAmount` is above 1000.")]
    [AdvancedQuery(select: [ nameof(Strings.IssueDate), nameof(Strings.Customer), nameof(Strings.InvoiceAmount), nameof(Strings.Status)], where: [nameof(Strings.InvoiceAmount), nameof(Strings.IsMoreThan), "1000"])]
    [Guide("You can click `Edit` button to further refine your `AdvancedQuery`.")]
    [Header("Advanced Usage")]
    [Guide("Similarly, you can follow the procedure of creating `AdvancedQuery` under any tab giving you the ultimate flexibility of querying your data.")]
    [Guide("Combine `AdvancedQueries` with `CustomFields` to track and report on specialized data unique to your business, such as customer satisfaction scores or specific service types.")]
    [LinkGuide("For more information see:", typeof(Settings.CustomFields.CustomFields))]
    internal abstract class NakedObjectsWithAdvancedQueries : NakedObjectsWithSimpleSearch
    {
        [InheritedProtoMember(260)] public Guid? AdvancedSearch;
        [InheritedProtoMember(261)] public bool EditAdvancedSearch;
        [InheritedProtoMember(263)] public bool ExpandGroupBy;
        [InheritedProtoMember(264)] public string ExpandGroupByKey;
        [InheritedProtoMember(265)] public bool DetailsOpen;
        [InheritedProtoMember(266)] public Guid? CopyToNewAdvancedSearch;

        protected override void InnerGet4(Context context)
        {
            if (EditAdvancedSearch)
            {
                CustomInnerGetForEdit(context);
                return;
            }

            var advancedQuery = ApplicationData.Businesses.Get(Business).SingleOrDefault<AdvancedQuery>(AdvancedSearch);
            if (advancedQuery != null)
            {
                context.Set<NakedObjectsWithReportView.ReportInfo>(new ReportInfo() { Title = advancedQuery.Name });

                var keys = advancedQuery.Select.Select(x => x.Key).Where(x => x.HasValue).Distinct().ToArray();

                var columns = context.Get<Column[]>();
                foreach (var e in columns)
                {
                    if (!e.Key.HasValue) continue;
                    if (e.Key == new Guid("e86f12dd-2bfc-4eef-a7b0-71e4e9caeda9")) continue;
                    e.Visible = false;
                }

                for (int i = 0; i < keys.Length; i++)
                {
                    var column = columns.SingleOrDefault(x => x.Key == keys[i]);
                    if (column == null) continue;
                    column.Priority = i;
                    column.Visible = true;
                }

                context.Set(columns.OrderBy(x => x.Priority).ToArray());

                if (advancedQuery.HasWhere)
                {
                    var rows = context.Get<Array>();

                    foreach (var e in advancedQuery.Where)
                    {
                        if (!e.Key.HasValue) continue;
                        var column = columns.SingleOrDefault(x => x.Key == e.Key);
                        if (column == null) continue;
                        column.EnsureCells(rows);

                        var rowsAfterSearch = new ArrayList();
                        foreach (var e2 in rows)
                        {
                            var value = column.GetValue(e2);                            
                            if (value is Tuple<decimal, ManagerServer.Model.Currency> item) value = item.Item1;
                            if (value is Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate> item2) value = item2.Item1;
                            if (value is Tuple<int, BusinessTemplate> item3) value = item3.Item1;
                            if (value is Tuple<decimal, BusinessTemplate> item4) value = item4.Item1;
                            if (value is NamedObject namedObject) value = namedObject.GetCodeAndName();

                            if (e.IsMatch(value))
                            {
                                rowsAfterSearch.Add(e2);
                            }
                        }
                        rows = rowsAfterSearch.ToArray(rows.GetType().GetElementType());

                    }

                    context.Set<Array>(rows);
                    context.Set<Total>(new Total() { Value = rows.Length });
                }

                if (advancedQuery.HasOrderBy)
                {
                    foreach (var e in advancedQuery.OrderBy)
                    {
                        if (!e.Key.HasValue) continue;
                        var sortColumn = columns.SingleOrDefault(x => x.Key == e.Key.Value);
                        if (sortColumn == null) continue;
                        Sort(context, sortColumn, e.SortOrder == SortOrder.Descending);
                    }
                }

                if (advancedQuery.HasGroupBy && advancedQuery.GroupBy != null)
                {
                    var groupByColumnKeys = advancedQuery.GroupBy.Where(x => x.Key.HasValue).Select(x => x.Key.Value).Distinct().ToArray();
                    var groupByColumns = columns.Where(x => x.Key.HasValue && groupByColumnKeys.Contains(x.Key.Value)).ToArray();
                    if (groupByColumns.Length > 0)
                    {
                        var rows = context.Get<Array>();

                        foreach (var e in groupByColumns) e.EnsureCells(rows);

                        if (ExpandGroupBy)
                        {
                            var row2 = new ArrayList(rows.Cast<object>().Where(x => ConvertArrayToKey(groupByColumns.Select(y => y.GetValueAsPlainText(x) ?? string.Empty).ToArray()) == ExpandGroupByKey).ToArray()).ToArray(rows.GetType().GetElementType());
                            context.Set<Array>(row2);
                            context.Set<Total>(new Total() { Value = row2.Length });
                        }
                        else
                        {
                            var newRows = new List<Tuple<string, Array>>();
                            
                            foreach (var e in rows.Cast<object>().GroupBy(x => ConvertArrayToKey(groupByColumns.Select(y => y.GetValueAsPlainText(x) ?? string.Empty).ToArray())))
                            {
                                var innerRows = new ArrayList(e.ToArray()).ToArray(rows.GetType().GetElementType());
                                newRows.Add(new Tuple<string, Array>(e.Key, innerRows));
                            }

                            context.Set<Array>(newRows.ToArray());
                            context.Set<Total>(new Total() { Value = newRows.Count });

                            var newColumns = new List<Column>();                            

                            for (int i = 0; i < groupByColumns.Length; i++)
                            {
                                newColumns.Add(new KeyColumn()
                                {
                                    Key = groupByColumns[i].Key,
                                    Label = groupByColumns[i].Label,
                                    Visible = true,
                                    Attributes = new Attribute[] { new KeyColumnIndexAttribute(i) }
                                });
                            }                            

                            foreach (var e in columns.Where(x => x.Visible && x.Attributes.OfType<SumAttribute>().Any()))
                            {
                                if (groupByColumns.Contains(e)) continue;
                                e.EnsureCells(rows);
                                newColumns.Add(new SumColumn(e, this)
                                {
                                    Key = e.Key,
                                    Visible = true,
                                    Label = e.Label,
                                    Attributes = e.Attributes
                                });
                            }

                            if (!newColumns.OfType<SumColumn>().Any())
                            {
                                newColumns.Add(new CountColumn(null, this)
                                {
                                    Key = new Guid("f221266b-ae24-4335-accf-232e22720712"),
                                    Visible = true,
                                    Label = Strings.Count,
                                    Attributes = [
                                        new CenterAttribute(),
                                        new BoldAttribute(),
                                        new SumAttribute()
                                ]
                                });
                            }

                            context.Set<Tuple<Column[]>>(new Tuple<Column[]>(columns));
                            context.Set<Column[]>(newColumns.ToArray());
                        }
                    }
                }
            }

            base.InnerGet4(context);
        }

        private string ConvertArrayToKey(string[] values)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, values);
                return Convert.ToBase64String(ms.ToArray());
            }
        }        

        protected sealed class KeyColumn : Column<string>
        {
            public override void EnsureCells(Array rows)
            {
                var index = Attributes.OfType<KeyColumnIndexAttribute>().Single().Index;
                var rows2 = (Tuple<string, Array>[])rows;
                var values = rows2.Select(x => GetInnerKey(x.Item1, index)).ToArray();
                AddValues(rows, values);
            }

            private string GetInnerKey(string key, int index)
            {
                using (var ms = new System.IO.MemoryStream(Convert.FromBase64String(key)))
                {
                    return ProtoBuf.Serializer.Deserialize<string[]>(ms)[index];
                }
            }
        }

        public sealed class KeyColumnIndexAttribute : Attribute
        {
            public int Index { get; init; }

            public KeyColumnIndexAttribute(int index)
            {
                Index = index;
            }
        }

        public sealed class SumColumn : Column<Tuple<decimal, BusinessTemplate>>
        {
            private Column innerColumn;
            private NakedObjectsWithAdvancedQueries nakedObjectsWithAdvancedSearch;

            public SumColumn(Column innerColumn, NakedObjectsWithAdvancedQueries nakedObjectsWithAdvancedSearch)
            {
                this.innerColumn = innerColumn;
                this.nakedObjectsWithAdvancedSearch = nakedObjectsWithAdvancedSearch;
            }

            public override void EnsureCells(Array rows)
            {
                var rows2 = (Tuple<string, Array>[])rows;
                var values = new Tuple<decimal, BusinessTemplate>[rows2.Length];
                for (int i = 0; i < rows2.Length; i++)
                {
                    var innerRows = rows2[i].Item2;
                    innerColumn.EnsureCells(innerRows);
                    var total = 0m;
                    for (int i2 = 0; i2 < innerRows.Length; i2++)
                    {
                        var innerValue = innerColumn.GetValue(innerRows.GetValue(i2));
                        if (innerValue is decimal value1) total += value1;
                        else if (innerValue is Tuple<decimal, ManagerServer.Model.Currency> value2) total += value2.Item1;
                        else if (innerValue is Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate> value3) total += value3.Item1;
                        else if (innerValue is Tuple<decimal, BusinessTemplate> value4) total += value4.Item1;
                    }

                    var businessTemplate = (NakedObjectsWithAdvancedQueries)nakedObjectsWithAdvancedSearch.MemberwiseClone();
                    businessTemplate.ExpandGroupBy = true;
                    businessTemplate.ExpandGroupByKey = rows2[i].Item1;
                    businessTemplate.Referrer = nakedObjectsWithAdvancedSearch.ToUrl();
                    businessTemplate.Skip = 0;
                    businessTemplate.PageSize = null;

                    values[i] = new Tuple<decimal, BusinessTemplate>(total, businessTemplate);
                }
                AddValues(rows, values);
            }
        }

        public sealed class CountColumn : Column<Tuple<int, BusinessTemplate>>
        {
            private Column innerColumn;
            private NakedObjectsWithAdvancedQueries nakedObjectsWithAdvancedSearch;

            public CountColumn(Column innerColumn, NakedObjectsWithAdvancedQueries nakedObjectsWithAdvancedSearch)
            {
                this.innerColumn = innerColumn;
                this.nakedObjectsWithAdvancedSearch = nakedObjectsWithAdvancedSearch;
            }

            public override void EnsureCells(Array rows)
            {
                var rows2 = (Tuple<string, Array>[])rows;
                var values = new Tuple<int, BusinessTemplate>[rows2.Length];
                for (int i = 0; i < rows2.Length; i++)
                {
                    var innerRows = rows2[i].Item2;
                    var count = innerRows.Length;                    

                    var businessTemplate = (NakedObjectsWithAdvancedQueries)nakedObjectsWithAdvancedSearch.MemberwiseClone();
                    businessTemplate.ExpandGroupBy = true;
                    businessTemplate.ExpandGroupByKey = rows2[i].Item1;
                    businessTemplate.Referrer = nakedObjectsWithAdvancedSearch.ToUrl();
                    businessTemplate.Skip = 0;
                    businessTemplate.PageSize = null;

                    values[i] = new Tuple<int, BusinessTemplate>(count, businessTemplate);
                }
                AddValues(rows, values);
            }
        }

        protected override void OnAfterHeader(Context context)
        {
            OnAfterHeaderShowCustomTableEditView(context);

            base.OnAfterHeader(context);
        }

        private void OnAfterHeaderShowCustomTableEditView(Context context)
        {
            var customTable = ApplicationData.Businesses.Get(Business).SingleOrDefault<AdvancedQuery>(AdvancedSearch);
            if (customTable != null)
            {
                var columns = context.Get<Tuple<Column[]>>()?.Item1 ?? context.Get<Column[]>();

                using (Div(@class: "card-inset"))
                {
                    using (Details(@class: "card", open: DetailsOpen))
                    {
                        using (Summary(@class: "card-header cursor-pointer print:hidden font-semibold", style: "display: list-item"))
                        {
                            if (!string.IsNullOrWhiteSpace(customTable.Name))
                            {
                                Write(customTable.Name);
                            }
                            else
                            {
                                Write(Strings.Unnamed);
                            }
                        }
                        using (Div(@class: "card-form print:hidden"))
                        {
                            using (Div(@class: "font-semibold")) Write(Strings.Select);
                            using (Div(@class: "mt-4 ps-6 flex gap-2"))
                            {
                                foreach (var e in customTable.Select.Where(x => x.Key.HasValue).Select(x => columns.SingleOrDefault(y => y.Key == x.Key)).Where(x => x != null).Select(x => x.Label))
                                {
                                    using (Span(@class: "bg-(--selection) text-(--selection-foreground) py-2 px-4 rounded")) Write(e);
                                }
                            }
                            if (customTable.HasWhere)
                            {
                                using (Div(@class: "font-semibold mt-4")) Write(Strings.HasWhere);
                                foreach (var e in customTable.Where)
                                {
                                    if (!e.Key.HasValue) continue;
                                    var column = columns.SingleOrDefault(x => x.Key == e.Key.Value);
                                    if (column == null) continue;
                                    using (Div(@class: "mt-4 ps-6 flex gap-2 items-center"))
                                    {
                                        using (Span(@class: "bg-(--selection) text-(--selection-foreground) py-2 px-4 rounded")) Write(column.Label);
                                        if (IsStringColumn(column))
                                        {
                                            using (Span()) Write(ManagerServer.Globalization.Strings.GetPropertyValue(e.StringFilter.ToString()));
                                            if (e.StringFilter == AdvancedQuery.StringOperator.Contains || e.StringFilter == AdvancedQuery.StringOperator.DoesNotContain)
                                            {
                                                using (Span(@class: "bg-(--input) text-(--input-foreground) border-(--input-border) border-2 py-2 px-4 rounded")) Write(e.Text);
                                            }
                                        }
                                        if (IsDateColumn(column))
                                        {
                                            using (Span()) Write(ManagerServer.Globalization.Strings.GetPropertyValue(e.DateFilter.ToString()));
                                            using (Span(@class: "bg-(--input) text-(--input-foreground) border-(--input-border) border-2 py-2 px-4 rounded")) Write(e.Date.ToLocalShortDisplayString());
                                        }
                                        if (IsNumberColumn(column))
                                        {
                                            using (Span()) Write(ManagerServer.Globalization.Strings.GetPropertyValue(e.NumberFilter.ToString()));
                                            if (e.NumberFilter == AdvancedQuery.NumberOperator.IsLessThan || e.NumberFilter == AdvancedQuery.NumberOperator.IsMoreThan)
                                            {
                                                using (Span(@class: "bg-(--input) text-(--input-foreground) border-(--input-border) border-2 py-2 px-4 rounded")) Write(e.Number.ToString());
                                            }
                                        }
                                        if (IsDecimalColumn(column))
                                        {
                                            using (Span()) Write(ManagerServer.Globalization.Strings.GetPropertyValue(e.DecimalFilter.ToString()));
                                            if (e.DecimalFilter == AdvancedQuery.DecimalOperator.IsLessThan || e.DecimalFilter == AdvancedQuery.DecimalOperator.IsMoreThan)
                                            {
                                                using (Span(@class: "bg-(--input) text-(--input-foreground) border-(--input-border) border-2 py-2 px-4 rounded")) Write(e.Decimal.ToNumberString());
                                            }
                                        }
                                        if (IsBooleanColumn(column))
                                        {
                                            using (Span()) Write(ManagerServer.Globalization.Strings.GetPropertyValue(e.BooleanFilter.ToString()));
                                        }
                                        if (IsEnumColumn(column))
                                        {
                                            if (e.EnumValue.HasValue)
                                            {
                                                using (Span()) Write(ManagerServer.Globalization.Strings.GetPropertyValue(e.EnumFilter.ToString()));
                                                using (Span(@class: "bg-(--input) text-(--input-foreground) border-(--input-border) border-2 py-2 px-4 rounded"))
                                                {
                                                    var enumType = column.GetType().GetGenericArguments()[0];
                                                    var enumValue = Enum.GetName(enumType, e.EnumValue.Value);
                                                    Write(ManagerServer.Globalization.Strings.GetPropertyValue(enumValue));
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (customTable.HasGroupBy && customTable.GroupBy != null)
                            {
                                using (Div(@class: "font-semibold mt-4")) Write(Strings.HasGroupBy);
                                using (Div(@class: "mt-4 ps-6 flex gap-2"))
                                {
                                    foreach (var e in customTable.GroupBy.Where(x => x.Key.HasValue).Select(x => columns.SingleOrDefault(y => y.Key == x.Key)).Where(x => x != null).Select(x => x.Label))
                                    {
                                        using (Span(@class: "bg-(--selection) text-(--selection-foreground) py-2 px-4 rounded")) Write(e);
                                    }
                                }
                            }

                            if (customTable.HasOrderBy && customTable.OrderBy != null)
                            {
                                using (Div(@class: "font-semibold mt-4")) Write(Strings.HasOrderBy);
                                using (Div(@class: "mt-4 ps-6 flex gap-2 items-center"))
                                {
                                    foreach (var e in customTable.OrderBy.Where(x => x.Key.HasValue))
                                    {
                                        var sortColumn = columns.SingleOrDefault(y => y.Key == e.Key);
                                        if (sortColumn == null) continue;
                                        using (Span(@class: "bg-(--selection) text-(--selection-foreground) py-2 px-4 rounded")) Write(sortColumn.Label);
                                        using (Span()) Write(ManagerServer.Globalization.Strings.GetPropertyValue(e.SortOrder.ToString()));
                                    }
                                }
                            }
                        }

                        using (Div(@class: "card-header print:hidden"))
                        {
                            using (Div(@class: "flex items-center gap-4"))
                            {
                                I(@class: $"fas fa-fw fa-turn-up fa-rotate-90 text-xl opacity-25");

                                var httpHandler = (NakedObjectsWithAdvancedQueries)this.MemberwiseClone();
                                httpHandler.EditAdvancedSearch = true;
                                httpHandler.Referrer = this.ToUrl();

                                using (A(@class: "btn", href: httpHandler.ToUrl())) Write(Strings.Edit);

                                var httpHandler2 = (NakedObjectsWithAdvancedQueries)this.MemberwiseClone();
                                httpHandler2.EditAdvancedSearch = true;
                                httpHandler2.AdvancedSearch = null;
                                httpHandler2.CopyToNewAdvancedSearch = AdvancedSearch.Value;

                                using (A(href: httpHandler2.ToUrl(), @class: "btn btn-outline"))
                                {
                                    Write(Strings.Clone);
                                }

                                var httpHandler3 = (NakedObjectsWithAdvancedQueries)this.MemberwiseClone();
                                httpHandler3.AdvancedSearch = null;
                                httpHandler3.ExpandGroupBy = false;
                                httpHandler3.ExpandGroupByKey = null;

                                using (A(href: httpHandler3.ToUrl(), @class: "text-(--muted-foreground)/25 hover:text-(--muted-foreground)/25"))
                                {
                                    I(@class: "fas fa-close text-base");
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void OnHeaderEndSection(Context context)
        {
            var guidAttribute = this.GetType().GetCustomAttribute<GuidAttribute>();
            if (guidAttribute != null)
            {
                using (Details(@class: "dropdown"))
                {
                    using (Summary(@class: "cursor-pointer", style: "color: #999; font-size: 12px; display: list-item"))
                    {
                        Write(Strings.AdvancedQueries);
                    }
                    using (Div(@class: "dropdown-menu"))
                    {
                        var customTables = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.AdvancedQuery>().Where(x => x.NakedTable == guidAttribute.Value).OrderBy(x => x.Name).ToArray();
                        foreach (var e in customTables)
                        {
                            var customTableHandler = (NakedObjectsWithAdvancedQueries)this.MemberwiseClone();
                            customTableHandler.AdvancedSearch = e.Key;
                            customTableHandler.ExpandGroupBy = false;
                            customTableHandler.Referrer = Referrer;
                            customTableHandler.Skip = 0;
                            customTableHandler.PageSize = null;
                            customTableHandler.SortBy = null;
                            customTableHandler.Term = null;
                            customTableHandler.DetailsOpen = false;
                            using (A(href: customTableHandler.ToUrl(), @class: "dropdown-item"))
                            {
                                var name = e.Name;
                                if (string.IsNullOrWhiteSpace(e.Name))
                                {
                                    //var advancedQueryDisplay = new AdvancedQueryDisplay(context, e);
                                    //name = advancedQueryDisplay.GetAutomaticName();
                                    name = Strings.Unnamed;
                                }
                                Write(name);
                            }
                        }
                        if (customTables.Any()) Hr(@class: "my-2");

                        var newCustomTableHandler = (NakedObjectsWithAdvancedQueries)this.MemberwiseClone();
                        newCustomTableHandler.EditAdvancedSearch = true;
                        newCustomTableHandler.ExpandGroupBy = false;
                        newCustomTableHandler.AdvancedSearch = null;
                        newCustomTableHandler.Referrer = this.ToUrl();
                        using (A(href: newCustomTableHandler.ToUrl(), @class: "dropdown-item")) Write(Strings.NewAdvancedQuery);
                    }
                }
            }

            base.OnHeaderEndSection(context);
        }        

        private void CustomInnerGetForEdit(Context context)
        {
            var columns = context.Get<Column[]>().Where(x => x.Key.HasValue).ToArray();

            using (Div(@class: "card", id: "v-model-form"))
            {
                using (Div(@class: "card-header"))
                {
                    using (Div(@class: "flex gap-3 items-center"))
                    {
                        using (Div(@class: "card-title")) Write(Strings.AdvancedQuery);
                        WriteHelp("advanced-queries", false);
                    }
                }

                using (Div(@class: "card-form"))
                {
                    using (Div(@class: "form-group"))
                    {
                        using (Label()) Write(Strings.Name);
                        using (Div(@class: "input-group"))
                        {
                            InputText(v_model: nameof(ManagerServer.Model.AdvancedQuery.Name), @class: "form-control", style: "width: 400px", placeholder: Strings.Unnamed);
                        }
                    }

                    using (Div(@class: "form-group"))
                    {
                        using (Label()) Write(Strings.Select);
                        using (Table())
                        {
                            using (TBody(v_model: "Select", @is: "draggable", tag: "tbody", handle: ".handle"))
                            {
                                using (Tr(v_for: "(lineItem, index) in Select"))
                                {
                                    using (Td(@class: "handle cursor-move"))
                                    {
                                        using (Div(style: "display: table; border-collapse: separate; width: 100%"))
                                        {
                                            using (Span(@class: "form-control text-center whitespace-nowrap"))
                                            {
                                                I(@class: "fas fa-arrows-v");
                                            }
                                        }
                                    }
                                    using (Td())
                                    {
                                        using (Select(v_model: "lineItem.Key", @class: "form-select"))
                                        {
                                            Option();
                                            foreach (var e in columns)
                                            {
                                                if (e.Key == new Guid("e86f12dd-2bfc-4eef-a7b0-71e4e9caeda9")) continue; // Attachment column
                                                Option(value: e.Key.Value.ToString(), text: e.Label);
                                            }
                                        }
                                    }
                                    using (Td(style: "vertical-align: top"))
                                    {
                                        using (Div(v_if: "Select.length > 1"))
                                        {
                                            using (Button(type: "button", @class: "btn", style: "height: 30px; font-size: 24px; font-weight: bold; color: #ccc; padding: 0px 3px", v_on_click: "Select.splice(index, 1)")) Write("&times;");
                                        }
                                    }
                                }
                            }
                        }
                        using (Button(type: "button", v_on_click: "Select.push({})", @class: "btn btn-sm mt-2")) Write(Strings.AddLine);
                    }

                    using (Div(@class: "flex items-start gap-2 my-1"))
                    {
                        InputCheckbox(id: "HasWhere", @class: "form-check-input", value: "true", v_model: "HasWhere");
                        using (Div(@class: "w-full"))
                        {
                            using (Label(@for: "HasWhere"))
                            {
                                Write(Strings.HasWhere);
                            }

                            using (Div(@class: "form-group", v_if: "HasWhere"))
                            {
                                using (Table())
                                {
                                    using (TBody(v_model: "Where", @is: "draggable", tag: "tbody", handle: ".handle"))
                                    {
                                        using (Tr(v_for: "(lineItem, index) in Where"))
                                        {
                                            using (Td(@class: "handle cursor-move"))
                                            {
                                                using (Div(style: "display: table; border-collapse: separate; width: 100%"))
                                                {
                                                    using (Span(@class: "form-control text-center whitespace-nowrap"))
                                                    {
                                                        I(@class: "fas fa-arrows-v");
                                                    }
                                                }
                                            }
                                            using (Td())
                                            {
                                                using (Select(v_model: "lineItem.Key", @class: "form-select"))
                                                {
                                                    Option();
                                                    foreach (var e in columns)
                                                    {
                                                        Option(value: e.Key.Value.ToString(), text: e.Label);
                                                    }
                                                }
                                            }
                                            using (Td())
                                            {
                                                using (Div(v_if: "isStringColumn(lineItem.Key)"))
                                                {
                                                    VSelect(typeof(ManagerServer.Model.AdvancedQuery.StringOperator), nameof(ManagerServer.Model.AdvancedQuery.WhereLine.StringFilter));
                                                }
                                                using (Div(v_if: "isDateColumn(lineItem.Key)"))
                                                {
                                                    VSelect(typeof(ManagerServer.Model.AdvancedQuery.DateOperator), nameof(ManagerServer.Model.AdvancedQuery.WhereLine.DateFilter));
                                                }
                                                using (Div(v_if: "isNumberColumn(lineItem.Key)"))
                                                {
                                                    VSelect(typeof(ManagerServer.Model.AdvancedQuery.NumberOperator), nameof(ManagerServer.Model.AdvancedQuery.WhereLine.NumberFilter));
                                                }
                                                using (Div(v_if: "isDecimalColumn(lineItem.Key)"))
                                                {
                                                    VSelect(typeof(ManagerServer.Model.AdvancedQuery.DecimalOperator), nameof(ManagerServer.Model.AdvancedQuery.WhereLine.DecimalFilter));
                                                }
                                                using (Div(v_if: "isBooleanColumn(lineItem.Key)"))
                                                {
                                                    VSelect(typeof(ManagerServer.Model.AdvancedQuery.BooleanOperator), nameof(ManagerServer.Model.AdvancedQuery.WhereLine.BooleanFilter));
                                                }
                                                using (Div(v_if: "isEnumColumn(lineItem.Key)"))
                                                {
                                                    VSelect(typeof(ManagerServer.Model.AdvancedQuery.EnumOperator), nameof(ManagerServer.Model.AdvancedQuery.WhereLine.EnumFilter));
                                                }
                                            }
                                            using (Td())
                                            {
                                                using (Div(v_if: "isStringColumn(lineItem.Key)"))
                                                {
                                                    InputText(v_model: "lineItem." + nameof(ManagerServer.Model.AdvancedQuery.WhereLine.Text), @class: "form-control", v_if: "lineItem.StringFilter == 0 || lineItem.StringFilter == 1");
                                                }
                                                using (Div(v_if: "isDateColumn(lineItem.Key)"))
                                                {
                                                    VInputDate("lineItem." + nameof(ManagerServer.Model.AdvancedQuery.WhereLine.Date));
                                                }
                                                using (Div(v_if: "isNumberColumn(lineItem.Key)"))
                                                {
                                                    InputText(@class: "form-control", v_model: $"lineItem.Number", v_if: $"lineItem.{nameof(ManagerServer.Model.AdvancedQuery.WhereLine.NumberFilter)} == 0 || lineItem.{nameof(ManagerServer.Model.AdvancedQuery.WhereLine.NumberFilter)} == 1");
                                                }
                                                using (Div(v_if: "isDecimalColumn(lineItem.Key)"))
                                                {
                                                    InputText(@class: "form-control", v_model: $"lineItem.Decimal", v_if: $"lineItem.{nameof(ManagerServer.Model.AdvancedQuery.WhereLine.DecimalFilter)} == 0 || lineItem.{nameof(ManagerServer.Model.AdvancedQuery.WhereLine.DecimalFilter)} == 1");
                                                }
                                                foreach (var e in columns)
                                                {
                                                    var valueType = e.GetType().GetGenericArguments()[0];
                                                    if (valueType.IsEnum)
                                                    {
                                                        using (Div(v_if: $"lineItem.Key == '{e.Key}'"))
                                                        {
                                                            VSelect(valueType, "EnumValue");
                                                        }
                                                    }
                                                }
                                            }
                                            using (Td(style: "vertical-align: top"))
                                            {
                                                using (Div(v_if: "Where.length > 1"))
                                                {
                                                    using (Button(type: "button", @class: "btn", style: "height: 30px; font-size: 24px; font-weight: bold; color: #ccc; padding: 0px 3px", v_on_click: "Where.splice(index, 1)")) Write("&times;");
                                                }
                                            }
                                        }
                                    }
                                }

                                using (Button(type: "button", v_on_click: $"addWhereLine()", @class: "btn btn-sm mt-2")) Write(Strings.AddLine);
                            }
                        }
                    }

                    using (Div(@class: "flex items-start gap-2 my-1"))
                    {
                        InputCheckbox(id: "HasOrderBy", @class: "form-check-input", value: "true", v_model: "HasOrderBy");
                        using (Div(@class: "w-full"))
                        {
                            using (Label(@for: "HasOrderBy"))
                            {
                                Write(Strings.HasOrderBy);
                            }
                            using (Div(@class: "form-group", v_if: "HasOrderBy"))
                            {
                                using (Table())
                                {
                                    using (TBody(v_model: "OrderBy", @is: "draggable", tag: "tbody", handle: ".handle"))
                                    {
                                        using (Tr(v_for: "(lineItem, index) in OrderBy"))
                                        {
                                            using (Td(@class: "handle cursor-move"))
                                            {
                                                using (Div(style: "display: table; border-collapse: separate; width: 100%"))
                                                {
                                                    using (Span(@class: "form-control text-center whitespace-nowrap"))
                                                    {
                                                        I(@class: "fas fa-arrows-v");
                                                    }
                                                }
                                            }
                                            using (Td())
                                            {
                                                using (Select(v_model: "lineItem.Key", @class: "form-select"))
                                                {
                                                    Option();
                                                    foreach (var e in columns)
                                                    {
                                                        Option(value: e.Key.Value.ToString(), text: e.Label);
                                                    }
                                                }
                                            }
                                            using (Td())
                                            {
                                                using (Select(v_model: "lineItem.SortOrder", @class: "form-select"))
                                                {
                                                    Option(text: Strings.Ascending, value: ((int)ManagerServer.Model.Enums.SortOrder.Ascending).ToString());
                                                    Option(text: Strings.Descending, value: ((int)ManagerServer.Model.Enums.SortOrder.Descending).ToString());
                                                }
                                            }
                                            using (Td(style: "vertical-align: top"))
                                            {
                                                using (Div(v_if: "OrderBy.length > 1"))
                                                {
                                                    using (Button(type: "button", @class: "btn", style: "height: 30px; font-size: 24px; font-weight: bold; color: #ccc; padding: 0px 3px", v_on_click: "OrderBy.splice(index, 1)")) Write("&times;");
                                                }
                                            }
                                        }
                                    }
                                }
                                using (Button(type: "button", v_on_click: "OrderBy.push({})", @class: "btn btn-sm mt-2")) Write(Strings.AddLine);
                            }
                        }
                    }

                    using (Div(@class: "flex items-start gap-2 my-1"))
                    {
                        InputCheckbox(id: "HasGroupBy", @class: "form-check-input", value: "true", v_model: "HasGroupBy");
                        using (Div(@class: "w-full"))
                        {
                            using (Label(@for: "HasGroupBy"))
                            {
                                Write(Strings.HasGroupBy);
                            }

                            /*
                            using (Div(@class: "form-group", v_if: "HasGroupBy"))
                            {
                                using (Select(v_model: "GroupBy", @class: "form-control", style: "width: auto"))
                                {
                                    Option();
                                    foreach (var e in columns)
                                    {
                                        Option(value: e.Key.Value.ToString(), text: e.Label);
                                    }
                                }
                            }
                            */

                            using (Div(@class: "form-group", v_if: "HasGroupBy"))
                            {
                                using (Table())
                                {
                                    using (TBody(v_model: "GroupBy", @is: "draggable", tag: "tbody", handle: ".handle"))
                                    {
                                        using (Tr(v_for: "(lineItem, index) in GroupBy"))
                                        {
                                            using (Td(@class: "handle cursor-move"))
                                            {
                                                using (Div(style: "display: table; border-collapse: separate; width: 100%"))
                                                {
                                                    using (Span(@class: "form-control text-center whitespace-nowrap"))
                                                    {
                                                        I(@class: "fas fa-arrows-v");
                                                    }
                                                }
                                            }
                                            using (Td())
                                            {
                                                using (Select(v_model: "lineItem.Key", @class: "form-select"))
                                                {
                                                    Option();
                                                    foreach (var e in columns)
                                                    {
                                                        Option(value: e.Key.Value.ToString(), text: e.Label);
                                                    }
                                                }
                                            }
                                            using (Td(style: "vertical-align: top"))
                                            {
                                                using (Div(v_if: "GroupBy.length > 1"))
                                                {
                                                    using (Button(type: "button", @class: "btn", style: "height: 30px; font-size: 24px; font-weight: bold; color: #ccc; padding: 0px 3px", v_on_click: "GroupBy.splice(index, 1)")) Write("&times;");
                                                }
                                            }
                                        }
                                    }
                                }
                                using (Button(type: "button", v_on_click: "GroupBy.push({})", @class: "btn btn-sm mt-2")) Write(Strings.AddLine);
                            }
                        }
                    }

#if DEBUG
                    //using (Pre(@class: "mt-8")) Write("{{ JSON.stringify($data, null, 2) }}");
#endif
                }

                using (Div(@class: "card-header flex gap-4"))
                {
                    using (PostForm())
                    {
                        InputHidden(name: "Json", v_model: "JSON.stringify($data, null, 2)");
                        if (!AdvancedSearch.HasValue)
                        {
                            using (Button(@class: "btn btn-primary")) Write(Strings.Create);
                        }
                        else
                        {
                            using (Button(@class: "btn btn-success")) Write(Strings.Update);
                        }
                    }

                    if (AdvancedSearch.HasValue)
                    {
                        using (PostForm())
                        {
                            InputHidden(name: "Json", value: string.Empty);
                            using (Button(@class: "btn btn-danger")) Write(Strings.Delete);
                        }
                    }
                }

                Script("resources/vue/vue.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
                Script("resources/sortable/sortable.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Dependency for VueDraggable
                Script("resources/vuedraggable/vuedraggable.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Required for reordering rows            
                Script("resources/datepicker/date-picker.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Date pickers

                using (Script())
                {
                    var advancedSearch = ApplicationData.Businesses.Get(Business).SingleOrDefault<AdvancedQuery>(AdvancedSearch ?? CopyToNewAdvancedSearch) ?? new AdvancedQuery();
                    if (advancedSearch.Select == null) advancedSearch.Select = columns.Where(x => x.Visible && x.Key != new Guid("e86f12dd-2bfc-4eef-a7b0-71e4e9caeda9")).Select(x => new ManagerServer.Model.AdvancedQuery.SelectLine() { Key = x.Key.Value }).ToArray();
                    if (advancedSearch.Where == null) advancedSearch.Where = new AdvancedQuery.WhereLine[1] { new ManagerServer.Model.AdvancedQuery.WhereLine() };
                    if (advancedSearch.OrderBy == null) advancedSearch.OrderBy = new AdvancedQuery.OrderByLine[1] { new ManagerServer.Model.AdvancedQuery.OrderByLine() };
                    if (advancedSearch.GroupBy == null) advancedSearch.GroupBy = new AdvancedQuery.GroupByLine[1] { new ManagerServer.Model.AdvancedQuery.GroupByLine() };

                    var methods = new Dictionary<string, Guid[]>();
                    methods.Add("isDateColumn", columns.Where(x => IsDateColumn(x)).Select(x => x.Key.Value).ToArray());
                    methods.Add("isStringColumn", columns.Where(x => IsStringColumn(x)).Select(x => x.Key.Value).ToArray());
                    methods.Add("isNumberColumn", columns.Where(x => IsNumberColumn(x)).Select(x => x.Key.Value).ToArray());
                    methods.Add("isDecimalColumn", columns.Where(x => IsDecimalColumn(x)).Select(x => x.Key.Value).ToArray());
                    methods.Add("isBooleanColumn", columns.Where(x => IsBooleanColumn(x)).Select(x => x.Key.Value).ToArray());
                    methods.Add("isEnumColumn", columns.Where(x => IsEnumColumn(x)).Select(x => x.Key.Value).ToArray());

                    var methods2 = new List<string>();
                    foreach (var e in methods)
                    {
                        var sb = new StringBuilder();
                        sb.Append($"{e.Key}: function(key) {{");
                        sb.Append("if (key == null) return false;");
                        foreach (var e2 in e.Value)
                        {
                            sb.Append($"if (key == '{e2}') return true;");
                        }
                        sb.Append("return false;");
                        sb.Append("}");
                        methods2.Add(sb.ToString());
                    }

                    methods2.Add($"addWhereLine: function() {{ this.Where.push({Newtonsoft.Json.JsonConvert.SerializeObject(new ManagerServer.Model.AdvancedQuery.WhereLine())}) }}");

                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(advancedSearch);
                    Write(@"app = new Vue({ el: ""#v-model-form"", data: " + json + ", methods: { " + string.Join($",{Environment.NewLine}", methods2) + " } })");
                }
            }
        }

        private static bool IsBooleanColumn(Column column)
        {
            if (column is Column<bool>) return true;
            return false;
        }

        private static bool IsDecimalColumn(Column column)
        {
            if (column is Column<decimal>) return true;
            if (column is Column<decimal?>) return true;
            if (column is Column<Tuple<decimal, ManagerServer.Model.Currency>>) return true;
            if (column is Column<Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>>) return true;
            if (column is Column<Tuple<decimal, BusinessTemplate>>) return true;
            return false;
        }

        private static bool IsNumberColumn(Column column)
        {
            if (column is Column<int>) return true;
            if (column is Column<int?>) return true;
            if (column is Column<Tuple<int, BusinessTemplate>>) return true;
            return false;
        }

        private static bool IsDateColumn(Column column)
        {
            if (column is Column<DateTime>) return true;
            if (column is Column<DateTime?>) return true;
            return false;
        }

        private static bool IsStringColumn(Column column)
        {
            if (column is Column<string>) return true;
            if (column is Column<string[]>) return true;
            if (column is Column<NamedObject>) return true;
            return false;
        }

        private static bool IsEnumColumn(Column column)
        {
            if (column.GetType().IsGenericType)
            {
                var valueType = column.GetType().GetGenericArguments()[0];
                if (valueType.IsEnum) return true;
            }
            return false;
        }

        private void VSelect(Type enumType, string field)
        {
            using (Select(@class: "form-select", v_model: $"lineItem.{field}"))
            {
                foreach (var e2 in Enum.GetValues(enumType))
                {
                    var value = (int)e2;
                    Option(value: value.ToString(), text: ManagerServer.Globalization.Strings.GetPropertyValue(e2.ToString()));
                }
            }
        }

        private void VInputDate(string fieldName)
        {
            var datePattern = "";
            var groups = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern.ToLowerInvariant().Where(x => x == 'd' || x == 'm' || x == 'y').GroupBy(x => x);
            if (groups.Count() != 3)
            {
                datePattern = "yyyy-mm-dd";
            }
            else
            {
                var list = new List<string>();
                foreach (var e in groups)
                {
                    if (e.Key == 'y') list.Add("YYYY");
                    else if (e.Key == 'm') list.Add("M");
                    else if (e.Key == 'd') list.Add("D");
                }

                var dateSeparator = " ";
                var shortDatePattern = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
                if (shortDatePattern.Contains('/')) dateSeparator = "/";
                else if (shortDatePattern.Contains('-')) dateSeparator = "-";
                else if (shortDatePattern.Contains('.')) dateSeparator = ".";

                datePattern = string.Join(dateSeparator, list.ToArray());
            }

            var weekStart = 1;
            if (ApplicationData.Businesses.Get(Business).Exists<ManagerServer.Model.DateAndNumberFormat>())
            {
                var regionFormats = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.DateAndNumberFormat>();
                weekStart = (int)regionFormats.FirstDayOfWeek;
            }

            using (Div())
            {
                Write(@"<date-picker v-model=""" + fieldName + @""" :lang=""{ formatLocale: { firstDayOfWeek: " + (int)System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek + @" } }"" partial-update=""true"" type=""date"" value-type=""YYYY-M-D"" format=""" + datePattern + @"""></date-picker>");
            }
        }

        protected override async Task InnerPost()
        {
            if (EditAdvancedSearch)
            {
                if (Request.HasFormContentType)
                {
                    var form = await Request.ReadFormAsync();
                    var json = form["Json"];

                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var jsonSettings = new JsonSerializerSettings()
                        {
                            Error = (se, ev) => ev.ErrorContext.Handled = true
                        };
                        var customTable = Newtonsoft.Json.JsonConvert.DeserializeObject<ManagerServer.Model.AdvancedQuery>(json, jsonSettings);
                        customTable.NakedTable = this.GetType().GetCustomAttribute<GuidAttribute>().Value;
                        customTable.Key = AdvancedSearch ?? Guid.CreateVersion7();

                        ApplicationData.Businesses.Process(Business, customTable, GetUserName());

                        if (!AdvancedSearch.HasValue)
                        {
                            var redirect = (NakedObjectsWithAdvancedQueries)this.MemberwiseClone();
                            redirect.EditAdvancedSearch = false;
                            redirect.AdvancedSearch = customTable.Key;
                            redirect.ExpandGroupBy = false;
                            redirect.Term = null;
                            redirect.Referrer = null;
                            redirect.DetailsOpen = true;
                            Response.Redirect(redirect.ToUrl());
                        }
                        else
                        {
                            var redirect = (NakedObjectsWithAdvancedQueries)this.MemberwiseClone();
                            redirect.EditAdvancedSearch = false;
                            redirect.DetailsOpen = true;
                            redirect.Referrer = null;
                            Response.Redirect(redirect.ToUrl());
                        }
                    }
                    else
                    {
                        ApplicationData.Businesses.Process(Business, AdvancedSearch.Value, GetUserName());
                        var redirect = (NakedObjectsWithAdvancedQueries)this.MemberwiseClone();
                        redirect.EditAdvancedSearch = false;
                        redirect.AdvancedSearch = null;
                        redirect.Referrer = null;
                        Response.Redirect(redirect.ToUrl());
                    }
                    return;
                }
            }

            await base.InnerPost();
        }
    }
}
