using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SupplierStatementsTransactions
{
    [ProtoContract]
    [Title(nameof(Strings.SupplierStatementsTransactions))]
    [Guide("*Supplier Statements - Transactions* provides a detailed overview of all transactions between your business and its suppliers, helping you track payments, invoices, and credits efficiently.")]
    [Guide("This report shows a complete transaction history for each supplier, including purchase invoices, debit notes, payments, and any other transactions that affect your accounts payable balance.")]
    [Guide("To create a new supplier statement report, go to the **Reports** tab, click **Supplier Statements - Transactions**, then click the **New Report** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.SupplierStatementsTransactions), name: nameof(Strings.NewReport))]
    internal sealed class SupplierStatementsTransactionsList : Table<SupplierStatementsTransactionsList.Record>
    {
        protected override ManagerComponents.HeaderButton GetPrimaryButton()
        {
            return new ManagerComponents.HeaderButton()
            {
                Text = Strings.SetPeriod,
                Url = new SupplierStatementsTransactionsForm() { Business = Business, Referrer = this.ToUrl(), Key = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.SupplierStatementsTransactions)) }.ToUrl()
            };
        }

        protected override Record[] GetObjects()
        {
            var from = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.SupplierStatementsTransactions>().FromDate;
            var to = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.SupplierStatementsTransactions>().GetToDate();

            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.Date <= to && x.GeneralLedgerAccount.IsAccountsPayable)
                .GroupBy(x => x.Supplier)
                .Where(x => x.Any(y => y.Date >= from))
                .OrderBy(x => x.Key.NameWithCode)
                .Select(x => new Record()
                {
                    FromDate = from,
                    ToDate = to,
                    Supplier = x.Key,
                    Transactions = x.Count(y => y.Date >= from),
                    Balance = new Tuple<decimal, Currency>(-x.Sum(y => y.AccountAmount), x.First().AccountCurrency)
                }).ToArray();
        }

        protected override BusinessTemplate GetView(Record o, string referrer)
        {
            return new SupplierStatementsTransactionsView()
            {
                Business = Business,
                Key = o.Supplier.Key,
                Referrer = referrer
            };
        }

        public record Record
        {
            [Center, WhitespaceNoWrap, MinWidth]
            public DateTime FromDate { get; set; }

            [Center, WhitespaceNoWrap, MinWidth]
            public DateTime ToDate { get; set; }

            public ManagerServer.Model.Supplier Supplier { get; set; }

            [Center, WhitespaceNoWrap, MinWidth, Sum, TabularNums]
            public int Transactions { get; set; }

            [Bold, Right, WhitespaceNoWrap, MinWidth, Sum, TabularNums]
            public Tuple<decimal, Currency> Balance { get; set; }
        }
    }
}