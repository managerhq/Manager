using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Query.GeneralLedger;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryQuantitySummary
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryQuantitySummary), nameof(Strings.Transactions))]
    [Guide("Shows inventory quantity movements for specific items.")]
    [Guide("Displays purchases, sales, and other transactions affecting inventory quantities.")]
    [Columns]
    internal sealed class InventoryQuantitySummaryTransactions : ObjectTable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        [ProtoMember(1)] public Guid InventoryItem;
        [ProtoMember(2)] public DateTime From;
        [ProtoMember(3)] public DateTime To;
        [ProtoMember(4)] public bool? Sales;
        [ProtoMember(5)] public bool? Purchases;
        [ProtoMember(6)] public bool? DebitNotes;
        [ProtoMember(7)] public bool? CreditNotes;
        [ProtoMember(8)] public bool? ProductionOrders;
        [ProtoMember(9)] public bool? InventoryWriteOffs;
        [ProtoMember(10)] public bool? JournalEntries;

        protected override GeneralLedgerTransaction[] GetObjects()
        {
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.GeneralLedgerAccount.IsInventoryOnHand)
                .Where(x => x.InventoryItem?.Key == InventoryItem)
                .Where(x => x.Date >= From && x.Date <= To)
                .Where(x => x.Qty.HasValue && x.Qty.Value != 0m);

            if (JournalEntries == true) transactions = transactions.Where(x => x.Transaction.GetType() == typeof(JournalEntry));
            if (DebitNotes == true) transactions = transactions.Where(x => x.Transaction.GetType() == typeof(DebitNote));
            if (CreditNotes == true) transactions = transactions.Where(x => x.Transaction.GetType() == typeof(CreditNote));
            if (ProductionOrders == true) transactions = transactions.Where(x => x.Transaction.GetType() == typeof(ProductionOrder));
            if (InventoryWriteOffs == true) transactions = transactions.Where(x => x.Transaction.GetType() == typeof(InventoryWriteOff));
            if (Sales == true) transactions = transactions.Where(x => x.IsSale && x.Transaction.GetType() != typeof(ManagerServer.Model.ProductionOrder) && x.Transaction.GetType() != typeof(ManagerServer.Model.InventoryWriteOff) && x.Transaction.GetType() != typeof(ManagerServer.Model.JournalEntry) && x.Transaction.GetType() != typeof(ManagerServer.Model.DebitNote) && x.Transaction.GetType() != typeof(ManagerServer.Model.CreditNote));
            if (Purchases == true) transactions = transactions.Where(x => !x.IsSale && x.Transaction.GetType() != typeof(ManagerServer.Model.ProductionOrder) && x.Transaction.GetType() != typeof(ManagerServer.Model.InventoryWriteOff) && x.Transaction.GetType() != typeof(ManagerServer.Model.JournalEntry) && x.Transaction.GetType() != typeof(ManagerServer.Model.DebitNote) && x.Transaction.GetType() != typeof(ManagerServer.Model.CreditNote));

            return transactions.OrderByDescending(x => x.Date).ThenByDescending(x => x.Qty.Value < 0m).ToArray();
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
        [Guid("9b3eb7d6-98c8-4a76-9d55-a00dbb0ce527")]
        public DateTime GetDate(GeneralLedgerTransaction o) => o.Date;

        [Guid("e72c44f2-3014-4d88-bf28-e1e005d01bd5")]
        public string GetTransaction(GeneralLedgerTransaction o) => o.Transaction.GetTransactionName();

        [HideColumnIfAllEmpty]
        [Guid("305f96ff-4943-434f-87cb-7463305bcbec")]
        public string GetBankOrCashAccount(GeneralLedgerTransaction o) => o.BankAccount?.GetCodeAndName();

        [HideColumnIfAllEmpty]
        [Guid("88ae8d20-b8fe-4a89-a802-b689124435e5")]
        public string GetCustomer(GeneralLedgerTransaction o) => o.Customer?.GetCodeAndName();

        [HideColumnIfAllEmpty]
        [Guid("f137a652-5c7f-4b05-82f7-6f43a9a52346")]
        public string GetSupplier(GeneralLedgerTransaction o) => o.Supplier?.GetCodeAndName();

        [HideColumnIfAllEmpty]
        [Guid("8401bce8-e511-4f5d-b57c-237c3726f412")]
        public string GetDescription(GeneralLedgerTransaction o) => o.TransactionLine?.GetDescriptionOrNull(o.Transaction) ?? o.Transaction.GetDescriptionOrNull();

        [Bold, Right, WhitespaceNoWrap, Sum]
        [Guid("c2d53b78-7b17-473f-883a-0e5f9d8ddd6c")]
        public decimal GetQty(GeneralLedgerTransaction o) => o.Qty.Value;
    }
}
