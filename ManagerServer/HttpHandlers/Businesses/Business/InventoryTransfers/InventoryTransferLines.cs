using ManagerServer.Model;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryTransfers
{
    [ProtoContract]
    [Guid("5a9c07e8-ce95-4abb-8536-a5bc0dab11af")]
    [Title(nameof(Strings.InventoryTransfer), nameof(Strings.Lines))]
    [Guide("This screen displays individual line items from all *inventory transfers* in your business.")]
    [Guide("Each row represents a single inventory item that was transferred from one location to another.")]
    [Guide("Use this screen to review the details of all inventory movements across your business, including quantities transferred and the specific items involved.")]
    [Guide("You can click on any line to view or edit the complete *inventory transfer* transaction that contains that line item.")]
    [Columns]
    internal sealed class InventoryTransferLines : NakedObjectsWithCustomFields<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        protected override Type GetCustomFieldsType()
        {
            return typeof(ManagerServer.Model.InventoryTransfer.Line);
        }

        protected override void InnerGet4(Context context)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var rows = database.OfType<InventoryTransfer>().SelectMany(x => x.GetGeneralLedgerTransactions(database)).ToArray();
            context.Set<Array>(rows);

            base.InnerGet4(context);
        }

        public override BusinessTemplate[] GetEdit(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new InventoryTransferForm() { Business = Business, Key = x.Transaction.Key, Referrer = referrer }).ToArray();
        }

        public override BusinessTemplate[] GetView(GeneralLedgerTransaction[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new InventoryTransferView() { Business = Business, Key = x.Transaction.Key, Referrer = referrer }).ToArray();
        }

        [Default]
        [WarnIfFutureDate, Center, MinWidth]
        [WhitespaceNoWrap]
        [Guid("6ae0d1f3-cb44-429c-97e6-89d1e31d60ed")]
        public DateTime[] GetDate(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("0d3cb724-2ea7-473c-b3f0-6c7ab1e9e271")]
        public string[] GetReference(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryTransfer.Reference).ToArray();
        }

        [Default]
        [Guid("ee2f312e-8729-462f-8b58-135b2acb9aaf")]
        public string[] GetInventoryLocation(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryLocation?.Name).ToArray();
        }

        [Guid("3d3683c1-d96f-4938-ae1c-dcd5569ee5be")]
        public string[] GetDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryTransfer.Description).ToArray();
        }

        [Default]
        [Guid("c7e1c82d-80ea-4eaf-901c-0247dd4af78e")]
        public string[] GetInventoryItem(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.InventoryItem?.GetNameWithCode()).ToArray();
        }

        [Guid("8e65703b-e6ee-4ce8-b8e5-d66d728fb7a2")]
        public string[] GetLineDescription(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.TransactionLine.GetLineDescription(x.Transaction)).ToArray();
        }

        [Bold]
        [Default]
        [Right, Sum]
        [Guid("6f8fac66-49e9-4ae4-ac29-4ac2a744f8d1")]
        public decimal?[] GetQty(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] rows)
        {
            return rows.Select(x => x.Qty.HasValue ? x.Qty.Value : default(decimal?)).ToArray();
        }
    }
}
