using ManagerServer.Api.Businesses.Business.Settings.Footers.CreditNotes;
using ManagerServer.Api.Businesses.Business.Settings.Footers.DebitNotes;
using ManagerServer.Api.Businesses.Business.Settings.Footers.DeliveryNotes;
using ManagerServer.Api.Businesses.Business.Settings.Footers.ExpenseClaims;
using ManagerServer.Api.Businesses.Business.Settings.Footers.GoodsReceipts;
using ManagerServer.Api.Businesses.Business.Settings.Footers.InterAccountTransfers;
using ManagerServer.Api.Businesses.Business.Settings.Footers.JournalEntries;
using ManagerServer.Api.Businesses.Business.Settings.Footers.Payments;
using ManagerServer.Api.Businesses.Business.Settings.Footers.Payslips;
using ManagerServer.Api.Businesses.Business.Settings.Footers.PurchaseInvoices;
using ManagerServer.Api.Businesses.Business.Settings.Footers.PurchaseOrders;
using ManagerServer.Api.Businesses.Business.Settings.Footers.PurchaseQuotes;
using ManagerServer.Api.Businesses.Business.Settings.Footers.Receipts;
using ManagerServer.Api.Businesses.Business.Settings.Footers.SalesInvoices;
using ManagerServer.Api.Businesses.Business.Settings.Footers.SalesOrders;
using ManagerServer.Api.Businesses.Business.Settings.Footers.SalesQuotes;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business.Settings.Footers
{
    internal sealed record FootersResource(
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    [ProtoContract]
    internal sealed class GetFooters : AuthorizedEndpoint<FootersResource>
    {
        public override FootersResource AuthorizedHandle()
        {
            var links = Hyperlinks.ForCurrentDocument(this);

            links["creditNotes"] = new Link(new GetCreditNoteFooterBatch { Business = Business }.ToUrl());
            links["debitNotes"] = new Link(new GetDebitNoteFooterBatch { Business = Business }.ToUrl());
            links["deliveryNotes"] = new Link(new GetDeliveryNoteFooterBatch { Business = Business }.ToUrl());
            links["expenseClaims"] = new Link(new GetExpenseClaimFooterBatch { Business = Business }.ToUrl());
            links["goodsReceipts"] = new Link(new GetGoodsReceiptFooterBatch { Business = Business }.ToUrl());
            links["interAccountTransfers"] = new Link(new GetInterAccountTransferFooterBatch { Business = Business }.ToUrl());
            links["journalEntries"] = new Link(new GetJournalEntryFooterBatch { Business = Business }.ToUrl());
            links["payments"] = new Link(new GetPaymentFooterBatch { Business = Business }.ToUrl());
            links["payslips"] = new Link(new GetPayslipFooterBatch { Business = Business }.ToUrl());
            links["purchaseInvoices"] = new Link(new GetPurchaseInvoiceFooterBatch { Business = Business }.ToUrl());
            links["purchaseOrders"] = new Link(new GetPurchaseOrderFooterBatch { Business = Business }.ToUrl());
            links["purchaseQuotes"] = new Link(new GetPurchaseQuoteFooterBatch { Business = Business }.ToUrl());
            links["receipts"] = new Link(new GetReceiptFooterBatch { Business = Business }.ToUrl());
            links["salesInvoices"] = new Link(new GetSalesInvoiceFooterBatch { Business = Business }.ToUrl());
            links["salesOrders"] = new Link(new GetSalesOrderFooterBatch { Business = Business }.ToUrl());
            links["salesQuotes"] = new Link(new GetSalesQuoteFooterBatch { Business = Business }.ToUrl());

            return new FootersResource(links);
        }
    }
}
