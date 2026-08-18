using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CustomerStatementsTransactions
{
    [ProtoContract]
    [Title(nameof(Strings.CustomerStatementsTransactions))]
    [Guide("`CustomerStatementsTransactions` provides detailed summary of all transactions associated with your customers which is useful when customer would like to reconcile their accounts with your records.")]
    [Guide("To create a new `CustomerStatementsTransactions`, go to `Reports` tab, click `CustomerStatementsTransactions`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.CustomerStatementsTransactions), name: nameof(Strings.NewReport))]
    internal sealed class CustomerStatementsTransactionsList : Table<CustomerStatementsTransactionsList.Record>
    {
        protected override ManagerComponents.HeaderButton GetPrimaryButton()
        {
            return new ManagerComponents.HeaderButton()
            {
                Text = Strings.SetPeriod,
                Url = new CustomerStatementsTransactionsForm() { Business = Business, Referrer = this.ToUrl(), Key = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.CustomerStatementsTransactions)) }.ToUrl()
            };
        }

        protected override Record[] GetObjects()
        {
            var from = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.CustomerStatementsTransactions>().FromDate;
            var to = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.CustomerStatementsTransactions>().GetToDate();
            var theme = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.CustomerStatementsTransactions>();

            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.Date <= to && x.GeneralLedgerAccount.IsAccountsReceivable)
                .GroupBy(x => x.Customer)
                .Where(x => x.Any(y => y.Date >= from))
                .OrderBy(x => x.Key.NameWithCode)
                .Select(x => new Record()
                {
                    FromDate = from,
                    ToDate = to,
                    Customer = x.Key,
                    Transactions = x.Count(y => y.Date >= from),
                    Balance = new Tuple<decimal, ManagerServer.Model.Currency>(x.Sum(y => y.AccountAmount), x.First().AccountCurrency)
                }).ToArray();
        }

        protected override BusinessTemplate GetView(Record o, string referrer)
        {
            return new CustomerStatementsTransactionsView()
            {
                Business = Business,
                Key = o.Customer.Key,
                Referrer = referrer
            };
        }

        public record Record
        {
            [Center, WhitespaceNoWrap, MinWidth]
            public DateTime FromDate { get; set; }

            [Center, WhitespaceNoWrap, MinWidth]
            public DateTime ToDate { get; set; }

            public ManagerServer.Model.Customer Customer { get; set; }

            [Center, WhitespaceNoWrap, MinWidth, Sum, TabularNums]
            public int Transactions { get; set; }

            [Bold, Right, WhitespaceNoWrap, MinWidth, Sum, TabularNums]
            public Tuple<decimal, ManagerServer.Model.Currency> Balance { get; set; }
        }
    }
}