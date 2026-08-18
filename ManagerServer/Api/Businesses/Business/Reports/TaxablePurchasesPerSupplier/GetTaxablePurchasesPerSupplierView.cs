using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxablePurchasesPerSupplier;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxablePurchasesPerSupplier
{
    [ProtoContract]
    internal sealed class GetTaxablePurchasesPerSupplierView : GetReportView<Model.TaxablePurchasesPerSupplier>
    {
        protected override string DefaultTitle => Strings.TaxablePurchasesPerSupplier;

        protected override ReportModel Build(Database business, Model.TaxablePurchasesPerSupplier report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            var accountingMethods = business.OfType<ManagerServer.Model.SalesInvoice>().Any() || business.OfType<ManagerServer.Model.PurchaseInvoice>().Any();
            if (accountingMethods)
            {
                model.Subtitle2 = Strings.AccrualBasis;
                if (report.AccountingMethod == AccountingBasis.CashBasis) model.Subtitle2 = Strings.CashBasis;
            }

            model.Columns.Add(new Column { Key = "NetPurchases", Name = Strings.NetPurchases });
            model.Columns.Add(new Column { Key = "TaxOnPurchases", Name = Strings.TaxOnPurchases, IsBold = true });
            model.Columns.Add(new Column { Key = "TotalPurchases", Name = Strings.TotalPurchases });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Currency, model.WholeNumbers, link);

            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business);
            if (report.AccountingMethod == AccountingBasis.CashBasis) transactions = transactions.AutomaticallyMatchPurchaseInvoices().ConvertPurchaseInvoicesToCashBasis2(report.FromDate.AddDays(-1), report.ToDate);
            var taxCodes = transactions.Where(x => (x.Transaction is PurchaseInvoice || x.Transaction is DebitNote) && x.Date >= report.FromDate && x.Date <= report.ToDate && x.TaxCode != null && x.Supplier != null).GroupBy(x => x.TaxCode).OrderBy(x => x.Key.Name);

            foreach (var e in taxCodes)
            {
                var groupRows = new Rows();

                foreach (var e2 in e.GroupBy(x => x.Supplier).OrderBy(x => x.Key.NameWithCode))
                {
                    var net = e2.Where(x => !x.IsTaxTransaction).Sum(x => x.AccountAmount);
                    var tax = e2.Where(x => x.IsTaxTransaction).Sum(x => x.AccountAmount);
                    var total = e2.Sum(x => x.AccountAmount);

                    var row = new Row
                    {
                        Name = e2.Key.NameWithCode,
                        Cells = new System.Collections.Generic.List<Cell>
                        {
                            Make(net),
                            Make(tax, new Link(new TaxablePurchasesPerSupplierTransactions { Business = Business, Referrer = Referrer, From = report.FromDate, To = report.ToDate, Supplier = e2.Key.Key, TaxCode = e.Key.Key, AccountingBasis = report.AccountingMethod }.ToUrl())),
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
