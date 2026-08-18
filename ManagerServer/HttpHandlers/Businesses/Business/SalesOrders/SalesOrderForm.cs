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

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesOrders
{
    [ProtoContract]
    [Title(nameof(Strings.SalesOrder), nameof(Strings.Edit))]
    [Guide("The `Sales Order` form enables you to record confirmed orders from customers, creating a formal commitment to deliver specific goods or services at agreed prices and terms.")]
    [Guide("Sales orders serve as binding agreements between you and your customers, helping track what needs to be delivered and invoiced. They provide crucial information for inventory management, production planning, and revenue forecasting. Each sales order can be linked to the original `Sales Quote` if one was issued.")]
    [Guide("When entering a sales order, pay careful attention to delivery dates, quantities, and special customer requirements. The system will track the fulfillment status, showing whether items have been delivered through `Delivery Notes` and invoiced through `Sales Invoices`. This ensures complete visibility of your order-to-cash process.")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.SalesOrder))]
    internal sealed class SalesOrderForm : NakedVueForm<ManagerServer.Model.SalesOrder>
    {
        [ProtoMember(1)] public Guid? Customer;

        protected override bool CanHaveImage() => true;

        protected override void OnSource(SalesOrder form, ManagerServer.Model.Object source)
        {
            if (!Key.HasValue)
            {
                var customer2 = ApplicationData.Businesses.Get(Business).SingleOrDefault<Customer>(Customer);
                if (customer2 != null)
                {
                    form.Customer = customer2.Key;
                    form.BillingAddress = customer2.BillingAddress;
                }
            }

            if (source is PurchaseQuote purchaseQuote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesOrder>(Business, purchaseQuote.CustomFields);
            }
            if (source is PurchaseOrder purchaseOrder)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesOrder>(Business, purchaseOrder.CustomFields);
            }
            if (source is PurchaseInvoice purchaseInvoice)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesOrder>(Business, purchaseInvoice.CustomFields);
            }
            if (source is SalesQuote salesQuote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesOrder>(Business, salesQuote.CustomFields);
            }
            if (source is SalesOrder salesOrder)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesOrder>(Business, salesOrder.CustomFields);
            }
            if (source is SalesInvoice salesInvoice)
            {
                Copy(salesInvoice, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesOrder>(Business, salesInvoice.CustomFields);
            }
            if (source is DebitNote debitNote)
            {
                Copy(debitNote, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesOrder>(Business, debitNote.CustomFields);
            }
            if (source is CreditNote creditNote)
            {
                Copy(creditNote, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesOrder>(Business, creditNote.CustomFields);
            }
            if (source is DeliveryNote deliveryNote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesOrder>(Business, deliveryNote.CustomFields);
            }
            if (source is GoodsReceipt goodsReceipt)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesOrder>(Business, goodsReceipt.CustomFields);
            }
            if (source is InventoryTransfer inventoryTransfer)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesOrder>(Business, inventoryTransfer.CustomFields);
            }

            if (source is PurchaseQuote || source is PurchaseOrder || source is PurchaseInvoice || source is DebitNote || source is InventoryTransfer)
            {
                if (form.Lines != null)
                {
                    var prices = new Dictionary<Guid, decimal>();
                    foreach (var e in ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.InventoryItem>().Where(x => x.DefaultSalesUnitPrice != 0m)) prices.Add(e.Key, e.DefaultSalesUnitPrice);
                    foreach (var e in ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.NonInventoryItem>().Where(x => x.DefaultSalesUnitPrice != 0m)) prices.Add(e.Key, e.DefaultSalesUnitPrice);

                    foreach (var e in form.Lines)
                    {
                        e.DiscountAmount = 0m;
                        e.DiscountPercentage = 0m;
                        if (e.Item.HasValue && prices.ContainsKey(e.Item.Value)) e.SalesUnitPrice = prices[e.Item.Value];
                    }
                }
            }
        }
    }
}