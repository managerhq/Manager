using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxableSalesPerCustomer;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxableSalesPerCustomer
{
    [ProtoContract]
    internal sealed class GetTaxableSalesPerCustomerView : GetReportView<Model.TaxableSalesPerCustomer>
    {
        protected override string DefaultTitle => Strings.TaxableSalesPerCustomer;

        protected override ReportModel Build(Database business, Model.TaxableSalesPerCustomer report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            var accountingMethods = business.OfType<ManagerServer.Model.SalesInvoice>().Any() || business.OfType<ManagerServer.Model.PurchaseInvoice>().Any();
            if (accountingMethods)
            {
                model.Subtitle2 = Strings.AccrualBasis;
                if (report.AccountingMethod == AccountingBasis.CashBasis) model.Subtitle2 = Strings.CashBasis;
            }

            model.Columns.Add(new Column { Key = "NetSales", Name = Strings.NetSales });
            model.Columns.Add(new Column { Key = "TaxOnSales", Name = Strings.TaxOnSales, IsBold = true });
            model.Columns.Add(new Column { Key = "TotalSales", Name = Strings.TotalSales });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Currency, model.WholeNumbers, link);

            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business);
            if (report.AccountingMethod == AccountingBasis.CashBasis) transactions = transactions.AutomaticallyMatchSalesInvoices().ConvertSalesInvoicesToCashBasis2(report.FromDate.AddDays(-1), report.ToDate);
            var taxCodes = transactions.Where(x => (x.Transaction is SalesInvoice || x.Transaction is CreditNote) && x.Date >= report.FromDate && x.Date <= report.ToDate && x.TaxCode != null && x.Customer != null).GroupBy(x => x.TaxCode).OrderBy(x => x.Key.Name);

            foreach (var e in taxCodes)
            {
                var groupRows = new Rows();

                foreach (var e2 in e.GroupBy(x => x.Customer).OrderBy(x => x.Key.NameWithCode))
                {
                    var net = e2.Where(x => !x.IsTaxTransaction).Sum(x => x.AccountAmount) * -1m;
                    var tax = e2.Where(x => x.IsTaxTransaction).Sum(x => x.AccountAmount) * -1m;
                    var total = e2.Sum(x => x.AccountAmount) * -1m;

                    var row = new Row
                    {
                        Name = e2.Key.NameWithCode,
                        Cells = new System.Collections.Generic.List<Cell>
                        {
                            Make(net),
                            Make(tax, new Link(new TaxableSalesPerCustomerTransactions { Business = Business, Referrer = Referrer, From = report.FromDate, To = report.ToDate, Customer = e2.Key.Key, TaxCode = e.Key.Key, AccountingBasis = report.AccountingMethod }.ToUrl())),
                            Make(total),
                        }
                    };
                    // ExcludeIfZero: return null when all zero
                    if (net == 0m && tax == 0m && total == 0m) continue;
                    groupRows.Items.Add(row);
                }

                model.Rows.Items.Add(new Row { Name = e.Key.Name, Rows = groupRows });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
