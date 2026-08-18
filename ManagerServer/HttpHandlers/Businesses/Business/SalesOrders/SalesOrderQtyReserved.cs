using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using Newtonsoft.Json;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesOrders
{
    [ProtoContract]
    [Title(nameof(Strings.SalesOrders), nameof(Strings.QtyReserved))]
    [Guide("The **Quantity Reserved** screen shows inventory items reserved on a specific sales order.")]
    [Guide("This screen helps you track the fulfillment status of each item on the sales order by displaying ordered quantities, delivered quantities, and remaining quantities to be delivered.")]
    [Columns]
    internal sealed class SalesOrderQtyReserved : NakedObjectsWithCustomFields<SalesOrders.SalesOrderQty>
    {
        [ProtoMember(1), JsonProperty("salesOrder")] public Guid SalesOrder;

        protected override void InnerGet4(Context context)
        {
            context.Set<Array>(SalesOrders.GetSalesOrderQuantities(ApplicationData.Businesses.Get(Business), salesOrders: [ SalesOrder ]));

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(SalesOrders.SalesOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new InventoryItems.InventoryItemForm() { Business = Business, Key = x.InventoryItem.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(SalesOrders.SalesOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new InventoryItems.InventoryItemView() { Business = Business, Key = x.InventoryItem.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [Guide("Displays the *inventory item* that was ordered on this sales order.")]
        public NamedObject[] GetInventoryItem(SalesOrders.SalesOrderQty[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.InventoryItem).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guide("Shows the total quantity ordered by the customer for this item.")]
        public decimal[] GetQtyOrdered(SalesOrders.SalesOrderQty[] rows)
        {
            return rows.Select(x => x.QtyOrdered).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guide("Shows the quantity already delivered to the customer. Click the value to view the delivery transactions for this item.")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyDelivered(SalesOrders.SalesOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(
                x.QtyDelivered,
                new SalesOrderQtyDeliveredTransactions() { Business = Business, SalesOrder = x.SalesOrder.Key, InventoryItem = x.InventoryItem.Key, Referrer = referrer }
            )).ToArray();
        }

        [Default]
        [Right, Sum]
        [Guide("Shows the quantity that has been invoiced to the customer. Click the value to view the sales invoice transactions for this item.")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyInvoiced(SalesOrders.SalesOrderQty[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(
                x.QtyInvoiced,
                new SalesOrderQtyInvoicedTransactions() { Business = Business, SalesOrder = x.SalesOrder.Key, InventoryItem = x.InventoryItem.Key, Referrer = referrer }
            )).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guide("Shows the remaining quantity reserved for the customer. This is the quantity still to be delivered to fulfill the sales order.")]
        public decimal[] GetQtyReserved(SalesOrders.SalesOrderQty[] rows)
        {
            return rows.Select(x => x.QtyReserved).ToArray();
        }
    }
}