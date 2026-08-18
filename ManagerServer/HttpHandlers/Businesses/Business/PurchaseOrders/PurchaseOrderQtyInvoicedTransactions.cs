using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using ManagerServer.Query.GeneralLedger;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseOrders
{
    [ProtoContract]
    [Title(nameof(Strings.PurchaseOrders), nameof(Strings.QtyInvoiced), nameof(Strings.Transactions))]
    [Guide("The **Quantity Invoiced Transactions** screen displays all purchase invoices linked to a specific purchase order.")]
    [Guide("This screen helps you track which *inventory items* have been invoiced by the supplier and match them against the original *purchase order*.")]
    [Columns]
    internal sealed class PurchaseOrderQtyInvoicedTransactions : NakedObjectsWithCustomFields<GeneralLedgerTransaction>
    {
        [ProtoMember(1)] public Guid InventoryItem;
        [ProtoMember(2)] public Guid PurchaseOrder;

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);

            var list = new List<GeneralLedgerTransaction>();

            list.AddRange(database.OfType<PurchaseInvoice>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.InventoryItem?.Key == InventoryItem)
                .Where(x => x.PurchaseOrder?.Key == PurchaseOrder)
                .Where(x => x.QtyInvoiced != 0m));

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
        [Guid("6fea1546-06fd-429e-8dbd-1acc26fa1e25")]
        [Guide("Shows the date when the *purchase invoice* was issued.")]
        public DateTime?[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date == DateTime.MinValue ? default(DateTime?) : x.Date).ToArray();
        }

        [Default]
        [Guide("Displays the *transaction type* name.")]
        [Guid("282b11e8-3888-49cc-95c1-ec73095336b0")]
        public string[] GetTransaction(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => ManagerServer.Globalization.Strings.GetPropertyValue(x.Transaction?.GetType().Name)).ToArray();
        }

        [Default]
        [PaddedSorting, Short]
        [Guid("075bd9b9-4dc8-4726-bf9e-03dd0ef80003")]
        [Guide("Shows the reference number of the *purchase invoice*.")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Transaction?.GetReference()).ToArray();
        }

        [Default]
        [Guid("b8dff249-3675-4bde-8c19-67dc3e6fa09d")]
        [Guide("Displays the *inventory item* that was invoiced.")]
        public NamedObject[] GetInventoryItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryItem).ToArray();
        }

        [Default]
        [Guid("a26221d5-32ad-4409-be8d-ac285e671aa1")]
        [Guide("Displays the *supplier* who issued the *purchase invoice*.")]
        public NamedObject[] GetSupplier(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Supplier).ToArray();
        }

        [Guid("2a888d02-c618-45a2-b72b-1820b6e7988c")]
        [Guide("Displays the description entered on the *purchase invoice*.")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Guid("68e28e41-b8e4-41fc-a535-f86e964c4c61")]
        [Guide("Displays the description of the specific line item from the *purchase invoice*.")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine?.GetLineDescription(x.Transaction)).ToArray();
        }

        [Default]
        [Right, Sum, Bold]
        [Guid("3d84629a-e9f3-497f-8853-37779704c747")]
        [Guide("Displays the quantity of items that have been invoiced against this *purchase order*.")]
        public decimal[] GetQtyInvoiced(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.QtyInvoiced).ToArray();
        }
    }
}