using System.Linq;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryTransfers
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.InventoryTransfers))]
    [Guid("930c3032-d279-499d-b82c-2ade542fd427")]
    [Guide("The **Inventory Transfers** tab enables you to track and record the movement of items between *inventory locations*. This feature is essential for businesses that operate with multiple storage areas, warehouses, or retail locations.")]
    [TabScreenshot("fa-person-dolly", nameof(Strings.InventoryTransfers))]
    [Guide("To create a new inventory transfer, click the **New Inventory Transfer** button.")]
    [HeroButtonScreenshot(nameof(Strings.InventoryTransfers), nameof(Strings.NewInventoryTransfer))]
    [Guide("The **Inventory Transfers** tab displays transfers in a table with the following columns:")]
    [Columns]
    internal sealed class InventoryTransfers : NakedObjectsWithAutomaticRows<InventoryTransfer>
    {
        [Default]
        [WarnIfFutureDate]
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("162e3b46-6b65-411a-bb35-a9a9db5e9fe8")]
        [Guide("The date when the inventory transfer occurred.")]
        public DateTime[] GetDate(InventoryTransfer[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [PaddedSorting]
        [WarnIfNotUnique]
        [Guid("74958baa-a7c5-4a5d-b759-4f074d3c94c6")]
        [Guide("A unique reference number to identify the inventory transfer. This can be automatically generated or manually entered.")]
        public string[] GetReference(InventoryTransfer[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("357539f2-3a33-4285-83c3-b83fbd6153c9")]
        [Guide("The *inventory location* from which items are being transferred.")]
        public string[] GetFrom(InventoryTransfer[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<CustomInventoryLocation>(x.InventoryLocation)?.Name).ToArray();
        }

        [Default]
        [Guid("02f1864e-ea71-42e0-8997-04a4e4efb61d")]
        [Guide("The *inventory location* to which items are being transferred.")]
        public string[] GetTo(InventoryTransfer[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<CustomInventoryLocation>(x.ToInventoryLocation)?.Name).ToArray();
        }

        [Default]
        [Guid("800a75d5-ace4-49e7-b600-faa19c0877b5")]
        [Guide("An optional description or notes about the inventory transfer. Use this field to record additional details such as the reason for the transfer or special handling instructions.")]
        public string[] GetDescription(InventoryTransfer[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Default]
        [Guid("10ae41d4-08c5-4ce3-a5fe-5f7fc32b14ba")]
        [Guide("A list of *inventory items* included in this transfer. Multiple items can be transferred in a single transaction.")]
        public string[] GetInventoryItems(InventoryTransfer[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => string.Join(", ", x.Lines?.Select(x => x.Item).Distinct().Select(x => database.SingleOrDefault<InventoryItem>(x)?.GetNameWithCode()).Where(x => !string.IsNullOrWhiteSpace(x)) ?? new string[0])).ToArray();
        }

        [Right, Bold]
        [Guid("6e2f89e5-1bcc-4ac0-aa33-7dafbbea9053")]
        [Guide("The total quantity of items transferred. This represents the sum of all item quantities in the transfer.")]
        public decimal[] GetQty(InventoryTransfer[] rows)
        {
            return rows.Select(x => x.Lines?.Sum(x => x.Qty ?? 0m) ?? 0m).ToArray();
        }

        protected override void OnFooterEndSection(Context context)
        {
            using (A(href: new InventoryTransferLines() { Business = Business }.ToUrl(), @class: "btn btn-xs")) Write(Strings.InventoryTransfers + " - " + Strings.Lines);
            base.OnFooterEndSection(context);
        }
    }
}
