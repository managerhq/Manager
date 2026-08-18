using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query;
using ManagerServer.Helpers;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.GoodsReceipts
{
    [ProtoContract]
    [Title(nameof(Strings.GoodsReceipt), nameof(Strings.Edit))]
    [Guide("A `Goods Receipt` records the arrival of goods from suppliers, updating your inventory quantities immediately upon receipt.")]
    [Guide("Use this form when you physically receive goods, regardless of whether you have received the supplier's invoice.")]
    [Header("Purpose")]
    [Guide("Goods receipts are essential for accurate inventory management. They allow you to record the physical receipt of goods separately from the financial transaction.")]
    [Guide("This separation is important when goods arrive before or after the supplier's invoice. The goods receipt updates inventory quantities and values without creating accounts payable entries, ensuring your inventory levels are always current.")]
    [Header("Recording a Goods Receipt")]
    [Guide("When recording a goods receipt, link it to the relevant `Purchase Order` and/or `Purchase Invoice` to maintain proper documentation.")]
    [Guide("Always verify quantities received against packing slips, check for damage or discrepancies, and record the actual receipt date.")]
    [Guide("This information is vital for inventory accuracy, supplier performance tracking, and resolving any disputes about deliveries.")]
    [Guide("The system automatically tracks whether purchase orders have been fully or partially received based on your goods receipts.")]
    [Header("Form Fields")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.GoodsReceipt))]
    internal sealed class GoodsReceiptForm : NakedVueForm<ManagerServer.Model.GoodsReceipt>
    {
        protected override bool CanHaveImage() => true;

        protected override void OnSource(GoodsReceipt form, ManagerServer.Model.Object source)
        {
            if (source is PurchaseQuote purchaseQuote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<GoodsReceipt>(Business, purchaseQuote.CustomFields);
            }
            if (source is PurchaseOrder purchaseOrder)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<GoodsReceipt>(Business, purchaseOrder.CustomFields);
            }
            if (source is PurchaseInvoice purchaseInvoice)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<GoodsReceipt>(Business, purchaseInvoice.CustomFields);
            }
            if (source is SalesQuote salesQuote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<GoodsReceipt>(Business, salesQuote.CustomFields);
            }
            if (source is SalesOrder salesOrder)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<GoodsReceipt>(Business, salesOrder.CustomFields);
            }
            if (source is SalesInvoice salesInvoice)
            {
                Copy(salesInvoice, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<GoodsReceipt>(Business, salesInvoice.CustomFields);
            }
            if (source is DebitNote debitNote)
            {
                Copy(debitNote, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<GoodsReceipt>(Business, debitNote.CustomFields);
            }
            if (source is CreditNote creditNote)
            {
                Copy(creditNote, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<GoodsReceipt>(Business, creditNote.CustomFields);
            }
            if (source is DeliveryNote deliveryNote)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<GoodsReceipt>(Business, deliveryNote.CustomFields);
            }
            if (source is GoodsReceipt goodsReceipt)
            {
                Copy(source, form);
                form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<GoodsReceipt>(Business, goodsReceipt.CustomFields);
            }
        }
    }
}