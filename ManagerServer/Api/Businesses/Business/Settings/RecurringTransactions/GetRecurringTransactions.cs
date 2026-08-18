using ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringInterAccountTransfers;
using ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringJournalEntries;
using ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringPayments;
using ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringPayslips;
using ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringPurchaseInvoices;
using ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringPurchaseOrders;
using ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringReceipts;
using ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringSalesInvoices;
using ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringSalesOrders;
using ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringSalesQuotes;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions
{
    internal sealed record RecurringTransactionsResource(
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    [ProtoContract]
    internal sealed class GetRecurringTransactions : AuthorizedEndpoint<RecurringTransactionsResource>
    {
        public override RecurringTransactionsResource AuthorizedHandle()
        {
            var links = Hyperlinks.ForCurrentDocument(this);

            links["recurringInterAccountTransfers"] = new Link(new GetRecurringInterAccountTransferBatch { Business = Business }.ToUrl());
            links["recurringJournalEntries"] = new Link(new GetRecurringJournalEntryBatch { Business = Business }.ToUrl());
            links["recurringPayments"] = new Link(new GetRecurringPaymentBatch { Business = Business }.ToUrl());
            links["recurringPayslips"] = new Link(new GetRecurringPayslipBatch { Business = Business }.ToUrl());
            links["recurringPurchaseInvoices"] = new Link(new GetRecurringPurchaseInvoiceBatch { Business = Business }.ToUrl());
            links["recurringPurchaseOrders"] = new Link(new GetRecurringPurchaseOrderBatch { Business = Business }.ToUrl());
            links["recurringReceipts"] = new Link(new GetRecurringReceiptBatch { Business = Business }.ToUrl());
            links["recurringSalesInvoices"] = new Link(new GetRecurringSalesInvoiceBatch { Business = Business }.ToUrl());
            links["recurringSalesOrders"] = new Link(new GetRecurringSalesOrderBatch { Business = Business }.ToUrl());
            links["recurringSalesQuotes"] = new Link(new GetRecurringSalesQuoteBatch { Business = Business }.ToUrl());

            return new RecurringTransactionsResource(links);
        }
    }
}
