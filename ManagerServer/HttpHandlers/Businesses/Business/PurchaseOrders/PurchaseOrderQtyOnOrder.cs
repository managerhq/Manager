using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using Newtonsoft.Json;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseOrders
{
    [ProtoContract]
    [Title(nameof(Strings.PurchaseOrders), nameof(Strings.QtyOnOrder))]
    [Guide("The **Quantity on Order** screen displays all *inventory items* included in a specific *purchase order*.")]
    [Guide("This screen helps you monitor the fulfillment status of each ordered item by tracking quantities at different stages of the procurement process.")]
    [Guide("Use this screen to see at a glance which items have been fully delivered and which are still pending.")]
    [Columns]
    internal sealed class PurchaseOrderQtyOnOrder : NakedObjectsWithCustomFields<PurchaseOrders.PurchaseOrderQty>
    {
        [ProtoMember(1), JsonProperty("purchaseOrder")] public Guid PurchaseOrder;

        protected override void InnerGet4(Context context)
        {
            context.Set<Array>(PurchaseOrders.GetPurchaseOrderQuantities(ApplicationData.Businesses.Get(Business), purchaseOrders: [PurchaseOrder]));

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(PurchaseOrders.PurchaseOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new InventoryItems.InventoryItemForm() { Business = Business, Key = x.InventoryItem.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(PurchaseOrders.PurchaseOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new InventoryItems.InventoryItemView() { Business = Business, Key = x.InventoryItem.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [Guide("Displays the *inventory item* that was ordered on this *purchase order*.")]
        public NamedObject[] GetInventoryItem(PurchaseOrders.PurchaseOrderQty[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.InventoryItem).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guide("Shows the total quantity of this item that was originally ordered on the *purchase order*.")]
        public decimal[] GetQtyOrdered(PurchaseOrders.PurchaseOrderQty[] rows)
        {
            return rows.Select(x => x.QtyOrdered).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guide("Shows the quantity that has been received through *goods receipts*. Click the quantity to view the detailed receipt transactions.")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyReceived(PurchaseOrders.PurchaseOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(
                x.QtyReceived,
                new PurchaseOrderQtyDeliveredTransactions() { Business = Business, PurchaseOrder = x.PurchaseOrder.Key, InventoryItem = x.InventoryItem.Key, Referrer = referrer }
            )).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guide("Shows the quantity that has been invoiced through *purchase invoices*. Click the quantity to view the detailed invoice transactions.")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyInvoiced(PurchaseOrders.PurchaseOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(
                x.QtyInvoiced,
                new PurchaseOrderQtyInvoicedTransactions() { Business = Business, PurchaseOrder = x.PurchaseOrder.Key, InventoryItem = x.InventoryItem.Key, Referrer = referrer }
            )).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guide("Shows the remaining quantity that is still on order and has not yet been received. This is calculated as the ordered quantity minus the received quantity.")]
        public decimal[] GetQtyReserved(PurchaseOrders.PurchaseOrderQty[] rows)
        {
            return rows.Select(x => x.QtyOnOrder).ToArray();
        }
    }
}