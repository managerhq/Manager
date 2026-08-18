using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.Api.Businesses.Business.DeliveryNotes
{
    [ProtoContract]
    internal sealed class GetDeliveryNoteView : GetTransactionView<Model.DeliveryNote>
    {
        protected override TransactionView GetViewData(Model.DeliveryNote o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.DeliveryNote;
            if (o.HasDeliveryNoteCustomTitle && !string.IsNullOrWhiteSpace(o.DeliveryNoteCustomTitle)) viewData.title = o.DeliveryNoteCustomTitle;
            viewData.reference = o.Reference;
            viewData.description = o.Description;

            viewData.fields.Add(new TransactionView.Field { label = Strings.DeliveryDate, text = o.DeliveryDate.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            var salesOrder = Database.SingleOrDefault<Model.SalesOrder>(o.SalesOrder);
            if (salesOrder != null) viewData.fields.Add(new TransactionView.Field { label = Strings.OrderNumber, text = salesOrder.GetName() });
            else if (!string.IsNullOrWhiteSpace(o.OrderNumber)) viewData.fields.Add(new TransactionView.Field { label = Strings.OrderNumber, text = o.OrderNumber });

            var salesInvoice = Database.SingleOrDefault<Model.SalesInvoice>(o.SalesInvoice);
            if (salesInvoice != null) viewData.fields.Add(new TransactionView.Field { label = Strings.InvoiceNumber, text = salesInvoice.GetName() });
            else if (!string.IsNullOrWhiteSpace(o.InvoiceNumber)) viewData.fields.Add(new TransactionView.Field { label = Strings.InvoiceNumber, text = o.InvoiceNumber });

            var inventoryLocation = Database.SingleOrDefault<Model.CustomInventoryLocation>(o.InventoryLocation);
            if (inventoryLocation != null) viewData.fields.Add(new TransactionView.Field { label = Strings.From, text = inventoryLocation.GetName() });

            viewData.table = BuildTable(o, showLineNumbers: o.HasLineNumber);

            if (!o.Customer.HasValue) return viewData;

            var customer = Database.SingleOrDefault<Model.Customer>(o.Customer.Value);
            if (customer == null) return viewData;

            viewData.recipient.code = customer.Code;
            viewData.recipient.name = customer.Name;
            viewData.recipient.address = o.DeliveryAddress;
            viewData.recipient.email = customer.Email;

            viewData.custom_fields.AddRange(GetCustomFields(typeof(Model.Customer), customer.CustomFields));
            viewData.custom_fields.AddRange(GetCustomFields2(typeof(Model.Customer), customer.CustomFields2));

            return viewData;
        }
    }
}
