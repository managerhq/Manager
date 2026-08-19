using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryQuantitySummary;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.InventoryQuantitySummary
{
    [ProtoContract]
    internal sealed class GetInventoryQuantitySummaryView : GetReportView<Model.InventoryQuantitySummary>
    {
        protected override string DefaultTitle => Strings.InventoryQuantitySummary;

        protected override ReportModel Build(Database business, Model.InventoryQuantitySummary report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            var openingBalances = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.Date < report.FromDate)
                .Where(x => x.Qty.HasValue)
                .Where(x => x.InventoryItem != null)
                .GroupBy(x => x.InventoryItem)
                .ToLookup(x => x.Key, x => x.Sum(y => y.Qty.Value));

            var movements = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.Date >= report.FromDate && x.Date <= report.ToDate)
                .Where(x => x.Qty.HasValue)
                .Where(x => x.Qty.Value != 0m)
                .Where(x => x.InventoryItem != null)
                .GroupBy(x => new { x.InventoryItem, TransactionType = x.Transaction.GetType(), IsSale = x.IsSale })
                .Select(x => new { x.Key.InventoryItem, x.Key.TransactionType, x.Key.IsSale, Qty = x.Sum(y => y.Qty.Value) })
                .ToArray();

            var types = new System.Collections.Generic.HashSet<System.Type>(movements.Select(x => x.TransactionType).Distinct());

            model.Columns.Add(new Column { Name = Strings.OpeningBalance });
            model.Columns.Add(new Column { Name = Strings.Purchases });
            if (types.Contains(typeof(ManagerServer.Model.DebitNote))) model.Columns.Add(new Column { Name = Strings.DebitNotes });
            if (types.Contains(typeof(ManagerServer.Model.ProductionOrder))) model.Columns.Add(new Column { Name = Strings.ProductionOrders });
            if (types.Contains(typeof(ManagerServer.Model.InventoryWriteOff))) model.Columns.Add(new Column { Name = Strings.InventoryWriteOffs });
            model.Columns.Add(new Column { Name = Strings.Sales });
            if (types.Contains(typeof(ManagerServer.Model.CreditNote))) model.Columns.Add(new Column { Name = Strings.CreditNote });
            if (types.Contains(typeof(ManagerServer.Model.JournalEntry))) model.Columns.Add(new Column { Name = Strings.JournalEntries });
            model.Columns.Add(new Column { Name = Strings.ClosingBalance, IsBold = true });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Quantity, model.WholeNumbers, link);

            foreach (var e in business.OfType<ManagerServer.Model.InventoryItem>().OrderBy(x => x.NameWithCode))
            {
                var openingBalance = openingBalances[e].SingleOrDefault();

                var purchases = movements.Where(x => x.InventoryItem == e && !x.IsSale && x.TransactionType != typeof(ManagerServer.Model.ProductionOrder) && x.TransactionType != typeof(ManagerServer.Model.InventoryWriteOff) && x.TransactionType != typeof(ManagerServer.Model.JournalEntry) && x.TransactionType != typeof(ManagerServer.Model.DebitNote) && x.TransactionType != typeof(ManagerServer.Model.CreditNote)).Sum(x => x.Qty);
                var debitNotes = movements.Where(x => x.InventoryItem == e && x.TransactionType == typeof(ManagerServer.Model.DebitNote)).Sum(x => x.Qty);
                var productionOrders = movements.Where(x => x.InventoryItem == e && x.TransactionType == typeof(ManagerServer.Model.ProductionOrder)).Sum(x => x.Qty);
                var inventoryWriteOffs = movements.Where(x => x.InventoryItem == e && x.TransactionType == typeof(ManagerServer.Model.InventoryWriteOff)).Sum(x => x.Qty);
                var journalEntries = movements.Where(x => x.InventoryItem == e && x.TransactionType == typeof(ManagerServer.Model.JournalEntry)).Sum(x => x.Qty);
                var sales = movements.Where(x => x.InventoryItem == e && x.IsSale && x.TransactionType != typeof(ManagerServer.Model.ProductionOrder) && x.TransactionType != typeof(ManagerServer.Model.InventoryWriteOff) && x.TransactionType != typeof(ManagerServer.Model.JournalEntry) && x.TransactionType != typeof(ManagerServer.Model.DebitNote) && x.TransactionType != typeof(ManagerServer.Model.CreditNote)).Sum(x => x.Qty);
                var creditNotes = movements.Where(x => x.InventoryItem == e && x.TransactionType == typeof(ManagerServer.Model.CreditNote)).Sum(x => x.Qty);
                var closingBalance = openingBalance + movements.Where(x => x.InventoryItem == e).Sum(x => x.Qty);

                //                if (openingBalance == 0m && purchases == 0m && productionOrders == 0m && inventoryWriteOffs == 0m && journalEntries == 0m && sales == 0m) continue;
                //                if (report.ExcludeItemsWithNoMovement && purchases == 0m && productionOrders == 0m && inventoryWriteOffs == 0m && journalEntries == 0m && sales == 0m) continue;

                //  MOD - 19.08.2026
                if (openingBalance == 0m && purchases == 0m && debitNotes == 0m && productionOrders == 0m && inventoryWriteOffs == 0m && journalEntries == 0m && sales == 0m && creditNotes == 0m) continue;
                if (report.ExcludeItemsWithNoMovement && purchases == 0m && debitNotes == 0m && productionOrders == 0m && inventoryWriteOffs == 0m && journalEntries == 0m && sales == 0m && creditNotes == 0m) continue;
                //

                var cells = new System.Collections.Generic.List<Cell>();
                cells.Add(Make(openingBalance));
                cells.Add(Make(purchases, new Link(new InventoryQuantitySummaryTransactions { Business = Business, InventoryItem = e.Key, From = report.FromDate, To = report.ToDate, Purchases = true }.ToUrl())));
                if (types.Contains(typeof(ManagerServer.Model.DebitNote))) cells.Add(Make(debitNotes, new Link(new InventoryQuantitySummaryTransactions { Business = Business, InventoryItem = e.Key, From = report.FromDate, To = report.ToDate, DebitNotes = true }.ToUrl())));
                if (types.Contains(typeof(ManagerServer.Model.ProductionOrder))) cells.Add(Make(productionOrders, new Link(new InventoryQuantitySummaryTransactions { Business = Business, InventoryItem = e.Key, From = report.FromDate, To = report.ToDate, ProductionOrders = true }.ToUrl())));
                if (types.Contains(typeof(ManagerServer.Model.InventoryWriteOff))) cells.Add(Make(inventoryWriteOffs, new Link(new InventoryQuantitySummaryTransactions { Business = Business, InventoryItem = e.Key, From = report.FromDate, To = report.ToDate, InventoryWriteOffs = true }.ToUrl())));
                cells.Add(Make(sales, new Link(new InventoryQuantitySummaryTransactions { Business = Business, InventoryItem = e.Key, From = report.FromDate, To = report.ToDate, Sales = true }.ToUrl())));
                if (types.Contains(typeof(ManagerServer.Model.CreditNote))) cells.Add(Make(creditNotes, new Link(new InventoryQuantitySummaryTransactions { Business = Business, InventoryItem = e.Key, From = report.FromDate, To = report.ToDate, CreditNotes = true }.ToUrl())));
                if (types.Contains(typeof(ManagerServer.Model.JournalEntry))) cells.Add(Make(journalEntries, new Link(new InventoryQuantitySummaryTransactions { Business = Business, InventoryItem = e.Key, From = report.FromDate, To = report.ToDate, JournalEntries = true }.ToUrl())));
                cells.Add(Make(closingBalance));

                model.Rows.Items.Add(new Row { Name = e.NameWithCode, Cells = cells });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
