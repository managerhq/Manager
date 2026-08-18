using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query;
using ManagerServer.Model;
using ProtoBuf;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;
using Newtonsoft.Json;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryItems
{
    [ProtoContract]
    [Guid("5b81aea4-1a54-4dbd-8e28-b58ec595d976")]
    [Title(nameof(Strings.InventoryItems), nameof(Strings.QtyOwned))]
    [Guide("The **Inventory Items - Qty Owned** screen displays a complete list of transactions that affect the *quantity owned* for a specific inventory item.")]
    [Guide("This screen helps you track how inventory quantities change over time through purchases, sales, and other transactions.")]
    [Header("Accessing the Qty Owned Screen")]
    [Guide("To access this screen, navigate to the **Inventory Items** tab.")]
    [TabScreenshot("fa-inventory", nameof(Strings.InventoryItems))]
    [Guide("Next, click on the number displayed in the **Qty Owned** column for any inventory item:")]
    [ColumnScreenshot(nameof(Strings.QtyOwned), 32)]
    [Header("Understanding the Columns")]
    [Guide("The screen displays transactions in reverse chronological order, with the most recent transactions appearing first.")]
    [Guide("Each row represents a transaction that has changed the quantity owned of the selected inventory item.")]
    [Columns]
    [Guide("Click the **Edit Columns** button to customize which columns are visible and arrange them according to your preferences.")]
    internal sealed class InventoryItemQtyOwned : NakedObjectsWithCustomFields<GeneralLedgerTransaction>
    {
        [ProtoMember(1), JsonProperty("inventoryItem")] public Guid InventoryItem;

        protected override void InnerGet4(Context context)
        {
            var rows = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem?.Key == InventoryItem)
                //.Where(x => !x.IsAutomaticCostOfGoodsSalesTransaction && !x.IsTaxTransaction)
                .Where(x => x.Transaction is not InventoryTransfer && x.Transaction is not DeliveryNote && x.Transaction is not GoodsReceipt)
                .Where(x => x.Qty.HasValue && x.Qty.Value != 0m)
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.Qty.Value < 0m)
                .ToArray();

            context.Set<Array>(rows);

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
        [Guid("153f5955-d7cf-4b51-9ac6-eb9ee066c1cb")]
        [Guide("The date when the inventory ownership transaction occurred.")]
        [Guide("This field tracks when inventory items were purchased, sold, written off, or otherwise changed ownership.")]
        [Guide("Future dates will display a warning indicator, as ownership changes typically reflect current or past events rather than future transactions.")]
        public DateTime?[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date == DateTime.MinValue ? default(DateTime?) : x.Date).ToArray();
        }

        [Default]
        [Guid("06afed1a-78a6-4e02-a1f4-e8db1e6fe848")]
        [Guide("The type of transaction that affected the inventory quantity.")]
        [Guide("Common transaction types include *Sales Invoice*, *Purchase Invoice*, *Inventory Write-off*, *Production Order*, and *Inventory Transfer*.")]
        [Guide("This column helps you identify how inventory ownership changes through different business activities and quickly understand the nature of each quantity movement.")]
        public string[] GetTransaction(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Transaction?.GetTransactionName()).ToArray();            
        }

        [Default]
        [PaddedSorting, Short]
        [Guide("The unique *reference number* assigned to each transaction.")]
        [Guid("a129e093-e767-4ab6-8a6f-611805023e65")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Transaction?.GetReference()).ToArray();
        }

        [Default]
        [Guid("5715f798-c573-4110-b622-8bd7d2d84bdf")]
        [Guide("The name of the inventory item being tracked.")]
        public NamedObject[] GetInventoryItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryItem).ToArray();
        }

        [Guid("ae37d3a8-27fe-4eb7-8dd1-b93a67cd07d3")]
        [Guide("The *bank account* or *cash account* associated with the transaction, if applicable.")]
        public NamedObject[] GetBankOrCashAccount(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.BankAccount).ToArray();
        }

        [Guid("e1d8b3b4-c7f4-40a2-9019-3f361cc06285")]
        [Guide("The *customer* involved in the transaction, typically shown for sales-related transactions.")]
        public NamedObject[] GetCustomer(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Customer).ToArray();
        }

        [Guid("86aade86-b41d-4351-9cdd-bb559cab8f4a")]
        [Guide("The *supplier* involved in the transaction, typically shown for purchase-related transactions.")]
        public NamedObject[] GetSupplier(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Supplier).ToArray();
        }

        [Guid("a3a63279-d381-4587-be59-583bb029e35c")]
        [Guide("A description or explanation of the overall transaction.")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Guid("89328d8b-2289-4b3f-a082-3e08eae69cde")]
        [Guide("Detailed information about the specific line item within the transaction that affected this inventory item.")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine?.GetLineDescription(x.Transaction)).ToArray();
        }

        [Default]
        [Right, Sum, Bold, RunningTotal]
        [Guid("97cde91a-90bf-4eef-bfd4-9769a41bf295")]
        [Guide("The quantity change for this transaction.")]
        [Guide("Positive numbers indicate increases in *quantity owned* (purchases, returns from customers), while negative numbers indicate decreases (sales, write-offs).")]
        [Guide("The *running total* shows the cumulative quantity owned after each transaction.")]
        public decimal[] GetQtyOwned(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty.Value).ToArray();
        }
    }
}
