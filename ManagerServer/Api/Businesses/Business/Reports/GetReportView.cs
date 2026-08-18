using ManagerServer.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports
{
    internal abstract class GetReportView<T> : ViewEndpoint<ReportModel>, IView where T : Model.Object, new()
    {
        public sealed override ReportModel AuthorizedHandle()
        {
            var business = GetApplicationData().Businesses.Get(Business);
            var report = business.SingleOrDefault<T>(Key);
            if (report == null) return null;

            Languages.SetLanguage(Language);

            var model = Build(business, report);
            if (model == null) return null;

            if (string.IsNullOrEmpty(model.Title)) model.Title = DefaultTitle;

            var businessDetails = business.Single<Model.BusinessDetails>();
            model.Business = businessDetails.Name;
            model.Direction = Languages.IsRightToLeft() ? "rtl" : "ltr";
            if (string.IsNullOrWhiteSpace(model.Business)) model.Business = Business;

            var leaves = FlattenColumns(model.Columns);
            PrepareTotals(model.Rows, leaves, model.WholeNumbers);
            ApplyIsLess(model.Rows, parentIsLess: false, model.WholeNumbers);

            return model;
        }

        private static List<Column> FlattenColumns(List<Column> columns)
        {
            var leaves = new List<Column>();
            if (columns == null) return leaves;
            foreach (var c in columns)
            {
                if (c.Subcolumns != null && c.Subcolumns.Count > 0) leaves.AddRange(c.Subcolumns);
                else leaves.Add(c);
            }
            return leaves;
        }

        private static void PrepareTotals(Rows rows, List<Column> leaves, bool wholeNumbers)
        {
            if (rows == null) return;

            foreach (var row in rows.Items)
            {
                if (row.Rows != null) PrepareTotals(row.Rows, leaves, wholeNumbers);
            }

            int columnCount = leaves.Count;
            if (columnCount == 0) return;

            List<Cell> template = null;
            foreach (var row in rows.Items)
            {
                if (row.IsTotalRow) continue;
                if (row.Cells != null) { template = row.Cells; break; }
                if (row.Rows?.TotalCells != null) { template = row.Rows.TotalCells; break; }
            }

            bool allHidden = leaves.All(c => c.HideTotals);

            List<Cell> BuildCells(decimal?[] values)
            {
                var result = new List<Cell>(columnCount);
                for (int i = 0; i < columnCount; i++)
                {
                    if (leaves[i].HideTotals) { result.Add(null); continue; }
                    var style = (template != null && i < template.Count && template[i] != null) ? template[i].Style : NumberStyle.Currency;
                    result.Add(ReportNumberFormat.Cell(values[i], style, wholeNumbers));
                }
                return result;
            }

            var sums = new decimal?[columnCount];
            foreach (var row in rows.Items)
            {
                if (row.IsTotalRow)
                {
                    // Running subtotal: snapshot sums so each IsTotalRow reflects rows before it.
                    if (row.Cells == null) row.Cells = BuildCells(sums);
                    continue;
                }
                var source = row.Cells ?? row.Rows?.TotalCells;
                if (source == null) continue;
                for (int i = 0; i < columnCount && i < source.Count; i++)
                {
                    if (source[i]?.Value is decimal v) sums[i] = (sums[i] ?? 0m) + v;
                }
            }

            if (rows.TotalCells == null) rows.TotalCells = BuildCells(sums);

            rows.RenderTotalRow = !rows.HideTotals && !allHidden && !string.IsNullOrEmpty(rows.TotalText);

            foreach (var row in rows.Items)
            {
                if (row.Rows == null) continue;
                if (row.Rows.HideTotals) continue;
                if (!string.IsNullOrEmpty(row.Rows.TotalText)) continue;
                if (string.IsNullOrWhiteSpace(row.Name)) continue;
                if (allHidden) continue;
                int childCount = row.Rows.Items.Count(r => !r.IsTotalRow);
                if (childCount <= 1 && !row.Rows.MakeTotalStandOut) continue;
                row.GroupTotalText = Strings.Total + " — " + row.Name;
            }
        }

        private static void ApplyIsLess(Rows rows, bool parentIsLess, bool wholeNumbers)
        {
            if (rows == null) return;
            var effectiveLess = parentIsLess || rows.IsLess;

            foreach (var row in rows.Items)
            {
                if (row.IsTotalRow) continue;
                if (row.Cells != null && effectiveLess) NegateCells(row.Cells, wholeNumbers);
                if (row.Rows != null) ApplyIsLess(row.Rows, effectiveLess, wholeNumbers);
            }

            if (rows.TotalCells != null && effectiveLess) NegateCells(rows.TotalCells, wholeNumbers);
        }

        private static void NegateCells(List<Cell> cells, bool wholeNumbers)
        {
            foreach (var cell in cells)
            {
                if (cell?.Value is decimal v)
                {
                    cell.Value = -v;
                    cell.Text = ReportNumberFormat.Format(cell.Value, cell.Style, wholeNumbers);
                }
            }
        }

        protected abstract ReportModel Build(Database business, T report);

        public View GetView()
        {
            var o = AuthenticatedHandle();
            return ViewMapper.From(o);
        }

        protected abstract string DefaultTitle { get; }
    }
}
