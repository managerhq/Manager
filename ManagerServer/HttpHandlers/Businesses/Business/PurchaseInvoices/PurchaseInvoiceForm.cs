using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.PurchaseInvoice), nameof(Strings.Edit))]
    [Guide("A `PurchaseInvoice` records a bill received from a supplier for goods or services provided to your business.")]
    [Guide("Purchase invoices are essential for tracking what you owe to suppliers and managing your accounts payable.")]
    [Header("Creating a Purchase Invoice")]
    [Guide("To create a purchase invoice, enter the details from the invoice you received from your supplier.")]
    [Guide("The form contains the following fields:")]
    [Fields(typeof(PurchaseInvoice))]
    internal sealed class PurchaseInvoiceForm : NakedVueForm<ManagerServer.Model.PurchaseInvoice>
    {
        [ProtoMember(1)] public Guid? Supplier;

        protected override bool CanHaveImage() => true;

        protected override void OnSource(ManagerServer.Model.PurchaseInvoice form, ManagerServer.Model.Object source)
        {
            if (!Key.HasValue)
            {
                if (Supplier.HasValue) form.Supplier = Supplier;

                if (source is PurchaseQuote purchaseQuote)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseInvoice>(Business, purchaseQuote.CustomFields);
                }
                if (source is PurchaseOrder purchaseOrder)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseInvoice>(Business, purchaseOrder.CustomFields);
                }
                if (source is PurchaseInvoice purchaseInvoice)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseInvoice>(Business, purchaseInvoice.CustomFields);
                }
                if (source is SalesQuote salesQuote)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseInvoice>(Business, salesQuote.CustomFields);
                }
                if (source is SalesOrder salesOrder)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseInvoice>(Business, salesOrder.CustomFields);
                }
                if (source is SalesInvoice salesInvoice)
                {
                    Copy(salesInvoice, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseInvoice>(Business, salesInvoice.CustomFields);
                }
                if (source is DebitNote debitNote)
                {
                    Copy(debitNote, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseInvoice>(Business, debitNote.CustomFields);
                }
                if (source is CreditNote creditNote)
                {
                    Copy(creditNote, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseInvoice>(Business, creditNote.CustomFields);
                }
                if (source is DeliveryNote deliveryNote)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseInvoice>(Business, deliveryNote.CustomFields);
                }
                if (source is GoodsReceipt goodsReceipt)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseInvoice>(Business, goodsReceipt.CustomFields);
                }
                if (source is InventoryTransfer inventoryTransfer)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseInvoice>(Business, inventoryTransfer.CustomFields);
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