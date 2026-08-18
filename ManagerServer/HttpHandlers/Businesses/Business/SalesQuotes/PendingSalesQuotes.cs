using System.Linq;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesQuotes
{
    [ProtoContract]
    [Guid("D37DBC41-371B-45E9-99E3-FF937A8B985E")]
    [Title(nameof(Strings.SalesQuotes), nameof(Strings.Pending))]
    [Guide("The **Pending Sales Quotes** screen displays *recurring sales quotes* that are due to be created based on their schedules.")]
    [Guide("This screen helps you manage and process *recurring sales quotes* efficiently by showing which quotes need to be generated.")]
    [Guide("Review the list of pending quotes and click **Create** to generate the actual *sales quotes* for your customers.")]
    [Columns]
    internal sealed class PendingSalesQuotes : NakedObjectsOfPendingRecurringTransactions<ManagerServer.Model.RecurringSalesQuote>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("C66576F3-3781-43F1-A681-E8E0AF4EE85B")]
        [Guide("Displays the scheduled date when the *recurring sales quote* will be automatically generated as a new *sales quote*.")]
        public DateTime?[] GetNextIssueDate(RecurringSalesQuote[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("905B15AF-B038-4B55-836D-AA79BF308090")]
        [Guide("Displays the *customer* who will receive the generated *sales quote* when it is created from the recurring template.")]
        public string[] GetCustomer(RecurringSalesQuote[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Customer>(x.Customer)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("27D29E33-7234-4A98-9234-6A6863DB0432")]
        [Guide("Shows the description from the *recurring sales quote* template that helps identify the content and purpose of the quote.")]
        public string[] GetDescription(RecurringSalesQuote[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }
    }
}
