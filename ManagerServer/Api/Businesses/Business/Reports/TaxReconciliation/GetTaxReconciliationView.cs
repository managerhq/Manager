using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxReconciliation;
using ManagerServer.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxReconciliation
{
    [ProtoContract]
    internal sealed class GetTaxReconciliationView : GetReportView<Model.TaxReconciliation>
    {
        protected override string DefaultTitle => Strings.TaxReconciliation;

        protected override ReportModel Build(Database business, Model.TaxReconciliation report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.Periods[0].FromDate.ToLocalShortDisplayString(), report.Periods[0].ToDate.ToLocalShortDisplayString());

            var accountingMethods = business.OfType<ManagerServer.Model.SalesInvoice>().Any() || business.OfType<ManagerServer.Model.PurchaseInvoice>().Any();
            if (accountingMethods)
            {
                model.Subtitle2 = Strings.AccrualBasis;
                if (report.AccountingMethod == AccountingBasis.CashBasis) model.Subtitle2 = Strings.CashBasis;
            }

            for (int i = 0; i < report.Periods.Length; i++)
            {
                model.Columns.Add(new Column { Name = report.Periods[i].ToDate.ToLocalShortDisplayString(), IsBold = (i == 0) });
            }

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.CurrencyParentheses, model.WholeNumbers, link);

            var dates = new List<DateTime>();
            dates.AddRange(report.Periods.Select(x => x.FromDate).Where(x => x > DateTime.MinValue).Select(x => x.AddDays(-1)));
            dates.AddRange(report.Periods.Select(x => x.ToDate));
            dates = dates.Distinct().ToList();

            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business);
            if (report.AccountingMethod == AccountingBasis.CashBasis) generalLedger = generalLedger.AutomaticallyMatchSalesInvoices().ConvertSalesInvoicesToCashBasis2(dates.ToArray()).AutomaticallyMatchPurchaseInvoices().ConvertPurchaseInvoicesToCashBasis2(dates.ToArray());
            var accounts = generalLedger.GroupBy(x => x.GeneralLedgerAccount);

            foreach (var e in accounts)
            {
                if (!e.Any(x => x.IsTaxTransaction)) continue;

                var closingBalanceRows = new Rows { TotalText = Strings.ClosingBalance };

                var openingBalanceCells = new List<Cell>();
                for (int i = 0; i < report.Periods.Length; i++)
                {
                    var openingBalance = e.Where(x => x.Date < report.Periods[i].FromDate).Sum(x => x.BaseAmount);
                    openingBalanceCells.Add(Make(openingBalance));
                }
                closingBalanceRows.Items.Add(new Row { Name = Strings.OpeningBalance, Cells = openingBalanceCells });

                var paymentsCells = new List<Cell>();
                for (int i = 0; i < report.Periods.Length; i++)
                {
                    var movement = e.Where(x => x.Date >= report.Periods[i].FromDate && x.Date <= report.Periods[i].ToDate && !x.IsTaxTransaction && x.Payment != null).Sum(x => x.BaseAmount);
                    paymentsCells.Add(Make(movement, new Link(new TaxReconciliationTransactions { Business = Business, Referrer = Referrer, GeneralLedgerAccount = e.Key.Key, From = report.Periods[i].FromDate, To = report.Periods[i].ToDate, Payments = true, AccountingBasis = report.AccountingMethod }.ToUrl())));
                }
                closingBalanceRows.Items.Add(new Row { Name = Strings.Payments, Cells = paymentsCells });

                var receiptsCells = new List<Cell>();
                for (int i = 0; i < report.Periods.Length; i++)
                {
                    var movement = e.Where(x => x.Date >= report.Periods[i].FromDate && x.Date <= report.Periods[i].ToDate && !x.IsTaxTransaction && x.Receipt != null).Sum(x => x.BaseAmount);
                    receiptsCells.Add(Make(movement, new Link(new TaxReconciliationTransactions { Business = Business, Referrer = Referrer, GeneralLedgerAccount = e.Key.Key, From = report.Periods[i].FromDate, To = report.Periods[i].ToDate, Receipts = true, AccountingBasis = report.AccountingMethod }.ToUrl())));
                }
                closingBalanceRows.Items.Add(new Row { Name = Strings.Receipts, Cells = receiptsCells });

                var otherMovementsCells = new List<Cell>();
                for (int i = 0; i < report.Periods.Length; i++)
                {
                    var movement = e.Where(x => x.Date >= report.Periods[i].FromDate && x.Date <= report.Periods[i].ToDate && !x.IsTaxTransaction && x.Receipt == null && x.Payment == null).Sum(x => x.BaseAmount);
                    otherMovementsCells.Add(Make(movement, new Link(new TaxReconciliationTransactions { Business = Business, Referrer = Referrer, GeneralLedgerAccount = e.Key.Key, From = report.Periods[i].FromDate, To = report.Periods[i].ToDate, Adjustments = true, AccountingBasis = report.AccountingMethod }.ToUrl())));
                }
                closingBalanceRows.Items.Add(new Row { Name = Strings.OtherMovements, Cells = otherMovementsCells });

                model.Rows.Items.Add(new Row { Name = e.Key.GetName(), Rows = closingBalanceRows });

                var newLiabilityRows = new Rows { TotalText = Strings.Total };

                var taxCollectedCells = new List<Cell>();
                for (int i = 0; i < report.Periods.Length; i++)
                {
                    var movement = e.Where(x => x.Date >= report.Periods[i].FromDate && x.Date <= report.Periods[i].ToDate && x.IsTaxTransaction && x.IsSale).Sum(x => x.BaseAmount);
                    taxCollectedCells.Add(Make(movement, new Link(new TaxReconciliationTransactions { Business = Business, Referrer = Referrer, GeneralLedgerAccount = e.Key.Key, From = report.Periods[i].FromDate, To = report.Periods[i].ToDate, TaxCollected = true, AccountingBasis = report.AccountingMethod }.ToUrl())));
                }
                newLiabilityRows.Items.Add(new Row { Name = Strings.TaxOnSales, Cells = taxCollectedCells });

                var taxPaidCells = new List<Cell>();
                for (int i = 0; i < report.Periods.Length; i++)
                {
                    var movement = e.Where(x => x.Date >= report.Periods[i].FromDate && x.Date <= report.Periods[i].ToDate && x.IsTaxTransaction && !x.IsSale).Sum(x => x.BaseAmount);
                    taxPaidCells.Add(Make(movement, new Link(new TaxReconciliationTransactions { Business = Business, Referrer = Referrer, GeneralLedgerAccount = e.Key.Key, From = report.Periods[i].FromDate, To = report.Periods[i].ToDate, TaxPaid = true, AccountingBasis = report.AccountingMethod }.ToUrl())));
                }
                newLiabilityRows.Items.Add(new Row { Name = Strings.TaxOnPurchases, Cells = taxPaidCells });

                model.Rows.Items.Add(new Row { Name = Strings.NewTaxLiability, Rows = newLiabilityRows });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
