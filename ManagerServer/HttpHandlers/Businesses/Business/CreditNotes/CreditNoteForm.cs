using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.CreditNotes
{
    [ProtoContract]
    [Title(nameof(Strings.CreditNote), nameof(Strings.Edit))]
    [Guide("Credit notes are financial documents that reduce the amount a customer owes you. They provide a formal record of refunds, returns, or adjustments to customer accounts.")]
    [Guide("Use this form to issue credits to customers when you need to reduce their outstanding balance or process refunds.")]
    [Header("When to Use Credit Notes")]
    [Guide("Issue credit notes in the following situations:")]
    [Guide("• Customer returns goods or cancels services")]
    [Guide("• You made pricing errors on the original invoice that need correction")]
    [Guide("• Customer received damaged, defective, or incorrect items")]
    [Guide("• You want to grant post-invoice discounts, rebates, or goodwill allowances")]
    [Guide("• Customer overpaid and you need to record the credit balance")]
    [Header("Creating Credit Notes")]
    [Guide("When creating a credit note, follow these steps:")]
    [Guide("• Select the customer from the dropdown list")]
    [Guide("• Reference the original sales invoice number if this credit relates to a specific invoice")]
    [Guide("• Enter the items and quantities being credited - use positive quantities")]
    [Guide("• Include a clear description explaining the reason for the credit")]
    [Guide("• Add any relevant notes or reference numbers")]
    [Header("Important Information")]
    [Guide("The system automatically handles the following when you save a credit note:")]
    [Guide("• Updates the customer's account balance to reduce what they owe")]
    [Guide("• Returns credited inventory items back to stock (if applicable)")]
    [Guide("• Creates the necessary accounting entries to maintain accurate financial records")]
    [Guide("• Makes the credit available to apply against future invoices or process as a refund")]
    [Header("Form Fields")]
    [Guide("Complete the credit note details using the fields below. Required fields are marked with an asterisk (*).")]
    [Fields(typeof(ManagerServer.Model.CreditNote))]
    internal sealed class CreditNoteForm : NakedVueForm<ManagerServer.Model.CreditNote>
    {
        [ProtoMember(1)] public Guid? Customer;

        protected override bool CanHaveImage() => true;

        protected override void OnSource(CreditNote form, ManagerServer.Model.Object source)
        {
            if (!Key.HasValue)
            {
                if (Customer.HasValue) form.Customer = Customer;

                if (source is PurchaseQuote purchaseQuote)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<CreditNote>(Business, purchaseQuote.CustomFields);
                }
                if (source is PurchaseOrder purchaseOrder)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<CreditNote>(Business, purchaseOrder.CustomFields);
                }
                if (source is PurchaseInvoice purchaseInvoice)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<CreditNote>(Business, purchaseInvoice.CustomFields);
                }
                if (source is SalesQuote salesQuote)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<CreditNote>(Business, salesQuote.CustomFields);
                }
                if (source is SalesOrder salesOrder)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<CreditNote>(Business, salesOrder.CustomFields);
                }
                if (source is SalesInvoice salesInvoice)
                {
                    Copy(salesInvoice, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<CreditNote>(Business, salesInvoice.CustomFields);
                }
                if (source is DebitNote debitNote)
                {
                    Copy(debitNote, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<CreditNote>(Business, debitNote.CustomFields);
                }
                if (source is CreditNote creditNote)
                {
                    Copy(creditNote, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<CreditNote>(Business, creditNote.CustomFields);
                }
                if (source is DeliveryNote deliveryNote)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<CreditNote>(Business, deliveryNote.CustomFields);
                }
                if (source is GoodsReceipt goodsReceipt)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<CreditNote>(Business, goodsReceipt.CustomFields);
                }
                if (source is InventoryTransfer inventoryTransfer)
                {
                    Copy(source, form);
                    form.CustomFields = ManagerServer.Query.CustomFieldExtensions.CopyCustomFields<CreditNote>(Business, inventoryTransfer.CustomFields);
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
}