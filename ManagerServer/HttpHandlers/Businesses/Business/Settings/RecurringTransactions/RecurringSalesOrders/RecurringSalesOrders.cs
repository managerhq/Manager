using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringSalesOrders
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(SalesOrders))]
    [Guid("5213ce62-fb4d-4e87-a8b3-7e449a19c3df")]
    [Title(nameof(Strings.RecurringSalesOrders), nameof(Strings.Pending))]
    [Guide("Recurring sales orders allow you to create sales orders that automatically generate on a predefined schedule for regular customers.")]
    [Guide("This feature is ideal for subscription services, regular supply agreements, or any business arrangement where customers order the same items repeatedly.")]
    [Guide("The system will automatically create new sales orders based on the frequency you set, saving time and reducing manual data entry.")]
    [Columns]
    internal sealed class RecurringSalesOrders : NakedObjectsWithAutomaticRows<ManagerServer.Model.RecurringSalesOrder>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("6ffb82de-b7c2-4789-bd84-f1d30a6e1a7a")]
        [Guide("Displays the scheduled date when the system will automatically generate the next sales order for each recurring transaction.")]
        public DateTime?[] GetNextIssueDate(RecurringSalesOrder[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("86f2efa9-4d05-41f6-9074-c1cd1e80a8e2")]
        [Guide("Displays the customer name associated with each recurring sales order.")]
        public string[] GetCustomer(RecurringSalesOrder[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Customer>(x.Customer)?.Name).ToArray();
        }

        [Default]
        [Guid("10397caa-3161-450d-8426-dc88ec2bf054")]
        [Guide("Displays the description or summary of what each recurring sales order contains.")]
        public string[] GetDescription(RecurringSalesOrder[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("b8e6afdf-28f5-4888-99ba-f3e2da2d3cba")]
        [Guide("Displays the total amount for each recurring sales order in the customer's currency, including all line items and applicable taxes.")]
        public Tuple<decimal, Currency>[] GetAmount(RecurringSalesOrder[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var output = new List<Tuple<decimal, Currency>>();
            foreach (var e in rows)
            {
                var salesOrder = new ManagerServer.Model.SalesOrder();
                Copy(e, salesOrder);
                var balancingTransaction = salesOrder.CreateGeneralLedgerTransactions(database).SingleOrDefault(x => x.IsBalancing);

                output.Add(balancingTransaction?.GetTransactionAmountWithCurrency());
            }
            return output.ToArray();
        }
    }
}