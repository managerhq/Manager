using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace ManagerServer.HttpHandlers.Businesses.Business.Customers
{
    [ProtoContract]
    [Title(nameof(Strings.Customers), nameof(Strings.QtyToDeliver))]
    [Guide("The *Customers - Qty to Deliver* screen displays inventory items that are pending delivery to customers.")]
    [Guide("This screen helps you track which items need to be delivered and allows you to create delivery notes efficiently.")]
    [Header("Accessing the Screen")]
    [Guide("To access the *Qty to Deliver* screen, go to the **Customers** tab.")]
    [TabScreenshot(icon: "fa-users-class", name: nameof(Strings.Customers))]
    [Guide("Then click the figure under the **Qty to Deliver** column for a specific customer.")]
    [ColumnScreenshot(name: nameof(Strings.QtyToDeliver), 55)]
    [Guide("If you do not see the *Qty to Deliver* column, you will need to enable it using the **Edit Columns** function.")]
    [Header("Creating Delivery Notes")]
    [Guide("Creating delivery notes from this screen is more efficient than creating them from scratch.")]
    [Guide("Select the inventory items with non-zero quantities that you want to deliver.")]
    [Guide("Click the **New Delivery Note** button to copy the selected items to a new delivery note.")]
    [Header("Working with Multiple Customers")]
    [Guide("You can create multiple delivery notes at once for different customers.")]
    [Guide("This is useful when you want to clear the *Qty to Deliver* figures across all customers and inventory items.")]
    [Guide("By default, the screen shows *Qty to Deliver* figures for a specific customer.")]
    [Guide("To view pending deliveries for all customers, click the **X** button next to the customer's name to remove the filter.")]
    [Guide("Then select the inventory items with non-zero quantities and click **New Delivery Note** to create delivery notes.")]
    [Guide("The system will automatically create separate delivery notes for each customer.")]
    [Header("Column Information")]
    [Guide("The screen contains the following columns:")]
    [Columns]
    internal sealed class CustomersQtyToDeliver : NakedObjectsWithCustomFields<CustomersQtyToDeliver.Item>
    {
        [ProtoMember(1)] public Guid? Customer;

        protected override void OnAfterHeader(Context context)
        {
            if (Customer.HasValue)
            {
                using (Div(@class: "bg-yellow-50 p-4 border border-t-white"))
                {
                    using (Div(@class: "flex items-center"))
                    {
                        using (Span(@class: "border-amber-100 border-2 bg-amber-100 py-2 px-4 font-semibold text-amber-700")) Write(Strings.Customer);
                        using (Span(@class: "border-amber-100 border-2 py-2 px-4 bg-white text-neutral-700")) Write(ApplicationData.Businesses.Get(Business)?.SingleOrDefault<ManagerServer.Model.Customer>(Customer.Value).GetCodeAndName());

                        var httpHandler = (CustomersQtyToDeliver)this.MemberwiseClone();
                        httpHandler.Skip = 0;
                        httpHandler.Customer = null;
                        using (A(href: httpHandler.ToUrl(), @class: "border-amber-100 border-2 bg-amber-100 py-2 px-4 text-amber-700 hover:text-amber-800")) I(@class: "fas fa-close");
                    }
                }
            }
            base.OnAfterHeader(context);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);

            var list = new List<Item>();

            list.AddRange(database.OfType<DeliveryNote>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.Customer != null)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.QtyToDeliver != 0m)
                .Select(x => new Item() { InventoryItem = x.InventoryItem, Customer = x.Customer, Qty = x.QtyToDeliver }));

            list.AddRange(database.OfType<SalesInvoice>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.Customer != null)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.QtyToDeliver != 0m)
                .Select(x => new Item() { InventoryItem = x.InventoryItem, Customer = x.Customer, Qty = x.QtyToDeliver }));

            list.AddRange(database.OfType<CreditNote>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.Customer != null)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.QtyToDeliver != 0m)
                .Select(x => new Item() { InventoryItem = x.InventoryItem, Customer = x.Customer, Qty = x.QtyToDeliver }));

            list.AddRange(database.OfType<InventoryItemStartingBalance>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.Customer != null)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.QtyToDeliver != 0m)
                .Select(x => new Item() { InventoryItem = x.InventoryItem, Customer = x.Customer, Qty = x.QtyToDeliver }));

            if (Customer.HasValue) list = list.Where(x => x.Customer.Key == Customer.Value).ToList();

            list = list.GroupBy(x => new { x.InventoryItem, x.Customer }).Select(x => new Item() { InventoryItem = x.Key.InventoryItem, Customer = x.Key.Customer, Qty = x.Sum(y => y.Qty) }).ToList();
            list = list.OrderByDescending(x => x.Qty != 0m).ThenBy(x => x.Customer.GetCodeAndName()).ThenByDescending(x => Math.Abs(x.Qty)).ToList();

            context.Set<Array>(list.ToArray());

            if (list.Any()) context.Set(new BatchOperation() { Name = Strings.NewDeliveryNote });

            base.InnerGet4(context);
        }

        public override Tuple<string, byte[]>[] GetBatchOperation(Item[] rows)
        {
            return Serialize(nameof(CustomersQtyToDeliver), rows.Select(x => x.Qty != 0m ? new Tuple<Guid, Guid, Guid?, decimal>(x.InventoryItem.Key, x.Customer.Key, x.InventoryLocation?.Key, x.Qty) : null).ToArray());
        }

        [Default]
        [Guid("ebc28fe5-c221-44dc-856e-0984039f22df")]
        [Guide("The customer to whom goods are pending delivery.")]
        [Guide("Displays both the customer code and name for easy identification.")]
        [Guide("When viewing all customers, this helps identify which customers are awaiting deliveries.")]
        public NamedObject[] GetCustomer(Item[] rows)
        {
            return rows.Select(x => x.Customer).ToArray();
        }

        [Default]
        [Guid("c87fce65-2b7e-4c33-9f8f-ca06945f6169")]
        [Guide("The *inventory item* that is pending delivery.")]
        [Guide("Displays both the item code and name for easy identification.")]
        public NamedObject[] GetInventoryItem(Item[] rows)
        {
            return rows.Select(x => x.InventoryItem).ToArray();
        }

        [Default, Right, Sum, Bold]
        [Guid("9b3f04ca-0e41-489f-8229-dfe2d66fa2ad")]
        [Guide("The quantity of each *inventory item* that is pending delivery.")]
        [Guide("Click the quantity figure to view the detailed delivery ledger for that specific customer and inventory item combination.")]
        [Guide("The total quantity to deliver is shown at the bottom of the column.")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyToDeliver(Item[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(x.Qty, new InventoryItems.InventoryItemQtyToDeliverTransactions() { Business = Business, Customer = x.Customer.Key, InventoryItem = x.InventoryItem.Key, Referrer = referrer })).ToArray();
        }

        public sealed class Item
        {
            public Customer Customer;
            public InventoryItem InventoryItem;
            public CustomInventoryLocation InventoryLocation;
            public decimal Qty;
        }

        protected override async Task InnerPost()
        {
            var rows = await Deserialize<Tuple<Guid, Guid, Guid?, decimal>>(nameof(CustomersQtyToDeliver));
            if (rows != null)
            {
                if (rows.Select(x => x.Item2).Distinct().Count() > 1)
                {
                    var list = new List<DeliveryNote>();
                    foreach (var e in rows.GroupBy(x => x.Item2))
                    {
                        list.Add(new DeliveryNote()
                        {
                            DeliveryDate = DateTime.Today,
                            Customer = e.Key,
                            Lines = e.Select(x => new DeliveryNote.Line()
                            {
                                Item = x.Item1,
                                Qty = x.Item4
                            }).ToArray()
                        });
                    }

                    ApplicationData.Businesses.Process(Business, list.ToArray(), GetUserName());
                    Response.Redirect(this.ToUrl());
                    return;
                }
                else
                {
                    using (var ms = new MemoryStream())
                    {
                        var deliveryNote = new DeliveryNote()
                        {
                            DeliveryDate = DateTime.Today,
                            Customer = Customer,
                            Lines = rows.Select(x => new DeliveryNote.Line()
                            {
                                Item = x.Item1,
                                Qty = x.Item4
                            }).ToArray()
                        };

                        ProtoBuf.Serializer.Serialize(ms, deliveryNote);

                        var referrer = this.ToUrl();
                        Response.Redirect(new DeliveryNotes.DeliveryNoteForm() { Business = Business, Data2 = ms.ToArray(), Referrer = referrer }.ToUrl());
                        return;
                    }
                }
            }

            await base.InnerPost();
        }
    }
}