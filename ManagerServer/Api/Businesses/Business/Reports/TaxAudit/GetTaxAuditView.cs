using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxAudit;
using ManagerServer.Model.Enums;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxAudit
{
    [ProtoContract]
    internal sealed class GetTaxAuditView : GetReportView<Model.TaxAudit>
    {
        protected override string DefaultTitle => Strings.TaxAudit;

        protected override ReportModel Build(Database business, Model.TaxAudit report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            var accountingMethods = business.OfType<ManagerServer.Model.SalesInvoice>().Any() || business.OfType<ManagerServer.Model.PurchaseInvoice>().Any();
            if (accountingMethods)
            {
                model.Subtitle2 = Strings.AccrualBasis;
                if (report.AccountingMethod == AccountingBasis.CashBasis) model.Subtitle2 = Strings.CashBasis;
            }

            var chartOfAccounts = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);
            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business);
            if (report.AccountingMethod == AccountingBasis.CashBasis) generalLedger = generalLedger.AutomaticallyMatchSalesInvoices().ConvertSalesInvoicesToCashBasis2(report.FromDate.AddDays(-1), report.ToDate).AutomaticallyMatchPurchaseInvoices().ConvertPurchaseInvoicesToCashBasis2(report.FromDate.AddDays(-1), report.ToDate);
            var transactions = generalLedger.Where(x => x.Date >= report.FromDate && x.Date <= report.ToDate);
            var accounts = transactions.GroupBy(x => x.GeneralLedgerAccount.Key).ToDictionary(x => x.Key, x => x.GroupBy(y => y.TaxCode).ToDictionary(y => y.Key?.Key ?? Guid.Empty, y => y.Sum(z => z.BaseAmount)));

            var taxCodes = transactions.Where(x => x.TaxCode != null).Select(x => x.TaxCode).Distinct().ToArray();

            model.Columns.Add(new Column { Name = Strings.NoTax });
            foreach (var e in taxCodes.OrderBy(x => x.Name)) model.Columns.Add(new Column { Name = e.Name });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.DebitCredit, model.WholeNumbers, link);

            foreach (var e in chartOfAccounts.ProfitAndLossStatement)
            {
                var groupRows = new Rows { HideTotals = true };
                var groupAccounts = e.GetAllAccounts();
                if (groupAccounts.Any())
                {
                    foreach (var e2 in groupAccounts)
                    {
                        if (accounts.ContainsKey(e2.Key))
                        {
                            var account = accounts[e2.Key];
                            var cells = new System.Collections.Generic.List<Cell>();
                            if (account.ContainsKey(Guid.Empty))
                            {
                                cells.Add(Make(account[Guid.Empty], new Link(new TaxAuditTransactions { Business = Business, Referrer = Referrer, GeneralLedgerAccount = e2.Key, From = report.FromDate, To = report.ToDate, AccountingBasis = report.AccountingMethod }.ToUrl())));
                            }
                            else
                            {
                                cells.Add(new Cell());
                            }
                            foreach (var e3 in taxCodes.OrderBy(x => x.Name))
                            {
                                if (account.ContainsKey(e3.Key))
                                {
                                    cells.Add(Make(account[e3.Key], new Link(new TaxAuditTransactions { Business = Business, Referrer = Referrer, GeneralLedgerAccount = e2.Key, From = report.FromDate, To = report.ToDate, AccountingBasis = report.AccountingMethod, TaxCode = e3.Key }.ToUrl())));
                                }
                                else
                                {
                                    cells.Add(new Cell());
                                }
                            }
                            groupRows.Items.Add(new Row { Name = e2.Name, Cells = cells });
                        }
                    }
                }
                model.Rows.Items.Add(new Row { Name = e.Name, Rows = groupRows });
            }

            foreach (var e in chartOfAccounts.BalanceSheet)
            {
                var groupRows = new Rows { HideTotals = true };
                var groupAccounts = e.GetAllAccounts();
                if (groupAccounts.Any())
                {
                    foreach (var e2 in groupAccounts)
                    {
                        if (accounts.ContainsKey(e2.Key))
                        {
                            var account = accounts[e2.Key];
                            var cells = new System.Collections.Generic.List<Cell>();
                            if (account.ContainsKey(Guid.Empty))
                            {
                                cells.Add(Make(account[Guid.Empty], new Link(new TaxAuditTransactions { Business = Business, Referrer = Referrer, GeneralLedgerAccount = e2.Key, From = report.FromDate, To = report.ToDate, AccountingBasis = report.AccountingMethod }.ToUrl())));
                            }
                            else
                            {
                                cells.Add(new Cell());
                            }
                            foreach (var e3 in taxCodes.OrderBy(x => x.Name))
                            {
                                if (account.ContainsKey(e3.Key))
                                {
                                    cells.Add(Make(account[e3.Key], new Link(new TaxAuditTransactions { Business = Business, Referrer = Referrer, GeneralLedgerAccount = e2.Key, From = report.FromDate, To = report.ToDate, AccountingBasis = report.AccountingMethod, TaxCode = e3.Key }.ToUrl())));
                                }
                                else
                                {
                                    cells.Add(new Cell());
                                }
                            }
                            groupRows.Items.Add(new Row { Name = e2.Name, Cells = cells });
                        }
                    }
                }
                model.Rows.Items.Add(new Row { Name = e.Name, Rows = groupRows });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
