using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Query.GeneralLedger;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryQuantityByLocation
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryQuantityByLocation), nameof(Strings.Transactions))]
    [Guide("Shows inventory quantity movements for items at specific locations.")]
    [Guide("Displays receipts, deliveries, and transfers affecting inventory balances.")]
    [Columns]
    internal sealed class InventoryQuantityByLocationTransactions : ObjectTable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        [ProtoMember(1)] public Guid InventoryItem;
        [ProtoMember(2)] public Guid? InventoryLocation;
        [ProtoMember(3)] public DateTime Date;

        protected override GeneralLedgerTransaction[] GetObjects()
        {
            var inventoryLocation = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.CustomInventoryLocation>(InventoryLocation);
            var inventoryItem = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.InventoryItem>(InventoryItem);

            var database = ApplicationData.Businesses.Get(Business);
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).ToList();
            transactions.AddRange(database.OfType<ManagerServer.Model.GoodsReceipt>().SelectMany(x => x.GetGeneralLedgerTransactions(database)));
            transactions.AddRange(database.OfType<ManagerServer.Model.DeliveryNote>().SelectMany(x => x.GetGeneralLedgerTransactions(database)));
            transactions.AddRange(database.OfType<ManagerServer.Model.InventoryTransfer>().SelectMany(x => x.GetGeneralLedgerTransactions(database)));

            return transactions
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand && x.InventoryItem == inventoryItem && x.InventoryLocation == inventoryLocation && x.Date <= Date)
                .Where(x => x.QtyOnHand != 0m)
                .OrderByDescending(x => x.Date)
                .ToArray();
        }

        protected override BusinessTemplate GetEdit(GeneralLedgerTransaction o, string referrer)
        {
            return TransactionViewer.GetEditHandler(Business, o.Transaction, referrer);
        }

        protected override BusinessTemplate GetView(GeneralLedgerTransaction o, string referrer)
        {
            return TransactionViewer.GetViewHandler(Business, o.Transaction, referrer);
        }

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("da1c4f51-ecbe-4852-a3a0-68a6b4564e73")]
        public DateTime GetDate(GeneralLedgerTransaction o) => o.Date;

        [Guid("fa7b6b58-f75a-4b5f-ad65-cfccaf3f70eb")]
        public string GetTransaction(GeneralLedgerTransaction o) => o.Transaction.GetTransactionName();

        [HideColumnIfAllEmpty]
        [Guid("137603c4-53e2-4860-b203-80f3ce50ae9e")]
        public string GetDescription(GeneralLedgerTransaction o) => o.Transaction.GetDescriptionOrNull();

        [Sum, Bold, Right, WhitespaceNoWrap]
        [Guid("5d527dcd-528d-4a41-bc8f-a87e0810ce54")]
        public decimal GetQty(GeneralLedgerTransaction o) => o.QtyOnHand;
    }
}