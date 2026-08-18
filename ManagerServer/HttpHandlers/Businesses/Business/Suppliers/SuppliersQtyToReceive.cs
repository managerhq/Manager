using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Suppliers
{
    [ProtoContract]
    [Title(nameof(Strings.Suppliers), nameof(Strings.QtyToReceive))]
    [Guide("The *Suppliers - Qty to Receive* screen shows inventory items that are pending to be received from specific suppliers.")]
    [Guide("This screen helps you track outstanding deliveries and create goods receipts for incoming inventory.")]
    [Header("Accessing the Screen")]
    [Guide("To access the *Qty to Receive* screen, go to the **Suppliers** tab.")]
    [TabScreenshot(icon: "fa-city", name: nameof(Strings.Suppliers))]
    [Guide("Then click the figure under the **Qty to Receive** column.")]
    [ColumnScreenshot(name: nameof(Strings.QtyToReceive), value: 43)]
    [Guide("If you do not see the *Qty to Receive* column, you will need to enable it using the **Edit Columns** function.")]
    [LinkGuide("For more information, see:", typeof(NakedObjectsWithEditColumns<>))]
    [Header("Creating Goods Receipts")]
    [Guide("Copying inventory items with non-zero quantities to a new goods receipt is easier than creating a goods receipt from scratch.")]
    [Guide("Select the inventory items with quantities other than zero.")]
    [Guide("Click the **New Goods Receipt** button to copy them to a new goods receipt.")]
    [Header("Working with Multiple Suppliers")]
    [Guide("You can create multiple goods receipts at once for many suppliers, which is useful when you want to clear the *Qty to Receive* figure across all suppliers and inventory items.")]
    [Guide("By default, the screen shows *Qty to Receive* figures for a specific supplier. To show figures for all suppliers, remove the supplier filter by clicking the **X** button next to their name.")]
    [Guide("Then continue as normal: select the inventory items with quantities other than zero and click the **New Goods Receipt** button.")]
    [Header("Screen Columns")]
    [Guide("The screen contains the following columns:")]
    [Columns]
    internal sealed class SuppliersQtyToReceive : NakedObjectsWithCustomFields<SuppliersQtyToReceive.Item>
    {
        [ProtoMember(1)] public Guid? Supplier;

        protected override void OnAfterHeader(Context context)
        {
            if (Supplier.HasValue)
            {
                using (Div(@class: "bg-yellow-50 p-4 border border-t-white"))
                {
                    using (Div(@class: "flex items-center"))
                    {
                        using (Span(@class: "border-amber-100 border-2 bg-amber-100 py-2 px-4 font-semibold text-amber-700")) Write(Strings.Supplier);
                        using (Span(@class: "border-amber-100 border-2 py-2 px-4 bg-white text-neutral-700")) Write(ApplicationData.Businesses.Get(Business)?.SingleOrDefault<ManagerServer.Model.Supplier>(Supplier.Value).GetCodeAndName());

                        var httpHandler = (SuppliersQtyToReceive)this.MemberwiseClone();
                        httpHandler.Skip = 0;
                        httpHandler.Supplier = null;
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

            list.AddRange(database.OfType<GoodsReceipt>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.Supplier != null)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.QtyToReceive != 0m)
                .Select(x => new Item() { InventoryItem = x.InventoryItem, Supplier = x.Supplier, Qty = x.QtyToReceive }));

            list.AddRange(database.OfType<PurchaseInvoice>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.Supplier != null)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.QtyToReceive != 0m)
                .Select(x => new Item() { InventoryItem = x.InventoryItem, Supplier = x.Supplier, Qty = x.QtyToReceive }));

            list.AddRange(database.OfType<DebitNote>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.Supplier != null)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.QtyToReceive != 0m)
                .Select(x => new Item() { InventoryItem = x.InventoryItem, Supplier = x.Supplier, Qty = x.QtyToReceive }));

            list.AddRange(database.OfType<InventoryItemStartingBalance>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.Supplier != null)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.QtyToReceive != 0m)
                .Select(x => new Item() { InventoryItem = x.InventoryItem, Supplier = x.Supplier, Qty = x.QtyToReceive }));

            if (Supplier.HasValue) list = list.Where(x => x.Supplier.Key == Supplier.Value).ToList();

            list = list.GroupBy(x => new { x.InventoryItem, x.Supplier }).Select(x => new Item() { InventoryItem = x.Key.InventoryItem, Supplier = x.Key.Supplier, Qty = x.Sum(y => y.Qty) }).ToList();
            list = list.OrderByDescending(x => x.Qty != 0m).ThenBy(x => x.Supplier.GetCodeAndName()).ThenByDescending(x => Math.Abs(x.Qty)).ToList();

            context.Set<Array>(list.ToArray());

            if (list.Any()) context.Set(new BatchOperation() { Name = Strings.NewGoodsReceipt });

            base.InnerGet4(context);
        }

        public override Tuple<string, byte[]>[] GetBatchOperation(Item[] rows)
        {
            return Serialize(nameof(SuppliersQtyToReceive), rows.Select(x => x.Qty != 0m ? new Tuple<Guid, Guid, Guid?, decimal>(x.InventoryItem.Key, x.Supplier.Key, x.InventoryLocation?.Key, x.Qty) : null).ToArray());
        }

        [Default]
        [Guid("177a1867-d2b6-44c3-990c-decaf8cd8651")]
        [Guide("The supplier from whom goods are pending to be received.")]
        [Guide("Shows the supplier's code and name for easy identification.")]
        [Guide("When viewing all suppliers, this column helps you see which suppliers have outstanding deliveries.")]
        public NamedObject[] GetSupplier(Item[] rows)
        {
            return rows.Select(x => x.Supplier).ToArray();
        }

        [Default]
        [Guide("The name of the inventory item that is pending to be received.")]
        [Guid("c87fce65-2b7e-4c33-9f8f-ca06945f6169")]
        public NamedObject[] GetInventoryItem(Item[] rows)
        {
            return rows.Select(x => x.InventoryItem).ToArray();
        }

        [Default, Right, Sum, Bold]
        [Guid("9b3f04ca-0e41-489f-8229-dfe2d66fa2ad")]
        [Guide("The balance indicates the quantity to be received from the supplier based on purchase invoices, debit notes, and past goods receipts.")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyToReceive(Item[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(x.Qty, new InventoryItems.InventoryItemQtyToReceiveTransactions() { Business = Business, Supplier = x.Supplier.Key, InventoryItem = x.InventoryItem.Key, Referrer = referrer })).ToArray();
        }

        public sealed class Item
        {
            public Supplier Supplier;
            public InventoryItem InventoryItem;
            public CustomInventoryLocation InventoryLocation;
            public decimal Qty;
        }

        protected override async Task InnerPost()
        {
            var rows = await Deserialize<Tuple<Guid, Guid, Guid?, decimal>>(nameof(SuppliersQtyToReceive));
            if (rows != null)
            {
                if (rows.Select(x => x.Item2).Distinct().Count() > 1)
                {
                    var list = new List<GoodsReceipt>();
                    foreach (var e in rows.GroupBy(x => x.Item2))
                    {
                        list.Add(new GoodsReceipt()
                        {
                            Date = DateTime.Today,
                            Supplier = e.Key,
                            Lines = e.Select(x => new GoodsReceipt.Line()
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
                        var goodsReceipt = new GoodsReceipt()
                        {
                            Date = DateTime.Today,
                            Supplier = Supplier.Value,
                            Lines = rows.Select(x => new GoodsReceipt.Line()
                            {
                                Item = x.Item1,
                                Qty = x.Item4
                            }).ToArray()
                        };

                        ProtoBuf.Serializer.Serialize(ms, goodsReceipt);

                        var referrer = this.ToUrl();
                        Response.Redirect(new GoodsReceipts.GoodsReceiptForm() { Business = Business, Data2 = ms.ToArray(), Referrer = referrer }.ToUrl());
                        return;
                    }
                }
            }

            await base.InnerPost();
        }
    }
}