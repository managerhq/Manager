using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;
using Newtonsoft.Json;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryItems
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryItems), nameof(Strings.QtyOnHand), nameof(Strings.Transactions))]
    [Guide("The **Qty on Hand - Transactions** screen shows detailed inventory movements for a specific *inventory item* and *inventory location*.")]
    [Guide("This report displays all transactions that have affected inventory quantities, providing a complete audit trail of inventory changes.")]
    [Guide("Each transaction shows the quantity change and maintains a running balance to help you track inventory levels over time.")]
    [Columns]
    internal sealed class InventoryItemQtyOnHandTransactions : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        [ProtoMember(1), JsonProperty("inventoryItem")] public Guid InventoryItem;
        [ProtoMember(2), JsonProperty("inventoryLocation")] public Guid? InventoryLocation;

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
                .Where(x => x.InventoryLocation?.Key == InventoryLocation)
                .Where(x => x.QtyOnHand != 0m));

            rows.AddRange(database.OfType<GoodsReceipt>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem?.Key == InventoryItem)
                .Where(x => x.InventoryLocation?.Key == InventoryLocation)
                .Where(x => x.Supplier != null)
                .Where(x => x.QtyOnHand != 0m));

            rows.AddRange(database.OfType<DeliveryNote>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem?.Key == InventoryItem)
                .Where(x => x.InventoryLocation?.Key == InventoryLocation)
                .Where(x => x.Customer != null)
                .Where(x => x.QtyOnHand != 0m));
            
            context.Set<Array>(rows.OrderByDescending(x => x.Date).ToArray());

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => HttpHandlers.Businesses.Business.Form.GetEdit(x.Transaction, Business, referrer)).ToArray();
        }

        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("4de14734-843f-4362-bd24-3db9afa18cfe")]
        [Guide("The date when the inventory movement occurred.")]
        [Guide("Transactions are displayed with the most recent first, making it easy to see current inventory activity.")]
        public DateTime[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("85a28c16-d871-423e-a8ed-76a97ffbf97b")]
        [Guide("The type of transaction and its *reference number*.")]
        [Guide("Click on the transaction to view or edit its details.")]
        public string[] GetTransaction(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.GetTransactionName()).ToArray();
        }

        [Default]
        [Guid("f1f5ab1b-f3db-413c-a11e-838684349fbf")]
        [Guide("The customer associated with this inventory movement.")]
        [Guide("This column appears when inventory is sold or delivered to customers.")]
        public NamedObject[] GetCustomer(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.Customer).ToArray();
        }

        [Default]
        [Guid("f578b80f-b4a4-4e62-8757-daa4abd85485")]
        [Guide("The *inventory item* being tracked in this report.")]
        [Guide("When viewing transactions for a specific item, this column shows the same item for all rows.")]
        public NamedObject[] GetInventoryItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.InventoryItem).ToArray();
        }

        [Default]
        [Guid("2f3f720a-d93a-4d07-8d11-5f56ec551a19")]
        [Guide("The supplier associated with this inventory movement.")]
        [Guide("This column appears when inventory is received from suppliers or returned to them.")]
        public NamedObject[] GetSupplier(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.Supplier).ToArray();
        }

        [Default]
        [Guid("9f80c651-6a08-472a-8900-686aabc17b46")]
        [Guide("The *inventory location* where this movement occurred.")]
        [Guide("When viewing transactions for a specific location, this column shows the same location for all rows.")]
        public NamedObject[] GetInventoryLocation(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.InventoryLocation).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [RunningTotal]
        [Guid("049987c6-09e5-4036-8a45-787d28eca3c0")]
        [Guide("The quantity change for this transaction.")]
        [Guide("Positive values increase inventory (purchases, *goods receipts*, *production orders*, customer returns).")]
        [Guide("Negative values decrease inventory (sales, deliveries, *inventory write-offs*, supplier returns).")]
        [Guide("The running total column shows the cumulative inventory quantity after each transaction, helping you track inventory levels over time.")]
        public decimal[] GetQtyOnHand(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.QtyOnHand).ToArray();
        }
    }
}
