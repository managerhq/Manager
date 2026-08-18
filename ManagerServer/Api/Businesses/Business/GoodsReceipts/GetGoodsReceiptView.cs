using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.Api.Businesses.Business.GoodsReceipts
{
    [ProtoContract]
    internal sealed class GetGoodsReceiptView : GetTransactionView<Model.GoodsReceipt>
    {
        protected override TransactionView GetViewData(Model.GoodsReceipt o)
        {
            var purchaseOrder = Database.SingleOrDefault<Model.PurchaseOrder>(o.PurchaseOrder);
            var purchaseInvoice = Database.SingleOrDefault<Model.PurchaseInvoice>(o.PurchaseInvoice);

            var viewData = new TransactionView();
            viewData.title = Strings.GoodsReceipt;
            if (o.HasGoodsReceiptCustomTitle && !string.IsNullOrWhiteSpace(o.GoodsReceiptCustomTitle)) viewData.title = o.GoodsReceiptCustomTitle;
            viewData.reference = o.Reference;
            viewData.description = o.Description;

            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = o.Date.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            if (purchaseOrder != null) viewData.fields.Add(new TransactionView.Field { label = Strings.OrderNumber, text = purchaseOrder.GetName() });
            else if (!string.IsNullOrWhiteSpace(o.OrderNumber)) viewData.fields.Add(new TransactionView.Field { label = Strings.OrderNumber, text = o.OrderNumber });

            if (purchaseInvoice != null) viewData.fields.Add(new TransactionView.Field { label = Strings.InvoiceNumber, text = purchaseInvoice.GetName() });
            else if (!string.IsNullOrWhiteSpace(o.InvoiceNumber)) viewData.fields.Add(new TransactionView.Field { label = Strings.InvoiceNumber, text = o.InvoiceNumber });

            var inventoryLocation = Database.SingleOrDefault<Model.CustomInventoryLocation>(o.InventoryLocation);
            if (inventoryLocation != null) viewData.fields.Add(new TransactionView.Field { label = Strings.InventoryLocation, text = inventoryLocation.GetName() });

            if (o.Supplier.HasValue)
            {
                var supplier = Database.SingleOrDefault<Model.Supplier>(o.Supplier.Value);
                if (supplier != null)
                {
                    viewData.recipient.code = supplier.Code;
                    viewData.recipient.name = supplier.Name;
                    viewData.recipient.address = supplier.Address;
                    viewData.recipient.email = supplier.Email;

                    viewData.custom_fields.AddRange(GetCustomFields(typeof(Model.Supplier), supplier.CustomFields));
                    viewData.custom_fields.AddRange(GetCustomFields2(typeof(Model.Supplier), supplier.CustomFields2));
                }
            }

            viewData.table = BuildTable(o, showLineNumbers: o.HasLineNumber);

            return viewData;
        }
    }
}
