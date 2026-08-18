using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using Newtonsoft.Json;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryItems
{
    [ProtoContract]
    [Guid("1f00ae47-2275-45b8-b82e-80df8e868054")]
    [Title(nameof(Strings.InventoryItems), nameof(Strings.QtyOnOrder))]
    [Guide("The *Inventory Items - Qty on Order* screen displays a list of purchase orders made to suppliers for a selected inventory item.")]
    [Guide("This screen shows all outstanding quantities that have been ordered but not yet fully received or invoiced.")]
    [Guide("To open this screen, navigate to the **Inventory Items** tab.")]
    [TabScreenshot("fa-inventory", nameof(Strings.InventoryItems))]
    [Guide("Next, click on the number in the **Qty on Order** column:")]
    [ColumnScreenshot(nameof(Strings.QtyOnOrder), 8)]
    [Guide("The *Inventory Items - Qty on Order* tab includes several columns to track the status of your purchase orders.")]
    [Columns]
    [Guide("To customize which columns appear, click the **Edit Columns** button.")]
    internal sealed class InventoryItemQtyOnOrder : NakedObjectsWithCustomFields<PurchaseOrders.PurchaseOrders.PurchaseOrderQty>
    {
        [ProtoMember(1), JsonProperty("inventoryItem")] public Guid InventoryItem;

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var activePurchaseOrders = database.OfType<PurchaseOrder>().Where(x => !x.Cancelled).Select(x => x.Key).ToArray();

            context.Set<Array>(PurchaseOrders.PurchaseOrders.GetPurchaseOrderQuantities(database, purchaseOrders: activePurchaseOrders, inventoryItems: [InventoryItem]));

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(PurchaseOrders.PurchaseOrders.PurchaseOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new PurchaseOrders.PurchaseOrderForm() { Business = Business, Key = x.PurchaseOrder.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(PurchaseOrders.PurchaseOrders.PurchaseOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new PurchaseOrders.PurchaseOrderView() { Business = Business, Key = x.PurchaseOrder.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guide("The date when the purchase order was issued to the supplier.")]
        [Guide("This helps track how long orders have been outstanding and identify any overdue deliveries.")]
        [Guide("Orders are typically sorted with the most recent dates first.")]
        public DateTime[] GetDate(PurchaseOrders.PurchaseOrders.PurchaseOrderQty[] rows)
        {
            return rows.Select(x => x.PurchaseOrder.Date).ToArray();
        }

        [Default]
        [PaddedSorting]
        [Guide("The purchase order's reference number.")]
        [Guide("Click on the reference number to view or edit the full purchase order details.")]
        public string[] GetPurchaseOrder(PurchaseOrders.PurchaseOrders.PurchaseOrderQty[] rows)
        {
            return rows.Select(x => x.PurchaseOrder.GetTransactionName()).ToArray();
        }

        [Default]
        [Guide("The supplier to whom the purchase order was issued.")]
        [Guide("This shows which supplier is responsible for delivering the outstanding quantities.")]
        public NamedObject[] GetSupplier(PurchaseOrders.PurchaseOrders.PurchaseOrderQty[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Supplier>(x.PurchaseOrder.Supplier)).ToArray();
        }

        [Default]
        [Guide("The inventory item being ordered.")]
        [Guide("This is the specific inventory item for which you are viewing outstanding purchase orders.")]
        public NamedObject[] GetInventoryItem(PurchaseOrders.PurchaseOrders.PurchaseOrderQty[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.InventoryItem>(InventoryItem)).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guide("The total quantity ordered on the purchase order.")]
        [Guide("This is the original quantity requested from the supplier.")]
        public decimal[] GetQtyOrdered(PurchaseOrders.PurchaseOrders.PurchaseOrderQty[] rows)
        {
            return rows.Select(x => x.QtyOrdered).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guide("The quantity that has been received and recorded in *Goods Receipts*.")]
        [Guide("Click on the number to see the list of goods receipts for this purchase order.")]
        [Guide("This helps track partial deliveries and what has already been added to inventory.")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyReceived(PurchaseOrders.PurchaseOrders.PurchaseOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(
                x.QtyReceived,
                new PurchaseOrders.PurchaseOrderQtyDeliveredTransactions() { Business = Business, PurchaseOrder = x.PurchaseOrder.Key, InventoryItem = x.InventoryItem.Key, Referrer = referrer }
            )).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guide("The quantity that has been invoiced by the supplier in *Purchase Invoices*.")]
        [Guide("Click on the number to see the list of purchase invoices for this purchase order.")]
        [Guide("This quantity may differ from the received quantity if goods are received before invoicing or vice versa.")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyInvoiced(PurchaseOrders.PurchaseOrders.PurchaseOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(
                x.QtyInvoiced,
                new PurchaseOrders.PurchaseOrderQtyInvoicedTransactions() { Business = Business, PurchaseOrder = x.PurchaseOrder.Key, InventoryItem = x.InventoryItem.Key, Referrer = referrer }
            )).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guide("The outstanding quantity still on order.")]
        [Guide("This is calculated as the quantity ordered minus the greater of quantity received or quantity invoiced.")]
        [Guide("When this reaches zero, the purchase order line is considered complete.")]
        public decimal[] GetQtyOnOrder(PurchaseOrders.PurchaseOrders.PurchaseOrderQty[] rows)
        {
            return rows.Select(x => x.QtyOnOrder).ToArray();
        }
    }
}