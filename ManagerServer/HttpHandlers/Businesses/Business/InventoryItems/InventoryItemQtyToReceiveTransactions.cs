using System.Collections.Generic;
using System.Linq;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Model.Attributes;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryItems
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryItems), nameof(Strings.QtyToReceive), nameof(Strings.Transactions))]
    [Guide("The **Quantity to Receive Transactions** screen displays a detailed list of inventory items that have been ordered or invoiced but not yet received into stock.")]
    [Guide("This screen helps you track outstanding inventory receipts from suppliers, showing transactions from **Purchase Invoices**, **Goods Receipts**, **Debit Notes**, and *starting balances*.")]
    [Guide("Each transaction shows the quantity that remains to be received, allowing you to monitor pending deliveries and ensure accurate inventory tracking.")]
    [Columns]
    internal sealed class InventoryItemQtyToReceiveTransactions : NakedObjectsWithCustomFields<GeneralLedgerTransaction>
    {
        [ProtoMember(1)] public Guid InventoryItem;
        [ProtoMember(2)] public Guid Supplier;

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
                .Where(x => x.Supplier?.Key == Supplier)
                .Where(x => x.QtyToReceive != 0m)
                .ToArray();

            context.Set<Array>(list.OrderByDescending(x => x.Date).ToArray());

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
        [WarnIfFutureDate, MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("9d4dd2bf-2dac-4c40-8388-376f26012da0")]
        [Guide("Displays the date when the transaction was created, helping you identify older pending receipts that may require follow-up.")]
        public DateTime?[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date == DateTime.MinValue ? default(DateTime?) : x.Date).ToArray();
        }

        [Default]
        [Guid("05d35177-52f7-45c5-93de-478e7c5b2d37")]
        [Guide("Indicates the type of transaction (such as **Purchase Invoice**, **Goods Receipt**, or **Debit Note**) that created the pending receipt.")]
        public string[] GetTransaction(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => ManagerServer.Globalization.Strings.GetPropertyValue(x.Transaction?.GetType().Name)).ToArray();
        }

        [Default]
        [PaddedSorting, Short]
        [Guid("042cf1c2-cbd7-4f43-9707-ad9b4b603ab0")]
        [Guide("Displays the *reference number* of the transaction, which can be used to locate the original document.")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Transaction?.GetReference()).ToArray();
        }

        [Default]
        [Guid("9b21ab2e-5a49-42bb-ad41-f2769473cc3c")]
        [Guide("Displays the name of the *inventory item* awaiting receipt.")]
        public NamedObject[] GetInventoryItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryItem).ToArray();
        }

        [Default]
        [Guid("b34a8e7e-9738-4d4d-a267-ca679b380673")]
        [Guide("Displays the *supplier* from whom the inventory item is expected to be received.")]
        public NamedObject[] GetSupplier(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Supplier).ToArray();
        }

        [Guid("f2b2df65-dd30-4862-ab41-e2703e031bac")]
        [Guide("Displays any description entered on the transaction, providing additional context about the pending receipt.")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Guid("f39fedae-0d52-4521-b415-c2b3efeb4574")]
        [Guide("Displays the description from the specific line item within the transaction, which may include additional details about the item or special instructions.")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine?.GetLineDescription(x.Transaction)).ToArray();
        }

        [Default]
        [Right, Sum, Bold, RunningTotal]
        [Guid("3d1bc80e-bb31-4f8b-abd8-e82f432860fd")]
        [Guide("Displays the quantity still pending receipt for each transaction, with a *running total* that accumulates the quantities as you scroll through the list.")]
        public decimal[] GetQtyToReceive(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.QtyToReceive).ToArray();
        }
    }
}