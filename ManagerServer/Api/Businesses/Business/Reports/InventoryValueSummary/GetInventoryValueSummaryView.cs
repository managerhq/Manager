using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryValueSummary
{
    [ProtoContract]
    internal sealed class GetInventoryValueSummaryView : GetReportView<Model.InventoryValueSummary>
    {
        protected override string DefaultTitle => Strings.InventoryValueSummary;

        protected override ReportModel Build(Database business, Model.InventoryValueSummary report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            model.Columns.Add(new Column { Name = Strings.OpeningBalance });
            model.Columns.Add(new Column { Name = Strings.Purchases });
            model.Columns.Add(new Column { Name = Strings.ProductionOrders });
            model.Columns.Add(new Column { Name = Strings.CostOfSales });
            model.Columns.Add(new Column { Name = Strings.Adjustments });
            model.Columns.Add(new Column { Name = Strings.ClosingBalance, IsBold = true });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Currency, model.WholeNumbers, link);

            var inventoryItems = business.OfType<ManagerServer.Model.InventoryItem>().ToDictionary(x => x.Key);
            var transactionsByItem = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Revaluate(report.FromDate, report.ToDate).Where(x => x.Date <= report.ToDate && x.GeneralLedgerAccount.IsInventoryOnHand).GroupBy(x => x.InventoryItem).ToDictionary(x => x.Key, x => x.ToArray());

            foreach (var e in inventoryItems.Values.OrderBy(x => x.NameWithCode))
            {
                if (!transactionsByItem.ContainsKey(e)) continue;
                var itemTransactions = transactionsByItem[e];

                var openingBalance = itemTransactions.Where(x => x.Date < report.FromDate).Sum(x => x.AccountAmount);
                var purchases = itemTransactions.Where(x => x.Date >= report.FromDate && x.Date <= report.ToDate && (x.Transaction is ManagerServer.Model.ExpenseClaim || x.Transaction is ManagerServer.Model.Payment || x.Transaction is ManagerServer.Model.PurchaseInvoice)).Sum(x => x.AccountAmount);
                var productions = itemTransactions.Where(x => x.Date >= report.FromDate && x.Date <= report.ToDate && x.Transaction is ManagerServer.Model.ProductionOrder).Sum(x => x.AccountAmount);
                var sales = itemTransactions.Where(x => x.Date >= report.FromDate && x.Date <= report.ToDate && (x.Transaction is ManagerServer.Model.Receipt || x.Transaction is ManagerServer.Model.SalesInvoice)).Sum(x => x.AccountAmount);
                var adjustments = itemTransactions.Where(x => x.Date >= report.FromDate && x.Date <= report.ToDate && (x.Transaction is ManagerServer.Model.InventoryWriteOff || x.Transaction is ManagerServer.Model.JournalEntry || x.Transaction is ManagerServer.Model.CreditNote || x.Transaction is ManagerServer.Model.DebitNote || x.Transaction == null)).Sum(x => x.AccountAmount);
                var closingBalance = itemTransactions.Where(x => x.Date <= report.ToDate).Sum(x => x.AccountAmount);

                if (report.ExcludeItemsWithNoMovement && purchases == 0m && productions == 0m && sales == 0m && adjustments == 0m) continue;

                model.Rows.Items.Add(new Row
                {
                    Name = e.NameWithCode,
                    Cells =
                    [
                        Make(openingBalance),
                        Make(purchases),
                        Make(productions),
                        Make(sales),
                        Make(adjustments),
                        Make(closingBalance),
                    ],
                });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
