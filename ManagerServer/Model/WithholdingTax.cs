using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("96f2f394-8ac1-4e93-a926-5761ce8f0732")]
    public sealed class WithholdingTax : ManagerServer.Model.Object
    {
        [Header("Withholding Tax Receivable")]
        [Guide("Enable this option to track withholding tax that customers deduct from payments to you.")]
        [Guide("This creates a `WithholdingTaxReceivable` account on your balance sheet to monitor the withholding tax balance for each customer.")]
        [Guide("When enabled, a `WithholdingTax` section appears when creating new `SalesInvoices` and `CreditNotes`, allowing you to specify withholding tax amounts.")]
        [Guide("Withholding tax amounts accumulate in the `WithholdingTaxReceivable` account. Clear this account by recording entries in the `WithholdingTaxReceipts` tab.")]
        [ProtoMember(1)] public bool WithholdingTaxReceivable { get; set; }

        [Header("Withholding Tax Payable")]
        [Guide("Enable this option to track withholding tax you must deduct from payments to suppliers.")]
        [Guide("This creates a `WithholdingTaxPayable` account on your balance sheet to monitor the withholding tax balance for each supplier.")]
        [Guide("When enabled, a `WithholdingTax` section appears when creating new `PurchaseInvoices`, allowing you to specify withholding tax amounts.")]
        [ProtoMember(2)] public bool WithholdingTaxPayable { get; set; }
    }
}
