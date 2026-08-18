using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringPurchaseOrders
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(PurchaseOrders))]
    [Guid("fd201cc4-8e64-41d6-8ea2-c3d86022a3b2")]
    [Title(nameof(Strings.RecurringPurchaseOrders), nameof(Strings.Pending))]
    [Guide("Recurring purchase orders automate the creation of purchase orders on a scheduled basis, saving time for regular or repeated orders from suppliers.")]
    [Guide("Use this feature when you need to order the same items from the same supplier at regular intervals, such as monthly supplies, quarterly inventory replenishments, or any other predictable ordering pattern.")]
    [Header("Setting Up Recurring Purchase Orders")]
    [Guide("To create a recurring purchase order, click the **New Recurring Purchase Order** button. You'll need to specify the supplier, items to order, quantities, prices, and the schedule for when orders should be automatically generated.")]
    [Guide("Each recurring purchase order will automatically create new purchase orders based on your defined schedule. The system will generate these orders on the specified dates without manual intervention.")]
    [Header("Managing Your Recurring Orders")]
    [Guide("The table below shows all your recurring purchase orders with key information including the next issue date, supplier, description, and total amount. You can edit or delete any recurring order by clicking on it.")]
    [Guide("When a recurring purchase order generates a new order, you'll find it in the regular **Purchase Orders** tab where you can review, modify if needed, and process it like any other purchase order.")]
    [Columns]
    internal sealed class RecurringPurchaseOrders : NakedObjectsWithAutomaticRows<ManagerServer.Model.RecurringPurchaseOrder>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("96c0c38b-761e-4a92-9e91-b3bf532de1c9")]
        [Guide("Displays the date when the next purchase order will be automatically generated. This helps you track when orders will be created and ensures you're prepared for upcoming supplier orders.")]
        public DateTime?[] GetNextIssueDate(RecurringPurchaseOrder[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("fe737271-7ac0-40a0-b308-039dd404aa3b")]
        [Guide("Shows the supplier name associated with each recurring purchase order. This helps you quickly identify which supplier will receive the automatically generated orders.")]
        public string[] GetSupplier(RecurringPurchaseOrder[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Supplier>(x.Supplier)?.Name).ToArray();
        }

        [Default]
        [Guid("7899d473-125d-4eca-b147-3e1d6f1fe604")]
        [Guide("Displays the description or reference for each recurring purchase order. This typically contains details about what is being ordered or any notes to help identify the purpose of the recurring order.")]
        public string[] GetDescription(RecurringPurchaseOrder[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("6e315b6e-3840-4bd4-8a5f-eda3baefce4b")]
        [Guide("Shows the total amount for each recurring purchase order in the supplier's currency. This represents the full order value that will be created each time the recurring order generates a new purchase order.")]
        public Tuple<decimal, ManagerServer.Model.Currency>[] GetAmount(ManagerServer.Model.RecurringPurchaseOrder[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var output = new List<Tuple<decimal, ManagerServer.Model.Currency>>();
            foreach (var e in rows)
            {
                var purchaseOrder = new ManagerServer.Model.PurchaseOrder();
                Copy(e, purchaseOrder);
                var balancingTransaction = purchaseOrder.CreateGeneralLedgerTransactions(database).SingleOrDefault(x => x.IsBalancing);

                output.Add(balancingTransaction?.GetReversedTransactionAmountWithCurrency());
            }
            return output.ToArray();
        }
    }
}