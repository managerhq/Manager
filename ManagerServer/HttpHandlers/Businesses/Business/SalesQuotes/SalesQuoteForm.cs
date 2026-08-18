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

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesQuotes
{
    [ProtoContract]
    [Title(nameof(Strings.SalesQuote), nameof(Strings.Edit))]
    [Guide("The `Sales Quote` form allows you to create professional quotations for potential customers, presenting your products or services with pricing and terms before they commit to a purchase.")]
    [Guide("Sales quotes are essential business documents that help you communicate your offerings clearly, establish pricing expectations, and provide a foundation for future sales orders or invoices.")]
    [Header("Purpose and Benefits")]
    [Guide("Sales quotes serve as non-binding proposals that can be revised based on customer feedback. They help you:")]
    [Guide("• Present a professional image to potential customers")]
    [Guide("• Clearly communicate pricing and terms")]
    [Guide("• Create a paper trail for negotiations")]
    [Guide("• Provide a foundation for converting accepted quotes into `Sales Orders` or `Sales Invoices`")]
    [Header("Creating a Sales Quote")]
    [Guide("When creating a sales quote, ensure all details are accurate and complete. Key features include:")]
    [Guide("• Link quotes to specific customers to automatically populate their details")]
    [Guide("• Set expiry dates to create urgency and manage quote validity")]
    [Guide("• Include detailed line items with quantities, unit prices, and applicable tax codes")]
    [Guide("• Add custom fields to capture additional information specific to your business needs")]
    [Guide("Once a customer accepts your quote, you can easily convert it to a `Sales Order` or `Sales Invoice` without re-entering data.")]
    [Header("Form Fields")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.SalesQuote))]
    internal sealed class SalesQuoteForm : NakedVueForm<ManagerServer.Model.SalesQuote>
    {
        [ProtoMember(1)] public Guid? Customer;

        protected override bool CanHaveImage() => true;

        protected override void OnSource(SalesQuote form, ManagerServer.Model.Object source)
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
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesQuote>(Business, purchaseQuote.CustomFields);
            }
            if (source is PurchaseOrder purchaseOrder)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesQuote>(Business, purchaseOrder.CustomFields);
            }
            if (source is PurchaseInvoice purchaseInvoice)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesQuote>(Business, purchaseInvoice.CustomFields);
            }
            if (source is SalesQuote salesQuote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesQuote>(Business, salesQuote.CustomFields);
            }
            if (source is SalesOrder salesOrder)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesQuote>(Business, salesOrder.CustomFields);
            }
            if (source is SalesInvoice salesInvoice)
            {
                Copy(salesInvoice, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesQuote>(Business, salesInvoice.CustomFields);
            }
            if (source is DebitNote debitNote)
            {
                Copy(debitNote, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesQuote>(Business, debitNote.CustomFields);
            }
            if (source is CreditNote creditNote)
            {
                Copy(creditNote, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesQuote>(Business, creditNote.CustomFields);
            }
            if (source is DeliveryNote deliveryNote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesQuote>(Business, deliveryNote.CustomFields);
            }
            if (source is GoodsReceipt goodsReceipt)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesQuote>(Business, goodsReceipt.CustomFields);
            }
            if (source is InventoryTransfer inventoryTransfer)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesQuote>(Business, inventoryTransfer.CustomFields);
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

            if (source is InventoryPriceList inventoryPriceList)
            {
                var lines = new List<SalesQuote.Line>();
                foreach (var e in ManagerServer.Api.Businesses.Business.Reports.InventoryPriceList.GetInventoryPriceListView.GetInventoryItems(Business, inventoryPriceList))
                {
                    lines.Add(new SalesQuote.Line()
                    {
                        Item = e.Key,
                        LineDescription = e.DefaultLineDescription,
                        SalesUnitPrice = e.DefaultSalesUnitPrice,
                        TaxCode = e.DefaultTaxCode
                    });
                }
                form.HideTotalAmount = true;
                form.Lines = lines.ToArray();
            }
        }
    }
}