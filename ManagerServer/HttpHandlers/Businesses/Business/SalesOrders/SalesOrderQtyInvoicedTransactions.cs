using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using ManagerServer.Query.GeneralLedger;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesOrders
{
    [ProtoContract]
    [Title(nameof(Strings.SalesOrders), nameof(Strings.QtyInvoiced), nameof(Strings.Transactions))]
    [Guide("The Quantity Invoiced Transactions screen displays all sales invoices that have been issued against a specific sales order.")]
    [Guide("This report helps you track which inventory items from the sales order have been invoiced to the customer and in what quantities.")]
    [Guide("Each row represents a line item from a sales invoice that references this sales order, showing the date, reference number, customer, and quantity invoiced.")]
    [Columns]
    internal sealed class SalesOrderQtyInvoicedTransactions : NakedObjectsWithCustomFields<GeneralLedgerTransaction>
    {
        [ProtoMember(1)] public Guid InventoryItem;
        [ProtoMember(2)] public Guid SalesOrder;

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);

            var list = new List<GeneralLedgerTransaction>();

            list.AddRange(database.OfType<SalesInvoice>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem?.Key == InventoryItem)
                .Where(x => x.SalesOrder?.Key == SalesOrder)
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
        [Guide("The date when the sales invoice containing this item was issued.")]
        public DateTime?[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date == DateTime.MinValue ? default(DateTime?) : x.Date).ToArray();
        }

        [Default]
        [Guide("The type of transaction (typically *Sales Invoice*).")]
        [Guid("282b11e8-3888-49cc-95c1-ec73095336b0")]
        public string[] GetTransaction(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => ManagerServer.Globalization.Strings.GetPropertyValue(x.Transaction?.GetType().Name)).ToArray();
        }

        [Default]
        [PaddedSorting, Short]
        [Guid("075bd9b9-4dc8-4726-bf9e-03dd0ef80003")]
        [Guide("The reference number of the sales invoice. Click on the reference to view or edit the invoice.")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Transaction?.GetReference()).ToArray();
        }

        [Default]
        [Guid("b8dff249-3675-4bde-8c19-67dc3e6fa09d")]
        [Guide("The specific inventory item from the sales order that was invoiced.")]
        public NamedObject[] GetInventoryItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryItem).ToArray();
        }

        [Default]
        [Guid("a26221d5-32ad-4409-be8d-ac285e671aa1")]
        [Guide("The customer to whom the sales invoice was issued.")]
        public NamedObject[] GetCustomer(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Customer).ToArray();
        }

        [Guid("2a888d02-c618-45a2-b72b-1820b6e7988c")]
        [Guide("The description from the sales invoice header.")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Guid("68e28e41-b8e4-41fc-a535-f86e964c4c61")]
        [Guide("The description of the specific line item from the sales invoice.")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine?.GetLineDescription(x.Transaction)).ToArray();
        }

        [Default]
        [Right, Sum, Bold]
        [Guid("3d84629a-e9f3-497f-8853-37779704c747")]
        [Guide("The quantity of the inventory item that was invoiced. The total at the bottom shows the cumulative quantity invoiced for this item against the sales order.")]
        public decimal[] GetQtyInvoiced(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.QtyInvoiced).ToArray();
        }
    }
}