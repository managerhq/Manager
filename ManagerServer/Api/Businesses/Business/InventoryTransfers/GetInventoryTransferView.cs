using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.InventoryTransfers
{
    [ProtoContract]
    internal sealed class GetInventoryTransferView : GetTransactionView<Model.InventoryTransfer>
    {
        protected override TransactionView GetViewData(Model.InventoryTransfer o)
        {
            var lineCustomFields = Database.GetCustomFields(typeof(Model.InventoryTransfer.Line)).Where(x => x.DisplayOnView).OrderBy(x => x.Position).ToArray();

            var inventoryItemsWithCode = Database.OfType<Model.InventoryItem>().Where(x => !string.IsNullOrWhiteSpace(x.ItemCode)).ToDictionary(x => x.Key);
            var nonInventoryItemsWithCode = Database.OfType<Model.NonInventoryItem>().Where(x => !string.IsNullOrWhiteSpace(x.Code)).ToDictionary(x => x.Key);

            var inventoryItems = Database.OfType<Model.InventoryItem>().ToDictionary(x => x.Key);
            var nonInventoryItems = Database.OfType<Model.NonInventoryItem>().ToDictionary(x => x.Key);

            var viewData = new TransactionView();
            viewData.title = Strings.InventoryTransfer;
            viewData.reference = o.Reference;
            viewData.description = o.Description;

            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = o.Date.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            if (o.Lines == null) return viewData;

            var code = o.Lines.Any(x => x.Item.HasValue && (inventoryItemsWithCode.ContainsKey(x.Item.Value) || nonInventoryItemsWithCode.ContainsKey(x.Item.Value)));
            var qty = o.Lines.Any(x => x.Qty != 0m);

            if (code) viewData.table.columns.Add(new TransactionView.Column { label = Strings.Code, nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Description });
            foreach (var e in lineCustomFields)
            {
                string align = null;
                var nowrap = false;
                if (e is Model.NumberCustomField) { align = "right"; nowrap = true; }
                if (e is Model.DateCustomField) { align = "center"; nowrap = true; }
                if (e is Model.CheckboxCustomField) align = "center";
                viewData.table.columns.Add(new TransactionView.Column { label = e.Name, align = align, nowrap = nowrap });
            }
            if (qty) viewData.table.columns.Add(new TransactionView.Column { label = Strings.Qty, align = "center", nowrap = true });

            foreach (var e in o.Lines)
            {
                var row = new TransactionView.Row();
                if (code)
                {
                    var value = string.Empty;
                    if (e.Item.HasValue)
                    {
                        if (inventoryItemsWithCode.ContainsKey(e.Item.Value)) value = inventoryItemsWithCode[e.Item.Value].ItemCode;
                        else if (nonInventoryItemsWithCode.ContainsKey(e.Item.Value)) value = nonInventoryItemsWithCode[e.Item.Value].Code;
                    }
                    row.cells.Add(new TransactionView.Cell { text = value });
                }

                var description = e.LineDescription;
                if (string.IsNullOrWhiteSpace(description) && e.Item.HasValue)
                {
                    if (inventoryItems.ContainsKey(e.Item.Value)) description = inventoryItems[e.Item.Value].ItemName;
                    else if (nonInventoryItems.ContainsKey(e.Item.Value)) description = nonInventoryItems[e.Item.Value].Name;
                }

                row.cells.Add(new TransactionView.Cell { text = description });
                foreach (var e2 in lineCustomFields)
                {
                    var value = e.CustomFields2?.GetValue(e2);
                    string text = null;
                    if (value is decimal d) text = d.ToNumberString();
                    if (value is DateTime dateTime) text = dateTime.ToShortDateString();
                    if (value is string s) text = s;
                    if (value is bool b && b) text = Strings.Yes;
                    if (value is string[] stringArray && stringArray.Length > 0) text = string.Join(", ", stringArray.Where(x => !string.IsNullOrWhiteSpace(x)));
                    row.cells.Add(new TransactionView.Cell { text = text, value = value });
                }
                if (qty) row.cells.Add(new TransactionView.Cell { text = e.Qty.ToNumberString() });
                viewData.table.rows.Add(row);
            }

            var inventoryLocations = Database.OfType<Model.CustomInventoryLocation>().ToDictionary(x => x.Key);

            if (o.InventoryLocation.HasValue && inventoryLocations.ContainsKey(o.InventoryLocation.Value))
            {
                viewData.custom_fields.Add(new TransactionView.CustomField { label = Strings.From, text = inventoryLocations[o.InventoryLocation.Value].Name });
            }

            if (o.ToInventoryLocation.HasValue && inventoryLocations.ContainsKey(o.ToInventoryLocation.Value))
            {
                viewData.custom_fields.Add(new TransactionView.CustomField { label = Strings.To, text = inventoryLocations[o.ToInventoryLocation.Value].Name });
            }

            return viewData;
        }
    }
}
