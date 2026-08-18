using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerComponents;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SupplierStatementsUnpaidInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.SupplierStatementsUnpaidInvoices))]
    [Guide("*Supplier Statements - Unpaid Invoices* provides a comprehensive overview of all outstanding purchase invoices from your suppliers, helping you track which invoices remain unpaid and manage your accounts payable effectively.")]
    [Guide("This report shows each supplier with their unpaid invoices, the number of outstanding invoices, and the total amount owed as of a specific date.")]
    [Guide("To create a new supplier statement report, go to the **Reports** tab, click **Supplier Statements - Unpaid Invoices**, then click the **New Report** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.SupplierStatementsUnpaidInvoices), name: nameof(Strings.NewReport))]
    internal sealed class SupplierStatementsUnpaidInvoicesList : Table<SupplierStatementsUnpaidInvoicesList.Record>
    {
        protected override HeaderButton GetPrimaryButton()
        {
            return new HeaderButton()
            {
                Text = Strings.SetDate,
                Url = new SupplierStatementsUnpaidInvoicesForm() { Business = Business, Referrer = this.ToUrl(), Key = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.SupplierStatementsUnpaidInvoices)) }.ToUrl()
            };
        }

        protected override Record[] GetObjects()
        {
            var date = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.SupplierStatementsUnpaidInvoices>().GetDate();

            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .AutomaticallyMatchPurchaseInvoices()
                .Where(x => x.Date <= date)
                .Where(x => x.GeneralLedgerAccount.IsAccountsPayable && x.PurchaseInvoice != null)
                .GroupBy(x => new { x.Supplier, x.PurchaseInvoice.Key })
                .Select(x => new { x.Key.Supplier, x.First().AccountCurrency, Balance = x.Sum(y => y.AccountAmount) })
                .Where(x => x.Balance != 0m)
                .GroupBy(x => x.Supplier)
                .OrderBy(x => x.Key.NameWithCode)
                .Select(x => new Record()
                {
                    Date = date,
                    Supplier = x.Key,
                    UnpaidInvoices = x.Count(),
                    Total = new Tuple<decimal, ManagerServer.Model.Currency>(-x.Sum(y => y.Balance), x.First().AccountCurrency)
                })
                .ToArray();
        }

        protected override BusinessTemplate GetView(Record o, string referrer)
        {
            return new SupplierStatementsUnpaidInvoicesView()
            {
                Business = Business,
                Key = o.Supplier.Key,
                Referrer = referrer
            };
        }

        public record Record
        {
            [Center, WhitespaceNoWrap, MinWidth]
            public DateTime Date { get; set; }

            public ManagerServer.Model.Supplier Supplier { get; set; }

            [Center, WhitespaceNoWrap, MinWidth, Sum, TabularNums]
            public int UnpaidInvoices { get; set; }

            [Bold, Right, WhitespaceNoWrap, MinWidth, Sum, TabularNums]
            public Tuple<decimal, ManagerServer.Model.Currency> Total { get; set; }
        }
    }
}