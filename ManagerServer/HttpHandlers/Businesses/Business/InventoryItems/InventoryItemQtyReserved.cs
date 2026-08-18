using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using Newtonsoft.Json;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryItems
{
    [ProtoContract]
    [Guid("d3812670-7f80-41df-bca1-8d3519a76c3d")]
    [Title(nameof(Strings.InventoryItems), nameof(Strings.QtyReserved))]
    [Guide("The **Inventory Items - Qty Reserved** screen displays a list of *sales orders* for a specific inventory item showing quantities that have been ordered but not yet delivered or invoiced.")]
    [Guide("*Reserved quantities* represent items allocated to sales orders that are awaiting delivery. This helps you track which items are committed to customers but have not yet been fulfilled.")]
    [Header("Accessing the Reserved Quantities Screen")]
    [Guide("To access this screen, navigate to the **Inventory Items** tab.")]
    [TabScreenshot("fa-inventory", nameof(Strings.InventoryItems))]
    [Guide("Next, click on the number in the **Qty Reserved** column:")]
    [ColumnScreenshot(nameof(Strings.QtyReserved), 5)]
    [Header("Understanding the Columns")]
    [Guide("The **Inventory Items - Qty Reserved** screen displays several columns showing order details and quantities. These columns help you track the status of each sales order and its associated quantities.")]
    [Columns]
    [Guide("Click on the **Edit Columns** button to select and customize the visible columns according to your needs.")]
    internal sealed class InventoryItemQtyReserved : NakedObjectsWithCustomFields<SalesOrders.SalesOrders.SalesOrderQty>
    {
        [ProtoMember(1), JsonProperty("inventoryItem")] public Guid InventoryItem;

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var activeSalesOrders = database.OfType<SalesOrder>().Where(x => !x.Cancelled).Select(x => x.Key).ToArray();

            context.Set<Array>(SalesOrders.SalesOrders.GetSalesOrderQuantities(database, salesOrders: activeSalesOrders, inventoryItems: [ InventoryItem ]));

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(SalesOrders.SalesOrders.SalesOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new SalesOrders.SalesOrderForm() { Business = Business, Key = x.SalesOrder.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(SalesOrders.SalesOrders.SalesOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new SalesOrders.SalesOrderView() { Business = Business, Key = x.SalesOrder.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        public NamedObject[] GetInventoryItem(SalesOrders.SalesOrders.SalesOrderQty[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.InventoryItem).ToArray();
        }

        [Default]
        [Right, Sum]
        public decimal[] GetQtyOrdered(SalesOrders.SalesOrders.SalesOrderQty[] rows)
        {
            return rows.Select(x => x.QtyOrdered).ToArray();
        }

        [Default]
        [Right, Sum]
        public Tuple<decimal, BusinessTemplate>[] GetQtyDelivered(SalesOrders.SalesOrders.SalesOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(
                x.QtyDelivered,
                new SalesOrders.SalesOrderQtyDeliveredTransactions() { Business = Business, SalesOrder = x.SalesOrder.Key, InventoryItem = x.InventoryItem.Key, Referrer = referrer }
            )).ToArray();
        }

        [Default]
        [Right, Sum]
        public Tuple<decimal, BusinessTemplate>[] GetQtyInvoiced(SalesOrders.SalesOrders.SalesOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(
                x.QtyInvoiced,
                new SalesOrders.SalesOrderQtyInvoicedTransactions() { Business = Business, SalesOrder = x.SalesOrder.Key, InventoryItem = x.InventoryItem.Key, Referrer = referrer }
            )).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        public decimal[] GetQtyReserved(SalesOrders.SalesOrders.SalesOrderQty[] rows)
        {
            return rows.Select(x => x.QtyReserved).ToArray();
        }
    }
}