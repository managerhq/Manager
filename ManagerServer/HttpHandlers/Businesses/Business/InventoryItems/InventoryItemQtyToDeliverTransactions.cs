using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using ManagerServer.Query.GeneralLedger;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryItems
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryItems), nameof(Strings.QtyToDeliver), nameof(Strings.Transactions))]
    [Guide("This screen displays all pending deliveries for a specific *inventory item* and *customer* combination.")]
    [Guide("It shows transactions where inventory has been committed to a customer but not yet physically delivered, helping you track outstanding delivery obligations.")]
    [Guide("The list includes *Delivery Notes*, *Sales Invoices*, and *Credit Notes* that have quantities pending delivery. Transactions are sorted by date with the most recent appearing first.")]
    [Columns]
    internal sealed class InventoryItemQtyToDeliverTransactions : NakedObjectsWithCustomFields<GeneralLedgerTransaction>
    {
        [ProtoMember(1)] public Guid InventoryItem;
        [ProtoMember(2)] public Guid Customer;

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
                .Where(x => x.Customer?.Key == Customer)
                .Where(x => x.QtyToDeliver != 0m)
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
        [Guid("268ab5a3-e6bf-4fe9-92aa-04c1bed87e93")]
        [Guide("Shows the transaction date for each pending delivery. Future dates may indicate scheduled deliveries that are not yet due.")]
        public DateTime?[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date == DateTime.MinValue ? default(DateTime?) : x.Date).ToArray();
        }

        [Default]
        [Guide("Displays the type of transaction (e.g., *Sales Invoice*, *Delivery Note*, *Credit Note*) that created the delivery obligation.")]
        [Guid("15f846b3-184e-47ac-acf3-dd15b1cd5953")]
        public string[] GetTransaction(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => ManagerServer.Globalization.Strings.GetPropertyValue(x.Transaction?.GetType().Name)).ToArray();
        }

        [Default]
        [PaddedSorting, Short]
        [Guid("2eb1ca27-5dcd-4216-ae8e-3e3f73f10c11")]
        [Guide("Shows the reference number of the transaction. Click to open the original transaction for viewing or editing.")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Transaction?.GetReference()).ToArray();
        }

        [Default]
        [Guid("561a2ce6-5c19-4a05-bd7e-6105b259cedb")]
        [Guide("Shows the *inventory item* that is pending delivery. This column may be hidden when viewing transactions for a specific item.")]
        public NamedObject[] GetInventoryItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryItem).ToArray();
        }

        [Default]
        [Guid("a6d3c609-29ee-4abe-8955-c6b91d6f6097")]
        [Guide("Shows the *customer* who is awaiting delivery of the items. This column may be hidden when viewing transactions for a specific customer.")]
        public NamedObject[] GetCustomer(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Customer).ToArray();
        }

        [Guid("7b8f148f-5853-41f6-9952-dc21c1c92f4a")]
        [Guide("Shows the overall description entered for the transaction, providing context about the sale or delivery.")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Guid("fd4dfa8a-a1df-4a2d-8f56-0793e2fefd22")]
        [Guide("Shows the specific line item description from the transaction, which may include additional details about the items to be delivered.")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine?.GetLineDescription(x.Transaction)).ToArray();
        }

        [Default]
        [Right, Sum, Bold, RunningTotal]
        [Guid("ad8cdfbf-784a-4dfa-a4d2-a96983596ca6")]
        [Guide("Displays the quantity of items still pending delivery for each transaction.")]
        [Guide("The *running total* column shows the cumulative quantity to be delivered, helping you track the total outstanding delivery obligations at any point in time.")]
        public decimal[] GetQtyToDeliver(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.QtyToDeliver).ToArray();
        }
    }
}