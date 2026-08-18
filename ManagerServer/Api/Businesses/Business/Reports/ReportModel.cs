using ManagerServer.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business.Reports
{
    internal sealed class ReportModel
    {
        public string Business { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Subtitle2 { get; set; }
        public bool WholeNumbers { get; set; }
        public List<Column> Columns { get; set; } = new();
        public Rows Rows { get; set; } = new();
        public string Footer { get; set; }
        public string Direction { get; set; }

        public void Prune(bool excludeZeroBalances)
        {
            PruneRows(Rows, excludeZeroBalances);
        }

        private static void PruneRows(Rows rows, bool excludeZeroBalances)
        {
            if (rows == null) return;

            for (int i = rows.Items.Count - 1; i >= 0; i--)
            {
                var row = rows.Items[i];
                if (row.IsTotalRow || row.MakeStandOut) continue;

                if (row.Rows != null)
                {
                    PruneRows(row.Rows, excludeZeroBalances);
                    if (!row.Rows.Items.Any(r => !r.IsTotalRow)) rows.Items.RemoveAt(i);
                }
                else if (excludeZeroBalances && row.Cells != null && row.Cells.All(c => (c?.Value ?? 0m) == 0m))
                {
                    rows.Items.RemoveAt(i);
                }
            }
        }
    }

    internal sealed class Column
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public bool IsBold { get; set; }

        [JsonIgnore]
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

    internal sealed class Rows
    {
        public List<Row> Items { get; set; } = new();
        public string TotalText { get; set; }

        [JsonIgnore]
        public bool IsLess { get; set; }

        [JsonIgnore]
        public bool HideLessPrefix { get; set; }

        public bool MakeTotalStandOut { get; set; }

        [JsonIgnore]
        public bool HideTotals { get; set; }

        public List<Cell> TotalCells { get; set; }
        public bool RenderTotalRow { get; set; }
    }

    internal sealed class Row
    {
        [JsonIgnore]
        public string Name { get; set; }

        public List<Cell> Cells { get; set; }
        public Rows Rows { get; set; }
        public bool MakeStandOut { get; set; }
        public bool IsTotalRow { get; set; }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(Name)) return Name;
                if (Rows != null && Rows.IsLess && !Rows.HideLessPrefix) return Strings.Less + ": " + Name;
                return Name;
            }
        }

        public string GroupTotalText { get; set; }
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
