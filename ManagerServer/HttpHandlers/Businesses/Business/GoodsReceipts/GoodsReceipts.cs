using System.Linq;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.GoodsReceipts
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("1c062a34-6cd7-4a0e-83b0-426e250dfd25")]
    [Title(nameof(Strings.GoodsReceipts))]
    [Guide("The **Goods Receipts** tab helps businesses track the arrival of purchased goods from suppliers.")]
    [Guide("This feature supports inventory management by allowing you to record goods when they arrive, rather than waiting for the supplier's invoice.")]
    [Guide("Recording goods receipts immediately improves the accuracy of your *inventory levels* and helps you track what has been delivered versus what has been invoiced.")]
    [TabScreenshot("fa-truck-loading", nameof(Strings.GoodsReceipts))]
    [Guide("To create a new goods receipt, click the **New Goods Receipt** button.")]
    [HeroButtonScreenshot(nameof(Strings.GoodsReceipts), nameof(Strings.NewGoodsReceipt))]
    [Guide("The **Goods Receipts** tab displays the following columns:")]
    [Columns]
    internal sealed class GoodsReceipts : NakedObjectsWithAutomaticRows<ManagerServer.Model.GoodsReceipt>
    {
        [ProtoMember(1)] public Guid? Supplier;

        protected override GoodsReceipt[] OnGetRows(GoodsReceipt[] rows)
        {
            if (Supplier.HasValue) rows = rows.Where(x => x.Supplier == Supplier).ToArray();
            return rows;
        }

        [Default]
        [Center, MinWidth]
        [WarnIfFutureDate]
        [WhitespaceNoWrap]
        [Guid("f82626a7-9ac2-4bef-8549-97d0bd6e4021")]
        [Guide("The date when the goods were received from the supplier. This date is important for *inventory tracking* and determines when items become available in stock.")]
        public DateTime[] GetDate(GoodsReceipt[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [WarnIfNotUnique]
        [Guid("36131f7e-1566-4c9a-9ecd-1d02e82110a9")]
        [Guide("A unique reference number for the goods receipt. This number helps you identify and track specific deliveries from suppliers.")]
        public string[] GetReference(GoodsReceipt[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Guid("15bf5a0f-d8f6-4e87-bfa5-0dadc288f1f9")]
        [Guide("The *purchase order* number that corresponds to this goods receipt. This links the received goods to the original purchase order placed with the supplier.")]
        public string[] GetOrderNumber(GoodsReceipt[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<PurchaseOrder>(x.PurchaseOrder)?.Reference).ToArray();
        }

        [Guid("061b337f-b8a1-4389-9103-aaab9e33a238")]
        [Guide("The *purchase invoice* number associated with this goods receipt. This shows which supplier invoice has been entered for the received goods.")]
        public string[] GetInvoiceNumber(GoodsReceipt[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<PurchaseInvoice>(x.PurchaseInvoice)?.Reference).ToArray();
        }

        [Default]
        [Guid("8675a530-0c4b-482c-8a2b-a8eff9b262f8")]
        [Guide("The supplier who delivered the goods. This identifies which supplier the goods were received from.")]
        public string[] GetSupplier(GoodsReceipt[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Supplier>(x.Supplier)?.GetCodeAndName()).ToArray();
        }

        [Guid("dbb0ce05-ed5e-4ac0-a4b3-440796fb61ac")]
        [Guide("The *inventory location* where the received goods are stored. This helps track inventory across multiple locations or warehouses.")]
        public string[] GetInventoryLocation(GoodsReceipt[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<CustomInventoryLocation>(x.InventoryLocation)?.Name).ToArray();
        }

        [Default]
        [Guid("4f619027-ba53-49be-b46c-385fa5066069")]
        [Guide("A brief description of the goods receipt. This can include notes about the delivery, such as the condition of goods or any special handling instructions.")]
        public string[] GetDescription(GoodsReceipt[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Right]
        [Guid("bae0ae25-b02e-445a-96bf-f0f99e86fa40")]
        [Guide("The total quantity of items received in this goods receipt. This shows the sum of all *line items* and helps track inventory additions.")]
        public decimal[] GetQtyReceived(GoodsReceipt[] rows)
        {
            return rows.Select(x => x.Lines?.Sum(x => x.Qty ?? 0m) ?? 0m).ToArray();
        }

        protected override void OnFooterEndSection(Context context)
        {
            using (A(href: new GoodsReceiptLines() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "btn btn-xs")) Write(Strings.GoodsReceipts + " - " + Strings.Lines);
            base.OnFooterEndSection(context);
        }
    }
}
