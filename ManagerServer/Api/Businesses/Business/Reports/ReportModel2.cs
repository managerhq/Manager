using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business.Reports.V2
{
    internal sealed class ReportModel2
    {
        public string Business { get; set; }
        public string Title { get; set; }
        public List<string> Subtitles { get; set; } = new();
        public List<Column> Columns { get; set; } = new();
        public List<Row> Rows { get; set; } = new();
        public string Footer { get; set; }
        public string Direction { get; set; }

        public void Round()
        {
            RoundRows(Rows);
        }

        public void Prune(bool excludeZeroBalances)
        {
            PruneRows(Rows, excludeZeroBalances);
        }

        public void Format()
        {
            FormatRows(Rows);
        }

        public void Collapse(params Guid?[] keys)
        {
            if (keys == null || keys.Length == 0) return;
            CollapseRows(Rows, [.. keys]);
        }

        public Row Extract(Guid key)
        {
            return ExtractRow(Rows, key);
        }

        private static Row ExtractRow(List<Row> rows, Guid key)
        {
            if (rows == null) return null;

            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                if (row.Key == key)
                {
                    rows.RemoveAt(i);
                    return row;
                }
                var found = ExtractRow(row.Rows, key);
                if (found != null) return found;
            }
            return null;
        }

        private static void CollapseRows(List<Row> rows, HashSet<Guid?> keys)
        {
            if (rows == null) return;

            foreach (var row in rows)
            {
                if (row.Key.HasValue && keys.Contains(row.Key) && row.Rows != null)
                {
                    row.Cells = row.Cells;
                    row.Rows = null;
                }
                else
                {
                    CollapseRows(row.Rows, keys);
                }
            }
        }

        private static void RoundRows(List<Row> rows)
        {
            if (rows == null) return;

            foreach (var row in rows)
            {
                if (row.Rows != null)
                {
                    RoundRows(row.Rows);
                }
                else if (row.Cells != null)
                {
                    foreach (var cell in row.Cells)
                    {
                        if (cell?.Value != null)
                        {
                            cell.Value = Math.Round(cell.Value.Value, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }
        }

        private static void PruneRows(List<Row> rows, bool excludeZeroBalances)
        {
            if (rows == null) return;

            for (int i = rows.Count - 1; i >= 0; i--)
            {
                var row = rows[i];
                if (row.Rows != null)
                {
                    PruneRows(row.Rows, excludeZeroBalances);
                    if (row.Rows.Count == 0) rows.RemoveAt(i);
                }
                else if (excludeZeroBalances && row.Cells != null && row.Cells.All(c => (c?.Value ?? 0m) == 0m))
                {
                    rows.RemoveAt(i);
                }
            }
        }

        private static void FormatRows(List<Row> rows)
        {
            if (rows == null) return;

            foreach (var row in rows)
            {
                if (row.Rows != null)
                {
                    FormatRows(row.Rows);
                }
                else if (row.Cells != null)
                {
                    foreach (var cell in row.Cells)
                    {
                        if (cell?.Value.HasValue == true)
                        {
                            cell.Text = ReportNumberFormat.Format(cell.Value, cell.Style, wholeNumbers: false);
                        }
                    }
                }
            }
        }
    }

    internal sealed class Column
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public bool IsBold { get; set; }
        public bool HideTotals { get; set; }

        public List<Column> Subcolumns { get; set; }
    }

    internal enum NumberStyle
    {
        Currency,
        Quantity,
        Percentage,
        DebitCredit,
        CurrencyParentheses,
    }

    internal sealed class Row
    {
        private List<Cell> _cells;

        public bool IsBold { get; set; }
        public Guid? Key { get; set; }
        public string Name { get; set; }
        public List<Row> Rows { get; set; }

        public List<Cell> Cells
        {
            get
            {
                if (Rows == null) return _cells;

                var totals = new List<Cell>();
                foreach (var child in Rows)
                {
                    var childCells = child.Cells;
                    if (childCells == null) continue;
                    for (int i = 0; i < childCells.Count; i++)
                    {
                        if (totals.Count <= i) totals.Add(new Cell { Style = childCells[i]?.Style ?? NumberStyle.Currency });
                        var value = childCells[i]?.Value;
                        if (value.HasValue)
                        {
                            totals[i].Value = (totals[i].Value ?? 0m) + value.Value;
                        }
                    }
                }
                return totals;
            }
            set => _cells = value;
        }

        public static Row Combine(params Row[] rows)
        {
            var combined = new Row { Rows = [.. rows] };
            combined.Cells = combined.Cells;
            combined.Rows = null;
            return combined;
        }

        public void Negate()
        {
            if (Rows != null)
            {
                foreach (var child in Rows) child.Negate();
            }
            else if (_cells != null)
            {
                foreach (var cell in _cells)
                {
                    if (cell?.Value != null) cell.Value = -cell.Value.Value;
                }
            }
        }
    }

    internal sealed class Cell
    {
        public decimal? Value { get; set; }
        public string Text { get; set; }
        public Link Link { get; set; }

        [JsonIgnore]
        public NumberStyle Style { get; set; }
    }
}
