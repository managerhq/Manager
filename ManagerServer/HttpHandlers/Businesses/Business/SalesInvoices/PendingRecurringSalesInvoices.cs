using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesInvoices
{
    [ProtoContract]
    [Guid("6C7EE67D-7A2F-4D7A-A97D-4DDBF2321D24")]
    [Title(nameof(Strings.SalesInvoices), nameof(Strings.Pending))]
    [Guide("The **Pending Sales Invoices** screen displays *recurring sales invoices* that are due to be created based on their scheduled dates.")]
    [Guide("From this screen, you can review which invoices are ready to be generated and process them individually or in batches.")]
    [Guide("Each pending invoice shows when it will be created, the customer details, description, and the total amount to be invoiced.")]
    [Columns]
    internal sealed class PendingRecurringSalesInvoices : NakedObjectsOfPendingRecurringTransactions<RecurringSalesInvoice>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("9961AE8C-43AB-4C7C-A0C6-7889E8503063")]
        [Guide("Displays the scheduled date when this *recurring sales invoice* will automatically generate a new invoice.")]
        public DateTime?[] GetNextIssueDate(RecurringSalesInvoice[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("67315A14-D951-42B5-A507-EF2520B3AA9A")]
        [Guide("Displays the *customer* who will receive the invoice when it is created.")]
        public string[] GetCustomer(RecurringSalesInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Customer>(x.Customer)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("D0FF2946-8689-42F3-A3BA-8CF81335BC50")]
        [Guide("Displays the description or reference that will appear on the generated invoice.")]
        public string[] GetDescription(RecurringSalesInvoice[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("80B5EE8E-3B6F-4682-BBA7-20B22E10CEB7")]
        [Guide("Displays the total amount to be invoiced, shown in the *customer's currency* if different from your base currency.")]
        public Tuple<decimal, Currency>[] GetAmount(RecurringSalesInvoice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var output = new List<Tuple<decimal, Currency>>();
            foreach (var e in rows)
            {
                var salesInvoice = new ManagerServer.Model.SalesInvoice();
                Copy(e, salesInvoice);
                var balancingTransaction = salesInvoice.CreateGeneralLedgerTransactions(database).SingleOrDefault(x => x.IsBalancing);

                output.Add(balancingTransaction?.GetTransactionAmountWithCurrency());
            }
            return output.ToArray();
        }
    }
}
