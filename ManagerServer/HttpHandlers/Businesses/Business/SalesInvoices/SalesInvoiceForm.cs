using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Model;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoice), nameof(Strings.Edit))]
    [Guide("This screen is for creating sales invoices.")]
    [Guide("It contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.SalesInvoice))]
    internal sealed class SalesInvoiceForm : NakedVueForm<ManagerServer.Model.SalesInvoice>
    {
        [ProtoMember(1)] public Guid? Customer;

        protected override bool CanHaveImage() => true;

        protected override void OnSource(SalesInvoice form, ManagerServer.Model.Object source)
        {
            if (!Key.HasValue)
            {
                var customer2 = ApplicationData.Businesses.Get(Business).SingleOrDefault<Customer>(Customer);
                if (customer2 != null)
                {
                    form.Customer = customer2.Key;
                    form.BillingAddress = customer2.BillingAddress;
                }

                if (source is PurchaseQuote purchaseQuote)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesInvoice>(Business, purchaseQuote.CustomFields);
                }
                if (source is PurchaseOrder purchaseOrder)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesInvoice>(Business, purchaseOrder.CustomFields);
                }
                if (source is PurchaseInvoice purchaseInvoice)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesInvoice>(Business, purchaseInvoice.CustomFields);
                }
                if (source is SalesQuote salesQuote)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesInvoice>(Business, salesQuote.CustomFields);
                }
                if (source is SalesOrder salesOrder)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesInvoice>(Business, salesOrder.CustomFields);
                }
                if (source is SalesInvoice salesInvoice)
                {
                    Copy(salesInvoice, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesInvoice>(Business, salesInvoice.CustomFields);
                }
                if (source is DebitNote debitNote)
                {
                    Copy(debitNote, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesInvoice>(Business, debitNote.CustomFields);
                }
                if (source is CreditNote creditNote)
                {
                    Copy(creditNote, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesInvoice>(Business, creditNote.CustomFields);
                }
                if (source is DeliveryNote deliveryNote)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesInvoice>(Business, deliveryNote.CustomFields);
                }
                if (source is GoodsReceipt goodsReceipt)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesInvoice>(Business, goodsReceipt.CustomFields);
                }
                if (source is InventoryTransfer inventoryTransfer)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<SalesInvoice>(Business, inventoryTransfer.CustomFields);
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

                if (source is Customer customer)
                {
                    form.Customer = customer.Key;
                    var lines = new List<SalesInvoice.Line>();
                    foreach (var e in new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsInventoryOnHand && x.Qty.HasValue && x.Customer?.Key == Source.Value && x.InventoryItem != null).GroupBy(x => x.InventoryItem).Select(x => new { Item = x.Key, Qty = x.Sum(y => y.Qty.Value) }).Where(x => x.Qty > 0m).ToArray())
                    {
                        lines.Add(new SalesInvoice.Line() { Item = e.Item.Key, Qty = e.Qty, SalesUnitPrice = ((InventoryItem)e.Item).DefaultSalesUnitPrice, TaxCode = ((InventoryItem)e.Item).DefaultTaxCode });
                    }
                    form.Lines = lines.ToArray();
                }
            }
        }
    }
}