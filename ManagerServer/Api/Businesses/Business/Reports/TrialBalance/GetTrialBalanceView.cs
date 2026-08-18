using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Master;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TrialBalance
{
    [ProtoContract]
    internal sealed class GetTrialBalanceView : GetReportView<Model.TrialBalance>
    {
        protected override string DefaultTitle => Strings.TrialBalance;

        protected override ReportModel Build(Database business, Model.TrialBalance report)
        {
            var model = new ReportModel();
            if (!string.IsNullOrWhiteSpace(report.Title)) model.Title = report.Title;
            if (report.Periods == null) return model;
            model.Subtitle = string.Format(Strings.As_at_XXX, report.Periods[0].ToDate.ToLocalShortDisplayString());

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
                model.Columns.Add(new Column
                {
                    Name = columnName,
                    Subcolumns = new List<Column>
                    {
                        new Column { Name = Strings.Debit, IsBold = (i == 0) },
                        new Column { Name = Strings.Credit, IsBold = (i == 0) },
                    }
                });
            }

            var dates = new List<DateTime>();
            dates.AddRange(report.Periods.Select(x => x.FromDate).Where(x => x > DateTime.MinValue).Select(x => x.AddDays(-1)));
            dates.AddRange(report.Periods.Select(x => x.ToDate));
            dates = dates.Distinct().ToList();

            var chartOfAccounts = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).DisposeFixedAssets().DisposeIntangibleAssets();
            if (report.AccountingMethod == AccountingBasis.CashBasis)
                transactions = transactions.AutomaticallyMatchSalesInvoices().AutomaticallyMatchPurchaseInvoices().ConvertSalesInvoicesToCashBasis2(dates.ToArray()).ConvertPurchaseInvoicesToCashBasis2(dates.ToArray());

            var profit = new decimal[report.Periods.Length];
            var balanceSheetBalances = new Dictionary<Guid, decimal>[report.Periods.Length];
            var profitAndLossBalances = new Dictionary<Guid, decimal>[report.Periods.Length];
            for (int i = 0; i < report.Periods.Length; i++)
            {
                var period = report.Periods[i];
                var periodTransactions = transactions.Revaluate(period.FromDate, period.ToDate).Where(x => x.Date <= period.ToDate);
                if (period.Division.HasValue) periodTransactions = periodTransactions.Where(x => x.Division?.Key == period.Division.Value);
                balanceSheetBalances[i] = periodTransactions.GroupBy(x => x.BalanceSheetAccount.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.BaseAmount));
                profitAndLossBalances[i] = periodTransactions.Where(x => x.Date >= period.FromDate && x.ProfitAndLossAccount != null).GroupBy(x => x.ProfitAndLossAccount.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.BaseAmount));
                profit[i] = profitAndLossBalances[i].Sum(x => x.Value) * -1m;

                var interdivisionalLoan = balanceSheetBalances[i].Sum(x => x.Value);
                if (interdivisionalLoan != 0m)
                    balanceSheetBalances[i].Add(Model.Object.GetGuidByType(typeof(Model.BalanceSheetInterdivisionalLoan)), interdivisionalLoan * -1);
            }

            foreach (var group in chartOfAccounts.ProfitAndLossStatement)
            {
                var row = BuildProfitAndLossRow(group, report, profitAndLossBalances, includeFromDate: true, model.WholeNumbers);
                if (row != null) model.Rows.Items.Add(row);
            }

            {
                var cells = new List<Cell>();
                for (int i = 0; i < report.Periods.Length; i++)
                {
                    cells.Add(ReportNumberFormat.Cell(profit[i] > 0 ? (decimal?)profit[i] : null, NumberStyle.Currency, model.WholeNumbers));
                    cells.Add(ReportNumberFormat.Cell(profit[i] < 0 ? (decimal?)(profit[i] * -1) : null, NumberStyle.Currency, model.WholeNumbers));
                }
                model.Rows.Items.Add(new Row { Name = Strings.Net_profit_loss, MakeStandOut = true, Cells = cells });
            }

            foreach (var group in chartOfAccounts.BalanceSheet)
            {
                var row = BuildBalanceSheetRow(group, report, balanceSheetBalances, includeFromDate: false, model.WholeNumbers);
                if (row != null) model.Rows.Items.Add(row);
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            model.Prune(report.ExcludeZeroBalances);

            return model;
        }

        private Row BuildProfitAndLossRow(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item, Model.TrialBalance report, Dictionary<Guid, decimal>[] baseBalances, bool includeFromDate, bool wholeNumbers)
        {
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account account)
            {
                var cells = new List<Cell>();
                bool allZero = true;
                for (int i = 0; i < report.Periods.Length; i++)
                {
                    baseBalances[i].TryGetValue(account.Key, out decimal balance);
                    var link = new Link(new ManagerServer.HttpHandlers.Businesses.Business.Reports.TrialBalance.TrialBalanceTransactions
                    {
                        Business = Business,
                        Referrer = Referrer,
                        CashBasis = report.AccountingMethod == AccountingBasis.CashBasis,
                        GeneralLedgerAccount = item.Key,
                        From = includeFromDate ? report.Periods[i].FromDate : (DateTime?)null,
                        To = report.Periods[i].ToDate,
                        Division = report.Periods[i].Division,
                    }.ToUrl());
                    cells.Add(ReportNumberFormat.Cell(balance > 0 ? balance : (decimal?)null, NumberStyle.Currency, wholeNumbers, link));
                    cells.Add(ReportNumberFormat.Cell(balance < 0 ? (balance * -1) : (decimal?)null, NumberStyle.Currency, wholeNumbers, link));
                    if (balance != 0m) allZero = false;
                }
                if (account.Key == AccountKeys.Suspense && allZero) return null;
                if (account.Inactive && allZero) return null;
                return new Row { Name = report.AccountCodes ? account.NameWithCode : account.Name, Cells = cells };
            }
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group group)
            {
                var innerRows = new Rows { HideTotals = true };
                foreach (var e in group.Items)
                {
                    var row2 = BuildProfitAndLossRow(e, report, baseBalances, includeFromDate, wholeNumbers);
                    if (row2 != null) innerRows.Items.Add(row2);
                }
                return new Row { Name = group.Name, Rows = innerRows };
            }
            throw new InvalidOperationException();
        }

        private Row BuildBalanceSheetRow(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item, Model.TrialBalance report, Dictionary<Guid, decimal>[] baseBalances, bool includeFromDate, bool wholeNumbers)
        {
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account account)
            {
                var cells = new List<Cell>();
                bool allZero = true;
                for (int i = 0; i < report.Periods.Length; i++)
                {
                    baseBalances[i].TryGetValue(account.Key, out decimal balance);
                    var link = new Link(new ManagerServer.HttpHandlers.Businesses.Business.Reports.TrialBalance.TrialBalanceTransactions
                    {
                        Business = Business,
                        Referrer = Referrer,
                        CashBasis = report.AccountingMethod == AccountingBasis.CashBasis,
                        GeneralLedgerAccount = item.Key,
                        From = includeFromDate ? report.Periods[i].FromDate : (DateTime?)null,
                        To = report.Periods[i].ToDate,
                        Division = report.Periods[i].Division,
                    }.ToUrl());
                    cells.Add(ReportNumberFormat.Cell(balance > 0 ? balance : (decimal?)null, NumberStyle.Currency, wholeNumbers, link));
                    cells.Add(ReportNumberFormat.Cell(balance < 0 ? (balance * -1) : (decimal?)null, NumberStyle.Currency, wholeNumbers, link));
                    if (balance != 0m) allZero = false;
                }
                if (account.Key == AccountKeys.Suspense && allZero) return null;
                if (account.Inactive && allZero) return null;
                return new Row { Name = report.AccountCodes ? account.NameWithCode : account.Name, Cells = cells };
            }
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group group)
            {
                var innerRows = new Rows { HideTotals = true };
                foreach (var e in group.Items)
                {
                    var row2 = BuildBalanceSheetRow(e, report, baseBalances, includeFromDate, wholeNumbers);
                    if (row2 != null) innerRows.Items.Add(row2);
                }
                return new Row { Name = group.Name, Rows = innerRows };
            }
            throw new InvalidOperationException();
        }
    }
}
