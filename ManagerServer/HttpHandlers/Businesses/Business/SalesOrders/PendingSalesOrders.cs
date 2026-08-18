using ManagerServer.Model;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesOrders
{
    [ProtoContract]
    [Guid("2FD90095-FEC7-4480-BA67-603A8771FBA6")]
    [Title(nameof(Strings.SalesOrders), nameof(Strings.Pending))]
    [Guide("The **Pending Sales Orders** screen displays all recurring sales orders that are ready to be processed.")]
    [Guide("When a recurring sales order reaches its scheduled date, it appears here for your review.")]
    [Guide("You can then create the actual sales order by clicking the appropriate action button.")]
    [Columns]
    internal sealed class PendingSalesOrders : NakedObjectsOfPendingRecurringTransactions<ManagerServer.Model.RecurringSalesOrder>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("CED9A27A-BEE9-45DA-8158-5AC866CCD32A")]
        [Guide("Displays the date when this recurring sales order is scheduled to be created as an actual sales order.")]
        public DateTime?[] GetNextIssueDate(RecurringSalesOrder[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("A0D58801-0474-4E9A-BA19-514C4A19833D")]
        [Guide("Displays the customer who will receive this sales order when it is created.")]
        public string[] GetCustomer(RecurringSalesOrder[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Customer>(x.Customer)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("2D845A2F-9BD7-4C5F-9AC7-64AE17D8A8F9")]
        [Guide("Displays the description or reference information for this recurring sales order.")]
        public string[] GetDescription(RecurringSalesOrder[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }
    }
}
