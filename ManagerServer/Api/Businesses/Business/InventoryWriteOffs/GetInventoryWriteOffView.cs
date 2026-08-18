using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.Api.Businesses.Business.InventoryWriteOffs
{
    [ProtoContract]
    internal sealed class GetInventoryWriteOffView : GetTransactionView<Model.InventoryWriteOff>
    {
        protected override TransactionView GetViewData(Model.InventoryWriteOff o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.InventoryWriteOff;

            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = o.Date.ToLocalShortDisplayString() });
            viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            viewData.description = o.Description;

            viewData.table.columns.Add(new TransactionView.Column { label = Strings.ItemCode });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.ItemName });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Qty, align = "center", nowrap = true });

            if (o.Items != null)
            {
                foreach (var e in o.Items)
                {
                    var inventoryItem = Database.SingleOrDefault<Model.InventoryItem>(e.InventoryItem);

                    var row = new TransactionView.Row();
                    row.cells.Add(new TransactionView.Cell { text = inventoryItem?.ItemCode, canBeHidden = true });
                    row.cells.Add(new TransactionView.Cell { text = inventoryItem?.ItemName });
                    row.cells.Add(new TransactionView.Cell { text = (e.Qty.ToNumberString() + " " + inventoryItem?.UnitName).Trim() });
                    viewData.table.rows.Add(row);
                }
            }

            return viewData;
        }
    }
}
