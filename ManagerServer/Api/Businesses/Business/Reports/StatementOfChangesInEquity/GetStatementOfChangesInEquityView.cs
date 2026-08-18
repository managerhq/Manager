using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Master;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.StatementOfChangesInEquity
{
    [ProtoContract]
    internal sealed class GetStatementOfChangesInEquityView : GetReportView<Model.StatementOfChangesInEquity>
    {
        protected override string DefaultTitle => Strings.StatementOfChangesInEquity;

        protected override ReportModel Build(Database business, Model.StatementOfChangesInEquity report)
        {
            var model = new ReportModel();
            if (!string.IsNullOrWhiteSpace(report.Title)) model.Title = report.Title;
            if (report.Rounding == Rounding.On) model.WholeNumbers = true;
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.Periods[0].FromDate.ToLocalShortDisplayString(), report.Periods[0].ToDate.ToLocalShortDisplayString());

            var accountingMethods = business.OfType<Model.SalesInvoice>().Any() || business.OfType<Model.PurchaseInvoice>().Any();
            if (accountingMethods)
            {
                model.Subtitle2 = Strings.AccrualBasis;
                if (report.AccountingMethod == AccountingBasis.CashBasis) model.Subtitle2 = Strings.CashBasis;
            }

            for (int i = 0; i < report.Periods.Length; i++)
            {
                var columnName = report.Periods[i].ToDate.ToLocalShortDisplayString();
                if (!string.IsNullOrWhiteSpace(report.Periods[i].ColumnName)) columnName = report.Periods[i].ColumnName;
                model.Columns.Add(new Column { Name = columnName, IsBold = (i == 0) });
            }

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.CurrencyParentheses, model.WholeNumbers, link);

            var dates = new List<DateTime>();
            dates.AddRange(report.Periods.Select(x => x.FromDate).Where(x => x > DateTime.MinValue).Select(x => x.AddDays(-1)));
            dates.AddRange(report.Periods.Select(x => x.ToDate));
            dates = dates.Distinct().ToList();

            var equity = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business).BalanceSheet.Single(x => x.Key == ChartOfAccountGroups.Equity);
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).DisposeFixedAssets().DisposeIntangibleAssets();
            if (report.AccountingMethod == AccountingBasis.CashBasis)
                transactions = transactions.AutomaticallyMatchSalesInvoices().ConvertSalesInvoicesToCashBasis2(dates.ToArray()).AutomaticallyMatchPurchaseInvoices().ConvertPurchaseInvoicesToCashBasis2(dates.ToArray());

            foreach (var e in equity.GetAllAccounts())
            {
                var innerRows = new Rows { TotalText = Strings.BalanceAtEndOfPeriod };

                // Opening balance row
                var openingCells = new List<Cell>();
                bool openingAllZero = true;
                for (int i = 0; i < report.Periods.Length; i++)
                {
                    var openingBalance = transactions.Where(x => x.GeneralLedgerAccount.Key == e.Key).Where(x => x.Date < report.Periods[i].FromDate).Sum(x => x.BaseAmount) * -1;
                    if (e.Key == AccountKeys.RetainedEarnings)
                    {
                        var profit = 0m;
                        if (report.Periods[i].FromDate > DateTime.MinValue)
                            profit = transactions.Revaluate(report.Periods[i].FromDate.AddDays(-1)).Where(x => x.GeneralLedgerAccount.IsProfitAndLossAccount).Where(x => x.Date < report.Periods[i].FromDate).Sum(x => x.BaseAmount);
                        openingBalance -= profit;
                    }
                    openingCells.Add(Make(openingBalance));
                    if (openingBalance != 0m) openingAllZero = false;
                }
                if (!openingAllZero)
                    innerRows.Items.Add(new Row { Name = Strings.BalanceAtBeginningOfPeriod, Cells = openingCells });

                // Profit/loss row for retained earnings
                if (e.Key == AccountKeys.RetainedEarnings)
                {
                    var profitCells = new List<Cell>();
                    for (int i = 0; i < report.Periods.Length; i++)
                    {
                        var profit = transactions.Revaluate(report.Periods[i].FromDate, report.Periods[i].ToDate).Where(x => x.GeneralLedgerAccount.IsProfitAndLossAccount).Where(x => x.Date >= report.Periods[i].FromDate && x.Date <= report.Periods[i].ToDate).Sum(x => x.BaseAmount) * -1;
                        profitCells.Add(Make(profit));
                    }
                    innerRows.Items.Add(new Row { Name = Strings.ProfitLossForThePeriod, Cells = profitCells });
                }

                // Transaction group rows
                foreach (var transactionGroup in transactions.Where(x => x.GeneralLedgerAccount.Key == e.Key && x.BaseAmount != 0m).GroupBy(x => x.TransactionLine?.GetDescriptionOrNull(x.Transaction) ?? x.Transaction?.GetDescriptionOrNull() ?? x.Transaction?.GetName()).OrderBy(x => x.Key))
                {
                    var cells = new List<Cell>();
                    for (int i = 0; i < report.Periods.Length; i++)
                    {
                        var movement = transactionGroup.Where(x => x.Date >= report.Periods[i].FromDate && x.Date <= report.Periods[i].ToDate).Sum(x => x.BaseAmount) * -1;
                        cells.Add(Make(movement, new Link(new ManagerServer.HttpHandlers.Businesses.Business.Reports.StatementOfChangesInEquity.StatementOfChangesInEquityTransactions
                        {
                            Business = Business,
                            Referrer = Referrer,
                            Account = e.Key,
                            From = report.Periods[i].FromDate,
                            To = report.Periods[i].ToDate,
                            AccountingBasis = report.AccountingMethod,
                            Description = transactionGroup.Key
                        }.ToUrl())));
                    }
                    innerRows.Items.Add(new Row { Name = transactionGroup.Key, Cells = cells });
                }

                model.Rows.Items.Add(new Row { Name = e.Name, Rows = innerRows });
            }

            var equityName = Strings.Equity;
            var equity2 = business.Single<Model.Equity>();
            if (!string.IsNullOrWhiteSpace(equity2.Name))
                equityName = equity2.Name;

            model.Rows.Items.Add(new Row { IsTotalRow = true, Name = string.Format(Strings.Total_XXX, equityName) });

            model.Footer = report.Footer;
            model.Prune(report.ExcludeZeroBalances);
            return model;
        }
    }
}
