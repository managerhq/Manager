using System.Linq;
using System.Collections.Generic;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringSalesInvoices
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(SalesInvoices))]
    [Guid("a01563ac-7538-452c-be88-2c7ce4b76f59")]
    [Title(nameof(Strings.RecurringSalesInvoices), nameof(Strings.Pending))]
    [Guide("The **Recurring Sales Invoices** tab allows you to create and manage sales invoices that are automatically generated on a scheduled basis.")]
    [Guide("Use this feature to automate regular billing for customers who receive the same invoice at fixed intervals, such as monthly subscriptions, quarterly service fees, or annual maintenance contracts.")]
    [Guide("Once set up, the system will automatically create new sales invoices based on your specified schedule, saving time and ensuring consistent billing.")]
    [Columns]
    internal sealed class RecurringSalesInvoices : NakedObjectsWithAutomaticRows<RecurringSalesInvoice>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("7019af9d-e265-413f-9164-338d07ba774b")]
        [Guide("Displays the date when the next sales invoice will be automatically generated for each recurring invoice schedule.")]
        public DateTime?[] GetNextIssueDate(RecurringSalesInvoice[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("e8f66fc6-d8c4-481b-b6b2-a1c988358b3a")]
        [Guide("Displays the customer name associated with each recurring sales invoice.")]
        public string[] GetCustomer(RecurringSalesInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Customer>(x.Customer)?.Name).ToArray();
        }

        [Default]
        [Guid("3bc3da86-a295-4671-b1d5-a7f8623f8f0f")]
        [Guide("Displays the description or summary of what is being invoiced on a recurring basis.")]
        public string[] GetDescription(RecurringSalesInvoice[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("9ac77aee-27a1-4425-b155-99f8b8bad483")]
        [Guide("Displays the total amount of each recurring invoice in the customer's currency, including all line items and applicable taxes.")]
        public Tuple<decimal, Currency>[] GetAmount(RecurringSalesInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var output = new List<Tuple<decimal, Currency>>();
            foreach (var e in rows)
            {
                var salesInvoice = new ManagerServer.Model.SalesInvoice();
                Copy(e, salesInvoice);
                var balancingTransaction = salesInvoice.CreateGeneralLedgerTransactions(database)?.SingleOrDefault(x => x.IsBalancing);

                output.Add(balancingTransaction?.GetTransactionAmountWithCurrency());
            }
            return output.ToArray();
        }
    }
}