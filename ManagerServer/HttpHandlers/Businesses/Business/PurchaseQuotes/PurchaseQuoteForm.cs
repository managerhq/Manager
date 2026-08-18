using ManagerServer.Model;
using System.Linq;
using System.Collections.Generic;
using System;
using ProtoBuf;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseQuotes
{
    [ProtoContract]
    [Title(nameof(Strings.PurchaseQuote), nameof(Strings.Edit))]
    [Guide("The `PurchaseQuote` form allows you to record and manage quotations received from suppliers, helping you compare prices and terms before making purchasing decisions.")]
    [Guide("Purchase quotes are essential for procurement management, enabling you to evaluate multiple supplier offers, negotiate better terms, and maintain a documented history of pricing proposals. You can easily convert accepted quotes into `PurchaseOrders` or `PurchaseInvoices` when you're ready to proceed with the purchase.")]
    [Guide("When entering a purchase quote, include all relevant details such as item descriptions, quantities, unit prices, and any special terms or conditions. You can mark quotes as 'Request for Quotation' if you're soliciting prices from suppliers. The status tracking helps you identify which quotes have been accepted and converted to actual purchases.")]
    [Fields(typeof(ManagerServer.Model.PurchaseQuote))]
    internal sealed class PurchaseQuoteForm : NakedVueForm<ManagerServer.Model.PurchaseQuote>
    {
        [ProtoMember(1)] public Guid? Supplier;

        protected override bool CanHaveImage() => true;

        protected override void OnSource(PurchaseQuote form, ManagerServer.Model.Object source)
        {
            if (!Key.HasValue)
            {
                if (Supplier.HasValue) form.Supplier = Supplier;
            }

            if (source is PurchaseQuote purchaseQuote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseQuote>(Business, purchaseQuote.CustomFields);
            }
            if (source is PurchaseOrder purchaseOrder)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseQuote>(Business, purchaseOrder.CustomFields);
            }
            if (source is PurchaseInvoice purchaseInvoice)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseQuote>(Business, purchaseInvoice.CustomFields);
            }
            if (source is SalesQuote salesQuote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseQuote>(Business, salesQuote.CustomFields);
            }
            if (source is SalesOrder salesOrder)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseQuote>(Business, salesOrder.CustomFields);
            }
            if (source is SalesInvoice salesInvoice)
            {
                Copy(salesInvoice, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseQuote>(Business, salesInvoice.CustomFields);
            }
            if (source is DebitNote debitNote)
            {
                Copy(debitNote, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseQuote>(Business, debitNote.CustomFields);
            }
            if (source is CreditNote creditNote)
            {
                Copy(creditNote, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseQuote>(Business, creditNote.CustomFields);
            }
            if (source is DeliveryNote deliveryNote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseQuote>(Business, deliveryNote.CustomFields);
            }
            if (source is GoodsReceipt goodsReceipt)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseQuote>(Business, goodsReceipt.CustomFields);
            }
            if (source is InventoryTransfer inventoryTransfer)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseQuote>(Business, inventoryTransfer.CustomFields);
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