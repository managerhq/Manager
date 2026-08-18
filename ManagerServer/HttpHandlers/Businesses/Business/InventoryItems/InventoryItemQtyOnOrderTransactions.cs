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
    [Title(nameof(Strings.InventoryItems), nameof(Strings.QtyOnOrder), nameof(Strings.Transactions))]
    [Guide("The *Quantity on Order Transactions* screen displays all transactions that affect the *quantity on order* for a specific inventory item.")]
    [Guide("This includes the original **Purchase Order** that created the order, **Goods Receipts** that partially or fully receive the items, and **Purchase Invoices** that may also affect the *quantity on order*.")]
    [Guide("The screen helps you track the fulfillment status of purchase orders by showing how ordered quantities are reduced as items are received or invoiced.")]
    [Columns]
    internal sealed class InventoryItemQtyOnOrderTransactions : NakedObjectsWithCustomFields<GeneralLedgerTransaction>
    {
        [ProtoMember(1), JsonProperty("inventoryItem")] public Guid InventoryItem;
        [ProtoMember(2), JsonProperty("purchaseOrder")] public Guid PurchaseOrder;

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);

            var transactions = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();
            transactions.AddRange(database.OfType<PurchaseOrder>().SelectMany(x => x.GetGeneralLedgerTransactions(database)));
            transactions.AddRange(database.OfType<GoodsReceipt>().SelectMany(x => x.GetGeneralLedgerTransactions(database)));
            transactions.AddRange(database.OfType<PurchaseInvoice>().SelectMany(x => x.GetGeneralLedgerTransactions(database)));
            transactions = transactions
                .Where(x => x.PurchaseOrderAsTransaction?.Key == PurchaseOrder || x.PurchaseOrder?.Key == PurchaseOrder)
                .Where(x => x.InventoryItem?.Key == InventoryItem)
                .Where(x => x.QtyOnOrder != 0m)
                .ToList();

            context.Set<Array>(transactions.OrderByDescending(x => x.Date).ThenBy(x => x.QtyOnOrder > 0m).ToArray());

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => TransactionViewer.GetEditHandler(Business, x.Transaction, referrer)).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => TransactionViewer.GetViewHandler(Business, x.Transaction, referrer)).ToArray();
        }

        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guide("The date when the transaction was recorded in the system.")]
        public DateTime[] GetDate(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guide("Displays the type of transaction (**Purchase Order**, **Goods Receipt**, or **Purchase Invoice**) along with its reference number.")]
        [Guide("Click on the transaction to view or edit its details.")]
        public string[] GetTransaction(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Transaction.GetTransactionName()).ToArray();
        }

        [Default]
        [Guide("The supplier associated with the transaction.")]
        [Guide("All transactions in this view will typically show the same supplier, as they relate to a specific purchase order.")]
        public NamedObject[] GetSupplier(GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.Supplier).ToArray();
        }

        [Default]
        [Guide("The inventory item being ordered or received.")]
        [Guide("This column confirms that all displayed transactions relate to the same inventory item you are viewing.")]
        public NamedObject[] GetInventoryItem(GeneralLedgerTransaction[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.InventoryItem).ToArray();
        }

        [Bold, Default, Right, Sum, RunningTotal]
        [Guide("The quantity that affects the *on-order balance* for this item.")]
        [Guide("Positive values increase the *quantity on order* (from **Purchase Orders**), while negative values decrease it (from **Goods Receipts** or **Purchase Invoices**).")]
        [Guide("The running total shows the cumulative *quantity on order* after each transaction, helping you track the remaining unfulfilled quantity.")]
        public decimal[] GetQtyOnOrder(GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.QtyOnOrder).ToArray();
        }
    }
}