using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryItems
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryItems), nameof(Strings.QtyToDeliver))]
    [Guide("The **Qty to Deliver** screen displays pending delivery commitments organized by customer.")]
    [Guide("This screen tracks inventory items that have been sold to customers but not yet delivered.")]
    [Guide("The quantities shown represent outstanding obligations to deliver physical goods to customers.")]
    [Columns]
    internal sealed class InventoryItemQtyToDeliver : NakedObjectsWithCustomFields<Tuple<ManagerServer.Model.Customer, decimal>>
    {
        [ProtoMember(1)] public Guid InventoryItem;

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);

            var inventoryTransactions = new List<Transaction>();
            inventoryTransactions.AddRange(database.OfType<DeliveryNote>());
            inventoryTransactions.AddRange(database.OfType<SalesInvoice>());
            inventoryTransactions.AddRange(database.OfType<CreditNote>());
            inventoryTransactions.AddRange(database.OfType<InventoryItemStartingBalance>());

            var list = inventoryTransactions
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem?.Key == InventoryItem)
                .Where(x => x.Customer != null)
                .Where(x => x.QtyToDeliver != 0m)
                .Select(x => new Tuple<Customer, decimal>(x.Customer, x.QtyToDeliver))
                .ToArray();

            context.Set<Array>(list.GroupBy(x => x.Item1).Select(x => new Tuple<Customer, decimal>(x.Key, x.Sum(y => y.Item2))).OrderByDescending(x => Math.Abs(x.Item2)).ToArray());

            base.InnerGet4(context);
        }

        [Default]
        [Guid("c87fce65-2b7e-4c33-9f8f-ca06945f6169")]
        [Guide("Shows the customer name for each pending delivery commitment.")]
        public NamedObject[] GetCustomer(Tuple<ManagerServer.Model.Customer, decimal>[] rows)
        {
            return rows.Select(x => x.Item1).ToArray();
        }

        [Default, Right, Sum, Bold]
        [Guid("9b3f04ca-0e41-489f-8229-dfe2d66fa2ad")]
        [Guide("Shows the quantity of items awaiting delivery to each customer.")]
        [Guide("Click the **quantity** to view the detailed transactions that make up this pending amount.")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyToDeliver(Tuple<ManagerServer.Model.Customer, decimal>[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(x.Item2, new InventoryItemQtyToDeliverTransactions() { Business = Business, InventoryItem = InventoryItem, Customer = x.Item1.Key, Referrer = referrer })).ToArray();
        }
    }
}