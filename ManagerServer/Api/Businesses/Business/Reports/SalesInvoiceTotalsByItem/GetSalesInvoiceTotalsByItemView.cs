using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByItem;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.SalesInvoiceTotalsByItem
{
    [ProtoContract]
    internal sealed class GetSalesInvoiceTotalsByItemView : GetReportView<Model.SalesInvoiceTotalsByItem>
    {
        protected override string DefaultTitle => Strings.SalesInvoiceTotalsByItem;

        protected override ReportModel Build(Database business, Model.SalesInvoiceTotalsByItem report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.Periods[0].FromDate.ToLocalShortDisplayString(), report.Periods[0].ToDate.ToLocalShortDisplayString());

            for (int i = 0; i < report.Periods.Length; i++)
            {
                var columnName = report.Periods[i].ToDate.ToLocalShortDisplayString();
                if (!string.IsNullOrWhiteSpace(report.Periods[i].ColumnName)) columnName = report.Periods[i].ColumnName;
                model.Columns.Add(new Column { Name = columnName, IsBold = (i == 0) });
            }

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.CurrencyParentheses, model.WholeNumbers, link);

            var items = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => (x.Transaction is SalesInvoice || x.Transaction is CreditNote) && (x.InventoryItem != null || x.NonInventoryItem != null || x.InventoryKit != null)).GroupBy(x => x.Item);

            foreach (var e in items)
            {
                var cells = new List<Cell>();
                for (int i = 0; i < report.Periods.Length; i++)
                {
                    var amount = e.Where(x => x.Date >= report.Periods[i].FromDate && x.Date <= report.Periods[i].ToDate).Sum(x => x.AccountAmount) * -1m;
                    cells.Add(Make(amount, new Link(new SalesInvoiceTotalsByItemTransactions { Business = Business, Referrer = Referrer, Item = e.Key.Key, From = report.Periods[i].FromDate, To = report.Periods[i].ToDate }.ToUrl())));
                }

                var allZero = cells.All(c => (c.Value ?? 0m) == 0m);
                if (allZero) continue;

                model.Rows.Items.Add(new Row { Name = e.Key.GetNameWithCode(), Cells = cells });
            }

            // sort by first column descending
            model.Rows.Items.Sort((a, b) => (b.Cells?[0].Value ?? 0m).CompareTo(a.Cells?[0].Value ?? 0m));

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
