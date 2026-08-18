using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Summary;
using ManagerServer.Model.Enums;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxSummary
{
    [ProtoContract]
    internal sealed class GetTaxSummaryView : GetReportView<Model.TaxSummary>
    {
        protected override string DefaultTitle => Strings.TaxSummary;

        protected override ReportModel Build(Database business, Model.TaxSummary report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            var accountingMethods = business.OfType<ManagerServer.Model.SalesInvoice>().Any() || business.OfType<ManagerServer.Model.PurchaseInvoice>().Any();
            if (accountingMethods)
            {
                model.Subtitle2 = Strings.AccrualBasis;
                if (report.AccountingMethod == AccountingBasis.CashBasis) model.Subtitle2 = Strings.CashBasis;
            }

            model.Columns.Add(new Column { Key = "netSales", Name = Strings.NetSales });
            model.Columns.Add(new Column { Key = "taxOnSales", Name = Strings.TaxOnSales, IsBold = true });
            model.Columns.Add(new Column { Key = "totalSales", Name = Strings.TotalSales });
            model.Columns.Add(new Column { Key = "netPurchases", Name = Strings.NetPurchases });
            model.Columns.Add(new Column { Key = "taxOnPurchases", Name = Strings.TaxOnPurchases, IsBold = true });
            model.Columns.Add(new Column { Key = "totalPurchases", Name = Strings.TotalPurchases });
            model.Columns.Add(new Column { Key = "taxOnSalesMinusTaxOnPurchases", Name = Strings.TaxLiability, IsBold = true });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Currency, model.WholeNumbers, link);

            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business);
            if (report.AccountingMethod == AccountingBasis.CashBasis) generalLedger = generalLedger.AutomaticallyMatchSalesInvoices().ConvertSalesInvoicesToCashBasis2(report.FromDate.AddDays(-1), report.ToDate).AutomaticallyMatchPurchaseInvoices().ConvertPurchaseInvoicesToCashBasis2(report.FromDate.AddDays(-1), report.ToDate);
            var generalLedger2 = generalLedger.Where(x => x.TaxCode != null && x.Date >= report.FromDate && x.Date <= report.ToDate);

            var division = business.SingleOrDefault<ManagerServer.Model.Division>(report.Division);
            if (division != null)
            {
                model.Subtitle2 += " - " + division.Name;
                generalLedger2 = generalLedger2.Where(x => x.Division?.Key == division.Key);
            }
            var taxCodes = generalLedger2.GroupBy(x => x.TaxCode).OrderBy(x => x.Key.Name);

            foreach (var e in taxCodes)
            {
                var netSales = e.Where(x => !x.IsTaxTransaction && x.IsSale).Sum(x => x.BaseAmount) * -1m;
                var taxCollected = e.Where(x => x.IsTaxTransaction && x.IsSale).Sum(x => x.BaseAmount) * -1m;
                var totalSales = netSales + taxCollected;

                var netPurchases = e.Where(x => !x.IsTaxTransaction && !x.IsSale).Sum(x => x.BaseAmount);
                var taxPaid = e.Where(x => x.IsTaxTransaction && !x.IsSale).Sum(x => x.BaseAmount);
                var totalPurchases = netPurchases + taxPaid;

                var cells = new System.Collections.Generic.List<Cell>
                {
                    Make(netSales, new Link(new HttpHandlers.Businesses.Business.Reports.TaxSummary.TaxSummaryTransactions { From = report.FromDate, To = report.ToDate, TaxTransactions = false, IsSale = true, CashBasis = report.AccountingMethod == AccountingBasis.CashBasis, Business = Business, Referrer = Referrer, TaxCode = e.Key.Key, Division = division?.Key }.ToUrl())),
                    Make(taxCollected),
                    Make(totalSales),
                    Make(netPurchases, new Link(new HttpHandlers.Businesses.Business.Reports.TaxSummary.TaxSummaryTransactions { From = report.FromDate, To = report.ToDate, TaxTransactions = false, IsSale = false, CashBasis = report.AccountingMethod == AccountingBasis.CashBasis, Business = Business, Referrer = Referrer, TaxCode = e.Key.Key, Division = division?.Key }.ToUrl())),
                    Make(taxPaid),
                    Make(totalPurchases),
                    Make(taxCollected - taxPaid),
                };

                model.Rows.Items.Add(new Row { Name = e.Key.Name, Cells = cells });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
