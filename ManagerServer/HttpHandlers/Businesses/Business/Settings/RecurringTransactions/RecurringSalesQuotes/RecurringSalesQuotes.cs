using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringSalesQuotes
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(SalesQuotes))]
    [Guid("465bb58d-5c2d-40b9-9542-76a4ac38413e")]
    [Title(nameof(Strings.RecurringSalesQuotes), nameof(Strings.Pending))]
    [Guide("Recurring sales quotes allow you to automatically generate sales quotes for your customers on a scheduled basis. This is useful for regular proposals or quotations that you send to customers periodically.")]
    [Guide("Each recurring sales quote contains all the details needed to generate a complete sales quote, including customer information, line items, pricing, and terms. The system will automatically create the actual sales quote based on your configured schedule.")]
    [Guide("To create a new recurring sales quote, click the **New Recurring Sales Quote** button. You can set the frequency (daily, weekly, monthly, etc.) and specify when the quotes should be generated.")]
    [Columns]
    internal sealed class RecurringSalesQuotes : NakedObjectsWithAutomaticRows<ManagerServer.Model.RecurringSalesQuote>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("f8402fee-4f6a-475c-ad21-bd9342eb27a5")]
        [Guide("Displays the date when the next sales quote will be automatically generated. This date is calculated based on the *recurrence pattern* you have configured for each recurring sales quote.")]
        public DateTime?[] GetNextIssueDate(RecurringSalesQuote[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("bc47ca0d-98e9-416e-b1a2-58d90908d570")]
        [Guide("Displays the customer who will receive the automatically generated sales quotes. This is the customer you selected when setting up the recurring sales quote.")]
        public string[] GetCustomer(RecurringSalesQuote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Customer>(x.Customer)?.Name).ToArray();
        }

        [Default]
        [Guid("88862a69-3c9e-4795-985d-c98fa1c02d64")]
        [Guide("Displays the description or summary of what the recurring sales quote contains. This helps you identify the purpose of each recurring quote at a glance.")]
        public string[] GetDescription(RecurringSalesQuote[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("1fea1889-a647-4728-ab10-ab1d8cd9df12")]
        [Guide("Displays the total amount of each recurring sales quote. If the customer uses a foreign currency, the amount will be shown in their currency. This total includes all line items and any applicable taxes configured in the recurring quote template.")]
        public Tuple<decimal, Currency>[] GetAmount(RecurringSalesQuote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var output = new List<Tuple<decimal, Currency>>();
            foreach (var e in rows)
            {
                var salesQuote = new ManagerServer.Model.SalesQuote();
                Copy(e, salesQuote);
                var balancingTransaction = salesQuote.CreateGeneralLedgerTransactions(database).SingleOrDefault(x => x.IsBalancing);

                output.Add(balancingTransaction?.GetTransactionAmountWithCurrency());
            }
            return output.ToArray();
        }
    }
}
