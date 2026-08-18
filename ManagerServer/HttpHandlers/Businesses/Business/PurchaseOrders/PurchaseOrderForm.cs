using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseOrders
{
    [ProtoContract]
    [Title(nameof(Strings.PurchaseOrder), nameof(Strings.Edit))]
    [Guide("A `PurchaseOrder` is an official document sent to suppliers that authorizes them to deliver specific goods or services at agreed prices and terms.")]
    [Guide("Purchase orders formalize your commitment to purchase and create a legal obligation between you and your supplier.")]
    [Header("Purpose and Benefits")]
    [Guide("Purchase orders serve multiple critical functions in your procurement process:")]
    [Guide("• **Authorization** - They provide formal approval for purchases and establish spending controls")]
    [Guide("• **Legal protection** - They create a binding agreement with clear terms and conditions")]
    [Guide("• **Budget control** - They help track committed expenses against available budgets")]
    [Guide("• **Inventory management** - They ensure timely ordering of materials to maintain optimal stock levels")]
    [Guide("• **Invoice matching** - They provide a reference for verifying supplier invoices against agreed prices and quantities")]
    [Header("Creating Purchase Orders")]
    [Guide("When creating a purchase order, pay careful attention to:")]
    [Guide("• **Supplier details** - Ensure the correct supplier is selected")]
    [Guide("• **Item specifications** - Include accurate descriptions, quantities, and unit prices")]
    [Guide("• **Delivery information** - Specify delivery dates, locations, and shipping instructions")]
    [Guide("• **Payment terms** - Define when and how payment will be made")]
    [Guide("• **Special conditions** - Add any specific requirements or terms in the notes section")]
    [Header("Workflow Integration")]
    [Guide("Purchase orders integrate seamlessly with other documents in your procurement workflow:")]
    [Guide("• **From quotes** - Convert approved `PurchaseQuotes` directly into purchase orders")]
    [Guide("• **To receipts** - Record deliveries through `GoodsReceipts` that reference the purchase order")]
    [Guide("• **To invoices** - Match `PurchaseInvoices` against purchase orders to verify pricing and quantities")]
    [Guide("The system automatically tracks outstanding quantities and amounts, helping you monitor supplier performance and ensure complete fulfillment of orders.")]
    [Header("Form Fields")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.PurchaseOrder))]
    internal sealed class PurchaseOrderForm : NakedVueForm<ManagerServer.Model.PurchaseOrder>
    {
        [ProtoMember(1)] public Guid? Supplier;

        protected override bool CanHaveImage() => true;

        protected override void OnSource(PurchaseOrder form, ManagerServer.Model.Object source)
        {
            if (!Key.HasValue)
            {
                if (Supplier.HasValue) form.Supplier = Supplier;
            }

            if (source is PurchaseQuote purchaseQuote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseOrder>(Business, purchaseQuote.CustomFields);
            }
            if (source is PurchaseOrder purchaseOrder)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseOrder>(Business, purchaseOrder.CustomFields);
            }
            if (source is PurchaseInvoice purchaseInvoice)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseOrder>(Business, purchaseInvoice.CustomFields);
            }
            if (source is SalesQuote salesQuote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseOrder>(Business, salesQuote.CustomFields);
            }
            if (source is SalesOrder salesOrder)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseOrder>(Business, salesOrder.CustomFields);
            }
            if (source is SalesInvoice salesInvoice)
            {
                Copy(salesInvoice, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseOrder>(Business, salesInvoice.CustomFields);
            }
            if (source is DebitNote debitNote)
            {
                Copy(debitNote, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseOrder>(Business, debitNote.CustomFields);
            }
            if (source is CreditNote creditNote)
            {
                Copy(creditNote, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseOrder>(Business, creditNote.CustomFields);
            }
            if (source is DeliveryNote deliveryNote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseOrder>(Business, deliveryNote.CustomFields);
            }
            if (source is GoodsReceipt goodsReceipt)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseOrder>(Business, goodsReceipt.CustomFields);
            }
            if (source is InventoryTransfer inventoryTransfer)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<PurchaseOrder>(Business, inventoryTransfer.CustomFields);
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