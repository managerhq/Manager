using System.Linq;
using ManagerServer.Globalization;
using ManagerServer;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryProfitMargin
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryProfitMargin), nameof(Strings.Transactions))]
    [Guide("Shows profit transactions for individual inventory items.")]
    [Guide("Displays sales revenue, cost of sales, and profit margins for each transaction.")]
    [Columns]
    internal sealed class InventoryProfitMarginTransactions : ObjectTable<InventoryProfitMarginTransactions.Record>
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;
        [ProtoMember(3)] public Guid Item;

        protected override BusinessTemplate GetEdit(Record o, string referrer)
        {
            return TransactionViewer.GetEditHandler(Business, o.Transaction, referrer);
        }

        protected override BusinessTemplate GetView(Record o, string referrer)
        {
            return TransactionViewer.GetViewHandler(Business, o.Transaction, referrer);
        }

        protected override Record[] GetObjects()
        {
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BaseCurrency>();

            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.Item?.Key == Item && !x.IsTaxTransaction && !x.GeneralLedgerAccount.IsInventoryOnHand && x.AccountAmount != 0m)
                .Where(x => (x.SalesInvoiceAsTransaction != null && x.SalesInvoiceAsTransaction.IssueDate >= From && x.SalesInvoiceAsTransaction.IssueDate <= To) || (x.CreditNote != null && x.CreditNote.IssueDate >= From && x.CreditNote.IssueDate <= To) || (x.Receipt != null && x.Receipt.Date >= From && x.Receipt.Date <= To))
                .OrderBy(x => x.Date)
                .GroupBy(x => x.Transaction)
                .Select(x => new Record()
                {
                    Transaction = x.Key,
                    GeneralLedgerTransactions = x.ToArray()
                }).ToArray();
        }

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("835565df-f670-4e18-ac17-fc97d497e092")]
        public DateTime GetDate(Record o) => o.GeneralLedgerTransactions.Min(x => x.Date);

        [Guid("b3f3ad5e-0fd7-44d5-acab-b3f9240f0a24")]
        public string GetTransaction(Record o) => o.Transaction.GetTransactionName();

        [HideColumnIfAllEmpty]
        [Guid("feaf39cf-fc17-4907-a0b3-5e2a0bbdc21a")]
        public string GetDescription(Record o) => o.Transaction.GetDescriptionOrNull();

        [Right, WhitespaceNoWrap, Sum]
        [Guid("a741ea66-d99a-4ac6-8619-96acd598473c")]
        public decimal GetSales(Record o)
        {
            return -o.GeneralLedgerTransactions.Where(x => !x.IsCostOfGoodsSold).Sum(x => x.BaseAmount);
        }

        [Right, WhitespaceNoWrap, Sum]
        [Guid("151ec75a-ae05-4588-bc42-7090cdcc502b")]
        public decimal GetCostOfSales(Record o)
        {
            return -o.GeneralLedgerTransactions.Where(x => x.IsCostOfGoodsSold).Sum(x => x.BaseAmount);
        }

        [Right, Bold, WhitespaceNoWrap, Sum]
        [Guid("7eebe013-d2ee-4a44-b277-7d170697be90")]
        public decimal GetProfit(Record o)
        {
            return GetSales(o) + GetCostOfSales(o);
        }

        [Center, WhitespaceNoWrap]
        [Guid("30defd26-998f-4603-bbe2-c73cd232835b")]
        public Tuple<decimal, string> GetMargin(Record o)
        {
            var sales = GetSales(o);
            var profit = GetProfit(o);
            var margin = 0;
            if (profit != 0m && sales != 0m) margin = (int)(profit / (sales / 100m));
            return new Tuple<decimal, string>(margin, $"{margin}%");
        }

        public record Record
        {
            public ManagerServer.Model.Transaction Transaction;
            public ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] GeneralLedgerTransactions;
        }
    }
}