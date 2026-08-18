using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerComponents;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CustomerStatementsUnpaidInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.CustomerStatementsUnpaidInvoices))]
    [Guide("`CustomerStatementsUnpaidInvoices` provides a comprehensive overview of all outstanding invoices for each customer. This is useful to show customers how much they owe with due dates.")]
    [Guide("To create a new `CustomerStatementsUnpaidInvoices`, go to `Reports` tab, click `CustomerStatementsUnpaidInvoices`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.CustomerStatementsUnpaidInvoices), name: nameof(Strings.NewReport))]
    internal sealed class CustomerStatementsUnpaidInvoicesList : Table<CustomerStatementsUnpaidInvoicesList.Record>
    {
        protected override HeaderButton GetPrimaryButton()
        {
            return new HeaderButton()
            {
                Text = Strings.SetDate,
                Url = new CustomerStatementsUnpaidInvoicesForm() { Business = Business, Referrer = this.ToUrl(), Key = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.CustomerStatementsUnpaidInvoices)) }.ToUrl()
            };
        }

        protected override Record[] GetObjects()
        {
            var date = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.CustomerStatementsUnpaidInvoices>().GetDate();
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .AutomaticallyMatchSalesInvoices().Where(x => x.Date <= date)
                .Where(x => x.GeneralLedgerAccount.IsAccountsReceivable && x.SalesInvoice != null)
                .GroupBy(x => new { x.Customer, x.SalesInvoice, x.AccountCurrency })
                .Select(x => new { x.Key.Customer, x.Key.SalesInvoice, x.Key.AccountCurrency, Balance = x.Sum(y => y.AccountAmount) })
                .Where(x => x.Balance != 0m)
                .GroupBy(x => x.Customer)
                .OrderBy(x => x.Key.NameWithCode)
                .Select(x => new Record()
                {
                    Date = date,
                    Customer = x.Key,
                    UnpaidInvoices = x.Count(),
                    Total = new Tuple<decimal, ManagerServer.Model.Currency>(x.Sum(y => y.Balance), x.First().AccountCurrency)
                })
                .ToArray();
        }

        protected override BusinessTemplate GetView(Record o, string referrer)
        {
            return new CustomerStatementUnpaidInvoicesView()
            {
                Business = Business,
                Key = o.Customer.Key,
                Referrer = referrer
            };
        }

        public record Record
        {
            [Center, WhitespaceNoWrap, MinWidth]
            public DateTime Date { get; set; }

            public ManagerServer.Model.Customer Customer { get; set; }

            [Center, WhitespaceNoWrap, MinWidth, Sum, TabularNums]
            public int UnpaidInvoices { get; set; }

            [Bold, Right, WhitespaceNoWrap, MinWidth, Sum, TabularNums]
            public Tuple<decimal, ManagerServer.Model.Currency> Total { get; set; }
        }
    }
}