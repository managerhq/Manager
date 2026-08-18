using FastMember;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.CustomReports
{
    [ProtoContract]
    internal sealed class GetCustomReportView : GetObjectViewEndpoint<CustomReport>
    {
        protected override string DefaultTitle => Strings.CustomReport;

        protected override View Build(Database business, CustomReport report)
        {
            ViewModel viewModel;
            try
            {
                viewModel = BuildViewModel(report, Business);
            }
            catch
            {
                return null;
            }

            var view = new View { Title = viewModel.Name };
            if (!string.IsNullOrWhiteSpace(viewModel.Description))
            {
                view.Subtitles.Add(viewModel.Description);
            }

            if (viewModel.Columns == null) return view;

            var columns = viewModel.Columns.ToArray();
            foreach (var c in columns)
            {
                view.Table.Columns.Add(new View.ColumnInfo
                {
                    Key = c.Key,
                    Label = c.Name,
                    Align = c.IsDecimal ? Align.Right : Align.Start,
                });
            }

            if (viewModel.Groups != null)
            {
                foreach (var g in viewModel.Groups)
                {
                    view.Table.Rows.Add(BuildGroupRow(g, columns));
                }
                view.Table.Rows.Add(BuildTotalRow(viewModel.Groups.SelectMany(x => x.GetAllRows()), columns));
            }
            else if (viewModel.Rows != null)
            {
                foreach (var r in viewModel.Rows)
                {
                    view.Table.Rows.Add(BuildDataRow(r, columns));
                }
                view.Table.Rows.Add(BuildTotalRow(viewModel.Rows, columns));
            }

            return view;
        }

        private static View.RowInfo BuildGroupRow(GroupRow group, Column[] columns)
        {
            var header = GetText(group.Value);
            if (string.IsNullOrWhiteSpace(header)) header = Strings.Empty;

            var row = new View.RowInfo
            {
                Cells = new List<View.CellInfo> { new View.CellInfo { Text = header } },
                Rows = new List<View.RowInfo>(),
            };

            if (group.Groups != null)
            {
                foreach (var g in group.Groups) row.Rows.Add(BuildGroupRow(g, columns));
            }
            if (group.Rows != null)
            {
                foreach (var r in group.Rows) row.Rows.Add(BuildDataRow(r, columns));
            }

            row.Rows.Add(BuildTotalRow(group.GetAllRows(), columns));
            return row;
        }

        private static View.RowInfo BuildDataRow(Row row, Column[] columns)
        {
            var cells = row.Cells.ToList();
            var output = new List<View.CellInfo>(columns.Length);
            for (int i = 0; i < columns.Length; i++)
            {
                var value = i < cells.Count ? cells[i] : null;
                output.Add(new View.CellInfo { Text = GetText(value) });
            }
            return new View.RowInfo { Cells = output };
        }

        private static View.RowInfo BuildTotalRow(IEnumerable<Row> rows, Column[] columns)
        {
            var sums = new decimal?[columns.Length];
            foreach (var r in rows)
            {
                var decimals = r.GetDecimals();
                for (int i = 0; i < columns.Length && i < decimals.Length; i++)
                {
                    if (decimals[i].HasValue)
                    {
                        sums[i] = (sums[i] ?? 0m) + decimals[i].Value;
                    }
                }
            }

            var cells = new List<View.CellInfo>(columns.Length);
            for (int i = 0; i < columns.Length; i++)
            {
                var text = columns[i].IsDecimal && sums[i].HasValue ? sums[i].Value.ToNumberString() : string.Empty;
                cells.Add(new View.CellInfo { Text = text });
            }
            return new View.RowInfo { IsTotalRow = true, Cells = cells };
        }

        private static string GetText(object o)
        {
            if (o == null) return string.Empty;
            if (o is NamedObject named) return named.GetName();
            if (o is DateTime date) return date.ToLocalShortDisplayString();
            if (o is decimal number) return number.ToNumberString();
            return o.ToString();
        }

        private sealed class ViewModel
        {
            public string Name;
            public string Description;
            public IEnumerable<Column> Columns;
            public IEnumerable<GroupRow> Groups;
            public IEnumerable<Row> Rows;
        }

        private sealed class Column
        {
            public string Key;
            public string Name;
            public bool IsDecimal;
        }

        private sealed class GroupRow
        {
            public object Value;
            public IEnumerable<GroupRow> Groups;
            public IEnumerable<Row> Rows;

            public IEnumerable<Row> GetAllRows()
            {
                var rows = new List<Row>();
                if (Groups != null) rows.AddRange(Groups.SelectMany(x => x.GetAllRows()));
                if (Rows != null) rows.AddRange(Rows);
                return rows;
            }
        }

        private sealed class Row
        {
            public IEnumerable<object> Cells;

            public decimal?[] GetDecimals()
            {
                return Cells.Select(x => x is decimal d ? (decimal?)d : null).ToArray();
            }
        }

        private static ViewModel BuildViewModel(CustomReport report, string fileId)
        {
            var viewModel = new ViewModel();
            viewModel.Name = report.Name;
            viewModel.Description = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            var type = typeof(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction);

            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(fileId)
                .DisposeFixedAssets()
                .DisposeIntangibleAssets();

            if (report.AccountingMethod == ManagerServer.Model.Enums.AccountingBasis.CashBasis)
            {
                generalLedger = generalLedger
                    .AutomaticallyMatchSalesInvoices()
                    .AutomaticallyMatchPurchaseInvoices()
                    .ConvertSalesInvoicesToCashBasis2(report.FromDate.AddDays(-1), report.ToDate)
                    .ConvertPurchaseInvoicesToCashBasis2(report.FromDate.AddDays(-1), report.ToDate);
            }

            generalLedger = generalLedger.Revaluate(report.FromDate, report.ToDate);

            var items = generalLedger
                .Where(x => x.Date >= report.FromDate && x.Date <= report.ToDate)
                .OrderBy(x => x.Date)
                .Select(x => ObjectAccessor.Create(x));

            if (report.HasWhere && report.Where != null)
            {
                foreach (var e in report.Where)
                {
                    var fieldType = GetFieldTypeOrPropertyType(type, e.GetFullname());
                    if (fieldType == null) continue;

                    if (fieldType == typeof(DateTime) || fieldType == typeof(DateTime?))
                    {
                        if (e.DateOperator == CustomReport.DateOperator.IsBetween)
                        {
                            items = items.Where(x => GetValue(x, e) as DateTime? >= e.StartDate && GetValue(x, e) as DateTime? <= e.EndDate);
                        }
                    }
                    if (fieldType == typeof(decimal) || fieldType == typeof(decimal?))
                    {
                        if (e.DecimalOperator == CustomReport.DecimalOperator.IsLessThan && e.Decimal.HasValue)
                        {
                            items = items.Where(x => (GetValue(x, e) as decimal? ?? 0m) < e.Decimal.Value);
                        }
                        else if (e.DecimalOperator == CustomReport.DecimalOperator.IsMoreThan && e.Decimal.HasValue)
                        {
                            items = items.Where(x => (GetValue(x, e) as decimal? ?? 0m) > e.Decimal.Value);
                        }
                        else if (e.DecimalOperator == CustomReport.DecimalOperator.IsNotZero)
                        {
                            items = items.Where(x => (GetValue(x, e) as decimal? ?? 0m) != 0m);
                        }
                        else if (e.DecimalOperator == CustomReport.DecimalOperator.IsZero)
                        {
                            items = items.Where(x => (GetValue(x, e) as decimal? ?? 0m) == 0m);
                        }
                    }
                    if (fieldType.IsSubclassOf(typeof(NamedObject)))
                    {
                        if (e.ObjectOperator == CustomReport.ObjectOperator.Is && e.Object.HasValue)
                        {
                            items = items.Where(x => (GetValue(x, e) as NamedObject)?.Key == e.Object.Value);
                        }
                        else if (e.ObjectOperator == CustomReport.ObjectOperator.IsNot && e.Object.HasValue)
                        {
                            items = items.Where(x => (GetValue(x, e) as NamedObject)?.Key != e.Object.Value);
                        }
                        else if (e.ObjectOperator == CustomReport.ObjectOperator.IsEmpty)
                        {
                            items = items.Where(x => GetValue(x, e) == null);
                        }
                        else if (e.ObjectOperator == CustomReport.ObjectOperator.IsNotEmpty)
                        {
                            items = items.Where(x => GetValue(x, e) != null);
                        }
                    }
                    if (fieldType == typeof(bool))
                    {
                        if (e.BooleanOperator == CustomReport.BooleanOperator.IsChecked)
                        {
                            items = items.Where(x => GetValue(x, e) as bool? == true);
                        }
                        else if (e.BooleanOperator == CustomReport.BooleanOperator.IsNotChecked)
                        {
                            items = items.Where(x => GetValue(x, e) as bool? == false);
                        }
                    }
                    if (fieldType == typeof(string))
                    {
                        if (e.StringOperator == CustomReport.StringOperator.Contains && !string.IsNullOrWhiteSpace(e.String))
                        {
                            items = items.Where(x => GetValue(x, e) != null && ((string)GetValue(x, e)).Contains(e.String));
                        }
                        else if (e.StringOperator == CustomReport.StringOperator.DoesNotContain && !string.IsNullOrWhiteSpace(e.String))
                        {
                            items = items.Where(x => GetValue(x, e) == null || !((string)GetValue(x, e)).Contains(e.String));
                        }
                        else if (e.StringOperator == CustomReport.StringOperator.IsEmpty)
                        {
                            items = items.Where(x => string.IsNullOrWhiteSpace(GetValue(x, e) as string));
                        }
                        else if (e.StringOperator == CustomReport.StringOperator.IsNotEmpty)
                        {
                            items = items.Where(x => !string.IsNullOrWhiteSpace(GetValue(x, e) as string));
                        }
                    }
                }
            }

            items = items.OrderBy(x => true);
            if (report.HasOrderBy && report.OrderBy != null)
            {
                foreach (var e in report.OrderBy)
                {
                    var fieldType = GetFieldTypeOrPropertyType(type, e.GetFullname());
                    if (fieldType == null) continue;

                    if (fieldType.IsSubclassOf(typeof(NamedObject)) || fieldType == typeof(IGeneralLedgerAccount))
                    {
                        if (e.SortOrder == ManagerServer.Model.Enums.SortOrder.Ascending)
                        {
                            items = ((IOrderedEnumerable<ObjectAccessor>)items).ThenBy(x => (GetValue(x, e) as NamedObject)?.GetName());
                        }
                        else
                        {
                            items = ((IOrderedEnumerable<ObjectAccessor>)items).ThenByDescending(x => (GetValue(x, e) as NamedObject)?.GetName());
                        }
                    }
                    else
                    {
                        if (e.SortOrder == ManagerServer.Model.Enums.SortOrder.Ascending)
                        {
                            items = ((IOrderedEnumerable<ObjectAccessor>)items).ThenBy(x => GetValue(x, e));
                        }
                        else
                        {
                            items = ((IOrderedEnumerable<ObjectAccessor>)items).ThenByDescending(x => GetValue(x, e));
                        }
                    }
                }
            }

            var selectElements = new List<CustomReport.SelectElement>();
            foreach (var e in report.Select)
            {
                var fieldType = GetFieldTypeOrPropertyType(type, e.GetFullname());
                if (fieldType == null) continue;
                selectElements.Add(e);
            }

            var columns = new List<Column>();
            foreach (var e in selectElements)
            {
                var columnName = e.DisplayName;

                if (string.IsNullOrWhiteSpace(columnName))
                {
                    var outerName = Strings.GetPropertyValue(e.SelectPrimaryField.Name);
                    var innerName = string.Empty;

                    if (e.SelectSecondaryField?.Name != null)
                    {
                        innerName = Strings.GetPropertyValue(e.SelectSecondaryField.Name);
                        if (e.SelectSecondaryField.Name.StartsWith("CustomFields."))
                        {
                            innerName = ApplicationData.Instance.Businesses.Get(fileId).SingleOrDefault<CustomField>(Guid.Parse(innerName.Split('.')[1]))?.Name;
                        }
                    }

                    columnName = string.Join(" ", new[] { outerName, innerName }.Where(x => !string.IsNullOrWhiteSpace(x)));
                }

                var fieldType = GetFieldTypeOrPropertyType(type, e.GetFullname());
                var isDecimal = (fieldType == typeof(decimal) || fieldType == typeof(decimal?));

                columns.Add(new Column { Key = e.SelectPrimaryField?.Name + e.SelectSecondaryField?.Name, Name = columnName, IsDecimal = isDecimal });
            }
            viewModel.Columns = columns;

            if (string.IsNullOrWhiteSpace(viewModel.Name)) viewModel.Name = Strings.CustomReport;

            var hierarchy = report.GroupBy.Where(x => GetFieldTypeOrPropertyType(type, x.GetFullname()) != null).ToArray();
            if (report.HasGroupBy && hierarchy.Length == 1 && report.GroupsToCollapse)
            {
                viewModel.Rows = GetRows(hierarchy[0], selectElements, items);
            }
            else if (report.HasGroupBy && hierarchy.Length > 0)
            {
                viewModel.Groups = GetGroups(hierarchy, selectElements, items, report.GroupsToCollapse);
            }
            else
            {
                viewModel.Rows = GetRows(selectElements, items);
            }

            return viewModel;
        }

        private static IEnumerable<GroupRow> GetGroups(IEnumerable<CustomReport.GroupByElement> hierarchy, IEnumerable<CustomReport.SelectElement> columns, IEnumerable<ObjectAccessor> transactions, bool collapseGroups)
        {
            var list = new List<GroupRow>();
            foreach (var e in transactions.GroupBy(x => GetValue(x, hierarchy.First())))
            {
                var group = new GroupRow();
                group.Value = e.Key;

                var innerHierarchy = hierarchy.Skip(1).ToArray();
                if (innerHierarchy.Length == 1 && collapseGroups)
                {
                    group.Rows = GetRows(innerHierarchy[0], columns, e);
                }
                else if (innerHierarchy.Length == 0)
                {
                    group.Rows = GetRows(columns, e);
                }
                else
                {
                    group.Groups = GetGroups(innerHierarchy, columns, e, collapseGroups);
                }
                list.Add(group);
            }
            return list;
        }

        private static IEnumerable<Row> GetRows(IEnumerable<CustomReport.SelectElement> columns, IEnumerable<ObjectAccessor> transactions)
        {
            var rows = new List<Row>();
            foreach (var e in transactions)
            {
                var row = new Row();
                var cells = new List<object>();
                foreach (var e2 in columns)
                {
                    cells.Add(GetValue(e, e2));
                }
                row.Cells = cells;
                rows.Add(row);
            }
            return rows;
        }

        private static IEnumerable<Row> GetRows(CustomReport.GroupByElement groupByElement, IEnumerable<CustomReport.SelectElement> columns, IEnumerable<ObjectAccessor> transactions)
        {
            var rows = new List<Row>();
            foreach (var e in transactions.GroupBy(x => GetValue(x, groupByElement)))
            {
                var row = new Row();
                var cells = new List<object>();
                foreach (var e2 in columns)
                {
                    decimal? total = null;
                    foreach (var e3 in e)
                    {
                        var o = GetValue(e3, e2);
                        if (o is decimal d)
                        {
                            if (!total.HasValue) total = 0m;
                            total += d;
                        }
                    }

                    if (total.HasValue) cells.Add(total.Value);
                    else cells.Add(GetValue(e.First(), e2));
                }
                row.Cells = cells;
                rows.Add(row);
            }
            return rows;
        }

        private static object GetValue(ObjectAccessor objectAccessor, CustomReport.WhereElement whereElement)
        {
            return GetValue(objectAccessor, whereElement.WherePrimaryField?.Name, whereElement.WhereSecondaryField?.Name, whereElement.WhereCustomField);
        }

        private static object GetValue(ObjectAccessor objectAccessor, CustomReport.OrderByElement orderElement)
        {
            return GetValue(objectAccessor, orderElement.OrderByPrimaryField?.Name, orderElement.OrderBySecondaryField?.Name, orderElement.OrderByCustomField);
        }

        private static object GetValue(ObjectAccessor objectAccessor, CustomReport.SelectElement selectElement)
        {
            return GetValue(objectAccessor, selectElement.SelectPrimaryField?.Name, selectElement.SelectSecondaryField?.Name, selectElement.SelectCustomField);
        }

        private static object GetValue(ObjectAccessor objectAccessor, CustomReport.GroupByElement groupElement)
        {
            return GetValue(objectAccessor, groupElement.GroupByPrimaryField?.Name, groupElement.GroupBySecondaryField?.Name, groupElement.GroupByCustomField);
        }

        private static object GetValue(ObjectAccessor objectAccessor, string name, string innerName, Guid? customField)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var value = objectAccessor[name];
            if (value == null) return null;
            if (!string.IsNullOrWhiteSpace(innerName))
            {
                if (innerName == "CustomFields")
                {
                    var dict = ObjectAccessor.Create(value)["CustomFields"] as Dictionary<Guid, string>;
                    if (dict == null) return null;
                    if (customField.HasValue && dict.ContainsKey(customField.Value)) return dict[customField.Value];
                    return null;
                }
                else if (innerName == "Name" && value is IGeneralLedgerAccount account)
                {
                    return account.GetName();
                }
                else
                {
                    try
                    {
                        value = ObjectAccessor.Create(value)[innerName];
                    }
                    catch
                    {
                        value = null;
                    }
                }
            }
            return value;
        }

        private static Type GetFieldTypeOrPropertyType(Type type, string name)
        {
            var parts = name.Split('.');

            var outerName = parts[0];
            var innerName = string.Empty;
            if (parts.Length > 1) innerName = parts[1];

            var outerType = type.GetFieldOrProperty(outerName)?.GetMemberType();

            if (outerType == null) return null;

            if (string.IsNullOrWhiteSpace(innerName)) return outerType;

            if (innerName == "CustomFields") return typeof(string);

            var innerType = outerType.GetFieldOrProperty(innerName)?.GetMemberType();

            return innerType;
        }
    }
}
