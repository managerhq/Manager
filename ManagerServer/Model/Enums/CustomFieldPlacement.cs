using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Enums
{
    public enum CustomFieldPlacement : int
    {
        BusinessDetails,
        BankAndCashAccounts,
        Receipts,
        Payments,
        InterAccountTransfers,
        Customers,
        Suppliers,
        Employees,
        CreditNotes,
        CreditNoteLines,
        DebitNotes,
        DebitNoteLines,
        SalesInvoices,
        SalesInvoiceLines,
        WithholdingTaxReceipts,
        SalesQuotes,
        SalesQuoteLines,
        SalesOrders,
        SalesOrderLines,
        BillableTime,
        DeliveryNotes,
        DeliveryNoteLines,
        GoodsReceipts,
        GoodsReceiptLines,
        InventoryTransfers,
        InventoryTransferLines,
        Payslips,
        JournalEntries,
        InventoryItems,
        InventoryWriteOffs,
        FixedAssets,
        IntangibleAssets,
        ExpenseClaims,
        PurchaseInvoices,
        PurchaseQuotes,
        PurchaseOrders,
        ProductionOrders,
        CapitalAccounts,
        SpecialAccounts,
        Folders,
        NonInventoryItems,
        TaxCodes
    }
}
