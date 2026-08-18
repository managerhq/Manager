using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.DebitNotes
{
    [ProtoContract]
    [Title(nameof(Strings.DebitNote), nameof(Strings.Edit))]
    [Guide("The `Debit Note` form enables you to record claims against suppliers for returns, allowances, or corrections to previously received purchase invoices.")]
    [Guide("Debit notes are formal documents that reduce the amount you owe to suppliers. They serve as the opposite of credit notes - while credit notes reduce what customers owe you, debit notes reduce what you owe suppliers.")]
    [Header("When to Use Debit Notes")]
    [Guide("Issue debit notes when returning goods to suppliers, claiming damages, receiving pricing adjustments, or correcting errors in supplier invoices. The debit note creates a negative balance in the supplier's account that can be offset against future purchases or refunded.")]
    [Header("Creating a Debit Note")]
    [Guide("When creating a debit note, specify the supplier and clearly state the reason for the debit. Include details of returned items with quantities and values, or describe the nature of any pricing disputes or quality issues.")]
    [Guide("Reference the original purchase invoice when applicable. The system will automatically adjust inventory levels if physical goods are being returned to the supplier. Multiple line items can be included with appropriate expense accounts and tax codes.")]
    [Header("Form Fields")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.DebitNote))]
    internal sealed class DebitNoteForm : NakedVueForm<ManagerServer.Model.DebitNote>
    {
        [ProtoMember(1)] public Guid? Supplier;

        protected override bool CanHaveImage() => true;

        protected override void OnSource(DebitNote form, ManagerServer.Model.Object source)
        {
            if (!Key.HasValue)
            {
                if (Supplier.HasValue) form.Supplier = Supplier;

                if (source is PurchaseQuote purchaseQuote)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DebitNote>(Business, purchaseQuote.CustomFields);
                }
                if (source is PurchaseOrder purchaseOrder)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DebitNote>(Business, purchaseOrder.CustomFields);
                }
                if (source is PurchaseInvoice purchaseInvoice)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DebitNote>(Business, purchaseInvoice.CustomFields);
                }
                if (source is SalesQuote salesQuote)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DebitNote>(Business, salesQuote.CustomFields);
                }
                if (source is SalesOrder salesOrder)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DebitNote>(Business, salesOrder.CustomFields);
                }
                if (source is SalesInvoice salesInvoice)
                {
                    Copy(salesInvoice, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DebitNote>(Business, salesInvoice.CustomFields);
                }
                if (source is DebitNote debitNote)
                {
                    Copy(debitNote, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DebitNote>(Business, debitNote.CustomFields);
                }
                if (source is CreditNote creditNote)
                {
                    Copy(creditNote, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DebitNote>(Business, creditNote.CustomFields);
                }
                if (source is DeliveryNote deliveryNote)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DebitNote>(Business, deliveryNote.CustomFields);
                }
                if (source is GoodsReceipt goodsReceipt)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DebitNote>(Business, goodsReceipt.CustomFields);
                }
                if (source is InventoryTransfer inventoryTransfer)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<DebitNote>(Business, inventoryTransfer.CustomFields);
                }

                if (source is SalesQuote || source is SalesOrder || source is SalesInvoice || source is CreditNote || source is InventoryTransfer)
                {
                    if (form.Lines != null)
                    {
                        var prices = new Dictionary<Guid, decimal>();
                        foreach (var e in ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.InventoryItem>().Where(x => x.DefaultPurchaseUnitPrice != 0m)) prices.Add(e.Key, e.DefaultPurchaseUnitPrice);
                        foreach (var e in ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.NonInventoryItem>().Where(x => x.DefaultPurchaseUnitPrice != 0m)) prices.Add(e.Key, e.DefaultPurchaseUnitPrice);

                        foreach (var e in form.Lines)
                        {
                            e.DiscountAmount = 0m;
                            e.DiscountPercentage = 0m;
                            if (e.Item.HasValue && prices.ContainsKey(e.Item.Value)) e.PurchaseUnitPrice = prices[e.Item.Value];
                        }
                    }
                }
            }
        }
    }
}