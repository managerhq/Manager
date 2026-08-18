using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business;
using ManagerServer.Model.Enums;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxTransactions
{
    [ProtoContract]
    internal sealed class GetTaxTransactionsView : GetReportView<Model.TaxTransactions>
    {
        protected override string DefaultTitle => Strings.TaxTransactions;

        protected override ReportModel Build(Database business, Model.TaxTransactions report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            var accountingMethods = business.OfType<ManagerServer.Model.SalesInvoice>().Any() || business.OfType<ManagerServer.Model.PurchaseInvoice>().Any();
            if (accountingMethods)
            {
                model.Subtitle2 = Strings.AccrualBasis;
                if (report.AccountingMethod == AccountingBasis.CashBasis) model.Subtitle2 = Strings.CashBasis;
            }

            model.Columns.Add(new Column { Key = "totalSales", Name = Strings.TotalSales });
            model.Columns.Add(new Column { Key = "taxOnSales", Name = Strings.TaxOnSales });
            model.Columns.Add(new Column { Key = "totalPurchases", Name = Strings.TotalPurchases });
            model.Columns.Add(new Column { Key = "taxOnPurchases", Name = Strings.TaxOnPurchases });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Currency, model.WholeNumbers, link);

            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business);
            if (report.AccountingMethod == AccountingBasis.CashBasis) generalLedger = generalLedger.AutomaticallyMatchSalesInvoices().ConvertSalesInvoicesToCashBasis2(report.FromDate.AddDays(-1), report.ToDate).AutomaticallyMatchPurchaseInvoices().ConvertPurchaseInvoicesToCashBasis2(report.FromDate.AddDays(-1), report.ToDate);
            var taxCodes = generalLedger.Where(x => x.TaxCode != null && x.Date >= report.FromDate && x.Date <= report.ToDate).GroupBy(x => x.TaxCode).OrderBy(x => x.Key.Name);

            foreach (var e in taxCodes)
            {
                var groupRows = new Rows();

                foreach (var e2 in e.OrderBy(x => x.Date).GroupBy(x => x.Transaction))
                {
                    var saleAmount = e2.Where(x => x.IsSale).Sum(x => x.BaseAmount) * -1m;
                    var saleTaxAmount = e2.Where(x => x.IsSale).Where(x => x.IsTaxTransaction).Sum(x => x.BaseAmount) * -1m;
                    var purchaseAmount = e2.Where(x => !x.IsSale).Sum(x => x.BaseAmount);
                    var purchaseTaxAmount = e2.Where(x => !x.IsSale).Where(x => x.IsTaxTransaction).Sum(x => x.BaseAmount);

                    var name = string.Join(" — ", new[] { e2.First().Date.ToLocalShortDisplayString(), e2.Key.GetName(), e2.First().Customer?.Name, e2.First().Supplier?.Name, e2.First().BankAccount?.Name }.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());

                    var viewHandler = TransactionViewer.GetViewHandler(Business, e2.Key, Referrer);
                    Link link = viewHandler != null ? new Link(viewHandler.ToUrl()) : null;

                    groupRows.Items.Add(new Row
                    {
                        Name = name,
                        Cells = new System.Collections.Generic.List<Cell>
                        {
                            Make(saleAmount, link),
                            Make(saleTaxAmount),
                            Make(purchaseAmount, link),
                            Make(purchaseTaxAmount),
                        }
                    });
                }

                model.Rows.Items.Add(new Row { Name = e.Key.Name, Rows = groupRows });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
