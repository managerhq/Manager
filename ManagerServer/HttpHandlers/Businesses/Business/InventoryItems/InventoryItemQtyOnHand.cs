using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using Newtonsoft.Json;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryItems
{
    [ProtoContract]
    [Guid("b95e45ac-f78c-4b99-8874-e3d17796b711")]
    [Title(nameof(Strings.InventoryItems), nameof(Strings.QtyOnHand))]
    [Guide("The **Quantity on Hand** screen displays current *inventory levels* for a specific *inventory item* across all locations.")]
    [Guide("This screen helps you track where your inventory is stored and how much is available at each location.")]
    [Guide("Inventory levels are automatically updated as you record transactions such as sales invoices, purchase invoices, inventory transfers, and inventory write-offs.")]
    [Columns]
    internal sealed class InventoryItemQtyOnHand : NakedObjectsWithCustomFields<Tuple<CustomInventoryLocation, decimal>>
    {
        [ProtoMember(1), JsonProperty("inventoryItem")] public Guid InventoryItem;
        
        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);

            var rows = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();

            var inventoryTransactions = new List<Transaction>();
            inventoryTransactions.AddRange(database.OfType<JournalEntry>());
            inventoryTransactions.AddRange(database.OfType<Payment>());
            inventoryTransactions.AddRange(database.OfType<Receipt>());
            inventoryTransactions.AddRange(database.OfType<ExpenseClaim>());
            inventoryTransactions.AddRange(database.OfType<InventoryWriteOff>());
            inventoryTransactions.AddRange(database.OfType<InventoryTransfer>());
            inventoryTransactions.AddRange(database.OfType<ProductionOrder>());
            inventoryTransactions.AddRange(database.OfType<SalesInvoice>());
            inventoryTransactions.AddRange(database.OfType<CreditNote>());
            inventoryTransactions.AddRange(database.OfType<PurchaseInvoice>());
            inventoryTransactions.AddRange(database.OfType<DebitNote>());
            inventoryTransactions.AddRange(database.OfType<InventoryItemStartingBalance>());

            rows.AddRange(inventoryTransactions
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem?.Key == InventoryItem)
                .Where(x => x.QtyOnHand != 0m));

            rows.AddRange(database.OfType<GoodsReceipt>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem?.Key == InventoryItem)
                .Where(x => x.Supplier != null)
                .Where(x => x.QtyOnHand != 0m));

            rows.AddRange(database.OfType<DeliveryNote>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem?.Key == InventoryItem)
                .Where(x => x.Customer != null)
                .Where(x => x.QtyOnHand != 0m));
            
            context.Set<Array>(rows.GroupBy(x => x.InventoryLocation).Select(x => new Tuple<CustomInventoryLocation, decimal>(x.Key, x.Sum(y => y.QtyOnHand))).OrderByDescending(x => Math.Abs(x.Item2)).ToArray());

            base.InnerGet4(context);
        }

        [Default]
        [Guid("2eddc46f-5ecd-4ad8-836d-6174b818ba35")]
        [Guide("Displays the name of each *inventory location* or warehouse where this item is stored.")]
        [Guide("If you have not created custom *inventory locations*, the default location will be shown.")]
        public NamedObject[] GetInventoryLocation(Tuple<ManagerServer.Model.CustomInventoryLocation, decimal>[] rows)
        {
            var defaultInventoryLocation = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.DefaultInventoryLocation>();
            return rows.Select(x => x.Item1 ?? new CustomInventoryLocation() {
                Name = defaultInventoryLocation.GetName() }).ToArray();
        }

        [Default, Right, Sum, Bold]
        [Guid("ed65b005-74bf-4a48-979b-1333d06fd5a4")]
        [Guide("Shows the current *quantity on hand* at each location.")]
        [Guide("Click on any quantity to view all transactions that affected inventory levels at that specific location.")]
        [Guide("The total at the bottom shows the combined quantity across all locations.")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyOnHand(Tuple<ManagerServer.Model.CustomInventoryLocation, decimal>[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(x.Item2, new InventoryItemQtyOnHandTransactions() { Business = Business, InventoryItem = InventoryItem, InventoryLocation = x.Item1?.Key, Referrer = referrer })).ToArray();
        }
    }
}
