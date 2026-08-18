using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Settings.InventoryKits
{
    [ProtoContract]
    internal sealed class GetInventoryKitView : GetTransactionView<Model.InventoryKit>
    {
        protected override TransactionView GetViewData(Model.InventoryKit o)
        {
            var inventoryItems = Database.OfType<Model.InventoryItem>().ToDictionary(x => x.Key);

            var viewData = new TransactionView();
            viewData.title = Strings.InventoryKit;

            viewData.recipient.name = (o.ItemCode + " " + o.ItemName).Trim();
            viewData.recipient.address = o.HasDefaultLineDescription ? o.DefaultLineDescription : null;

            if (!string.IsNullOrWhiteSpace(o.UnitName)) viewData.fields.Add(new TransactionView.Field { label = Strings.UnitName, text = o.UnitName });

            viewData.description = Strings.BillOfMaterials;
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Qty, align = "center", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.ItemCode, align = "center", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.ItemName });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Description });

            if (o.BillOfMaterials != null)
            {
                foreach (var e in o.BillOfMaterials)
                {
                    if (e.InventoryItem.HasValue && inventoryItems.ContainsKey(e.InventoryItem.Value))
                    {
                        var inventoryItem = inventoryItems[e.InventoryItem.Value];
                        var row = new TransactionView.Row();
                        row.cells.Add(new TransactionView.Cell { text = e.Qty.ToNumberString() + " " + inventoryItem.UnitName });
                        row.cells.Add(new TransactionView.Cell { text = inventoryItem.ItemCode });
                        row.cells.Add(new TransactionView.Cell { text = inventoryItem.ItemName });
                        row.cells.Add(new TransactionView.Cell { text = inventoryItem.HasDefaultLineDescription ? inventoryItem.DefaultLineDescription : null });
                        viewData.table.rows.Add(row);
                    }
                }
            }

            if (viewData.table.rows.All(x => string.IsNullOrWhiteSpace(x.cells[1].text) || x.cells[1].text.Equals("&nbsp;")))
            {
                viewData.table.columns.RemoveAt(1);
                foreach (var e in viewData.table.rows.ToArray()) e.cells.RemoveAt(1);
            }

            return viewData;
        }
    }
}
