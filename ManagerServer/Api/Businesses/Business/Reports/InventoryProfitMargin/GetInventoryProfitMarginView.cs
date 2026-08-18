using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryProfitMargin;
using System;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryProfitMargin
{
    [ProtoContract]
    internal sealed class GetInventoryProfitMarginView : GetReportView<Model.InventoryProfitMargin>
    {
        protected override string DefaultTitle => Strings.InventoryProfitMargin;

        protected override ReportModel Build(Database business, Model.InventoryProfitMargin report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            model.Columns.Add(new Column { Key = "sales", Name = Strings.Sales });
            model.Columns.Add(new Column { Key = "costOfSales", Name = Strings.CostOfSales });
            model.Columns.Add(new Column { Key = "profit", Name = Strings.Profit });
            model.Columns.Add(new Column { Name = Strings.Margin, HideTotals = true });

            Cell Curr(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Currency, model.WholeNumbers, link);
            Cell Pct(decimal? v) => ReportNumberFormat.Cell(v, NumberStyle.Percentage, model.WholeNumbers);

            var items = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.InventoryItem != null || x.InventoryKit != null)
                .Where(x => !x.GeneralLedgerAccount.IsInventoryOnHand && !x.IsTaxTransaction && x.BaseAmount != 0m)
                .Where(x => (x.SalesInvoiceAsTransaction != null && x.SalesInvoiceAsTransaction.IssueDate >= report.FromDate && x.SalesInvoiceAsTransaction.IssueDate <= report.ToDate) || (x.CreditNote != null && x.CreditNote.IssueDate >= report.FromDate && x.CreditNote.IssueDate <= report.ToDate) || (x.Receipt != null && x.Receipt.Date >= report.FromDate && x.Receipt.Date <= report.ToDate))
                .GroupBy(x => x.Item)
                .OrderBy(x => x.Key.GetNameWithCode());

            foreach (var e in items)
            {
                var sales = e.Where(x => !x.IsCostOfGoodsSold).Sum(x => x.BaseAmount) * -1m;
                var costOfSales = e.Where(x => x.IsCostOfGoodsSold).Sum(x => x.BaseAmount) * -1m;
                var profit = sales + costOfSales;
                var margin = 0m;
                if (profit != 0m && sales != 0m) margin = Math.Round(profit / (sales / 100m), 2, MidpointRounding.AwayFromZero);

                model.Rows.Items.Add(new Row
                {
                    Name = e.Key.GetNameWithCode(),
                    Cells =
                    [
                        Curr(sales),
                        Curr(costOfSales),
                        Curr(profit, new Link(new InventoryProfitMarginTransactions { Business = Business, Referrer = Referrer, Item = e.Key.Key, From = report.FromDate, To = report.ToDate }.ToUrl())),
                        Pct(margin),
                    ],
                });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
