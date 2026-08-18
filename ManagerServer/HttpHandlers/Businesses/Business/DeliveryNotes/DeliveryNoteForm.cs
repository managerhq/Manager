using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.DeliveryNotes
{
    [ProtoContract]
    [Title(nameof(Strings.DeliveryNote), nameof(Strings.Edit))]
    [Guide("The `DeliveryNote` form allows you to record shipments of goods to customers, providing formal documentation of what was delivered, when, and to whom.")]
    [Guide("Delivery notes are essential logistics documents that accompany shipped goods, serving as proof of delivery and helping track inventory movements. They update inventory quantities without creating financial transactions, allowing you to separate the physical delivery process from invoicing. This is particularly useful when goods are delivered before or after invoicing occurs.")]
    [Guide("When creating a delivery note, link it to the relevant `SalesOrder` and/or `SalesInvoice` to maintain proper documentation trails. Include accurate quantities, delivery addresses, and any special handling instructions. The delivery date should reflect when goods actually left your premises or were received by the customer. Delivery notes help resolve disputes about what was shipped and ensure customers receive exactly what they ordered.")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.DeliveryNote))]
    internal sealed class DeliveryNoteForm : NakedVueForm<ManagerServer.Model.DeliveryNote>
    {
        protected override bool CanHaveImage() => true;

        protected override void OnSource(DeliveryNote form, ManagerServer.Model.Object source)
        {
            if (source is PurchaseQuote purchaseQuote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DeliveryNote>(Business, purchaseQuote.CustomFields);
            }
            if (source is PurchaseOrder purchaseOrder)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DeliveryNote>(Business, purchaseOrder.CustomFields);
            }
            if (source is PurchaseInvoice purchaseInvoice)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DeliveryNote>(Business, purchaseInvoice.CustomFields);
            }
            if (source is SalesQuote salesQuote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DeliveryNote>(Business, salesQuote.CustomFields);
            }
            if (source is SalesOrder salesOrder)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DeliveryNote>(Business, salesOrder.CustomFields);
            }
            if (source is SalesInvoice salesInvoice)
            {
                Copy(salesInvoice, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DeliveryNote>(Business, salesInvoice.CustomFields);
            }
            if (source is DebitNote debitNote)
            {
                Copy(debitNote, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DeliveryNote>(Business, debitNote.CustomFields);
            }
            if (source is CreditNote creditNote)
            {
                Copy(creditNote, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DeliveryNote>(Business, creditNote.CustomFields);
            }
            if (source is DeliveryNote deliveryNote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DeliveryNote>(Business, deliveryNote.CustomFields);
            }
            if (source is GoodsReceipt goodsReceipt)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DeliveryNote>(Business, goodsReceipt.CustomFields);
            }
        }
    }
}