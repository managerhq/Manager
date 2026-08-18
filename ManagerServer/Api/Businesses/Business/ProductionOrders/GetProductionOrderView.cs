using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.ProductionOrders
{
    [ProtoContract]
    internal sealed class GetProductionOrderView : GetTransactionView<Model.ProductionOrder>
    {
        protected override TransactionView GetViewData(Model.ProductionOrder o)
        {
            var inventoryItems = Database.OfType<Model.InventoryItem>().ToDictionary(x => x.Key);

            var viewData = new TransactionView();
            viewData.title = Strings.ProductionOrder;

            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = o.Date.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            viewData.description = o.Description;

            var lines = new Query.GeneralLedger.GeneralLedger(Business).Where(x => x.Transaction == o).ToArray();

            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Qty, align = "center", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Item });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Amount, align = "right", nowrap = true });

            var totalAmount = 0m;

            var baseCurrency = Database.Single<Model.BaseCurrency>();
            foreach (var e in lines.Where(x => x.InventoryItem != null && x.InventoryItem.Key != o.FinishedInventoryItem && x.IsCostOfGoodsSold && x.GeneralLedgerAccount.IsInventoryOnHand).GroupBy(x => x.InventoryItem))
            {
                var qty = e.Sum(x => x.Qty ?? 0m) * -1m;

                var row = new TransactionView.Row();
                row.cells.Add(new TransactionView.Cell { text = qty.ToNumberString() });
                row.cells.Add(new TransactionView.Cell { text = e.Key.NameWithCode });
                row.cells.Add(new TransactionView.Cell { text = (e.Sum(x => x.BaseAmount) * -1m).ToCurrencyString(baseCurrency, CurrencySymbol.None) });
                viewData.table.rows.Add(row);

                totalAmount += e.Sum(x => x.BaseAmount) * -1m;
            }

            var total = o.Qty.ToNumberString();
            total += " \u00D7 ";
            if (o.FinishedInventoryItem.HasValue && inventoryItems.ContainsKey(o.FinishedInventoryItem.Value)) total += inventoryItems[o.FinishedInventoryItem.Value].ItemCode + " " + inventoryItems[o.FinishedInventoryItem.Value].ItemName;

            foreach (var e in lines.Where(x => x.InventoryItem == null))
            {
                viewData.table.totals.Add(new TransactionView.Total { label = e.GeneralLedgerAccount.GetName(), text = (e.BaseAmount * -1m).ToCurrencyString(baseCurrency, CurrencySymbol.Short) });
                totalAmount += e.BaseAmount * -1m;
            }

            viewData.table.totals.Add(new TransactionView.Total { label = total, emphasis = true, text = totalAmount.ToCurrencyString(baseCurrency, CurrencySymbol.Short) });

            return viewData;
        }
    }
}
