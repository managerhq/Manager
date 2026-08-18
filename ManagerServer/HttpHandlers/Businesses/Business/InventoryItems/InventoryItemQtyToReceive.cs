using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryItems
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryItems), nameof(Strings.QtyToReceive))]
    [Guide("The **Quantity to Receive** screen displays pending receipts from suppliers for inventory items that have been purchased but not yet received into stock.")]
    [Guide("This screen helps you track outstanding purchase orders and monitor which items are expected to arrive from each supplier.")]
    [Guide("The quantities shown represent items that will increase your *inventory* once they are received through *goods receipts* or other receiving transactions.")]
    [Columns]
    internal sealed class InventoryItemQtyToReceive : NakedObjectsWithCustomFields<Tuple<ManagerServer.Model.Supplier, decimal>>
    {
        [ProtoMember(1)] public Guid InventoryItem;

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);

            var inventoryTransactions = new List<Transaction>();
            inventoryTransactions.AddRange(database.OfType<GoodsReceipt>());
            inventoryTransactions.AddRange(database.OfType<PurchaseInvoice>());
            inventoryTransactions.AddRange(database.OfType<DebitNote>());
            inventoryTransactions.AddRange(database.OfType<InventoryItemStartingBalance>());

            var list = inventoryTransactions
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem?.Key == InventoryItem)
                .Where(x => x.Supplier != null)
                .Where(x => x.QtyToReceive != 0m)
                .Select(x => new Tuple<Supplier, decimal>(x.Supplier, x.QtyToReceive))
                .ToArray();

            context.Set<Array>(list.GroupBy(x => x.Item1).Select(x => new Tuple<Supplier, decimal>(x.Key, x.Sum(y => y.Item2))).OrderByDescending(x => Math.Abs(x.Item2)).ToArray());

            base.InnerGet4(context);
        }

        [Default]
        [Guid("d7549439-666c-472b-a9fb-5a6b28751b6f")]
        [Guide("Displays the supplier name from whom the inventory items are expected to be received.")]
        public NamedObject[] GetSupplier(Tuple<ManagerServer.Model.Supplier, decimal>[] rows)
        {
            return rows.Select(x => x.Item1).ToArray();
        }

        [Default, Right, Sum, Bold]
        [Guid("6c2ca8fb-63d4-4445-8db1-59610cd4dcb2")]
        [Guide("Shows the total quantity of inventory items pending receipt from each supplier.")]
        [Guide("**Click on any quantity** to view the detailed list of transactions that make up this pending amount.")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyToReceive(Tuple<ManagerServer.Model.Supplier, decimal>[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(x.Item2, new InventoryItemQtyToReceiveTransactions() { Business = Business, InventoryItem = InventoryItem, Supplier = x.Item1.Key, Referrer = referrer })).ToArray();
        }
    }
}