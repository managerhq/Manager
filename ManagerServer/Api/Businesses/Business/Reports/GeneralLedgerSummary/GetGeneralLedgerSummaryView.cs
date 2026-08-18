using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Master;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.GeneralLedgerSummary
{
    [ProtoContract]
    internal sealed class GetGeneralLedgerSummaryView : GetReportView<Model.GeneralLedgerSummary>
    {
        protected override string DefaultTitle => Strings.GeneralLedgerSummary;

        protected override ReportModel Build(Database business, Model.GeneralLedgerSummary report)
        {
            var model = new ReportModel();
            var from = report.FromDate;
            var to = report.ToDate;

            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, from.ToLocalShortDisplayString(), to.ToLocalShortDisplayString());

            model.Columns.Add(new Column { Name = Strings.OpeningBalance, HideTotals = true });
            model.Columns.Add(new Column { Name = Strings.Total_debits, IsBold = true });
            model.Columns.Add(new Column { Name = Strings.Total_credits, IsBold = true });
            model.Columns.Add(new Column { Name = Strings.Net_movement, HideTotals = true });
            model.Columns.Add(new Column { Name = Strings.ClosingBalance, HideTotals = true });

            var chartOfAccounts = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);
            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .DisposeFixedAssets()
                .DisposeIntangibleAssets()
                .Revaluate(from, to)
                .Where(x => x.Date <= to)
                .ToArray();

            var profitAndLossAccounts = generalLedger.Where(x => x.ProfitAndLossAccount != null).ToLookup(x => x.ProfitAndLossAccount.Key);
            var balanceSheetAccounts = generalLedger.ToLookup(x => x.BalanceSheetAccount.Key);

            foreach (var group in chartOfAccounts.ProfitAndLossStatement)
            {
                var row = BuildProfitAndLossRow(group, profitAndLossAccounts, from, to, report.AccountCodes, false, model.WholeNumbers);
                if (row != null) model.Rows.Items.Add(row);
            }

            var profit = generalLedger.Where(x => x.ProfitAndLossAccount != null && x.Date >= from).Sum(x => x.BaseAmount) * -1m;

            model.Rows.Items.Add(new Row
            {
                MakeStandOut = true,
                Name = Strings.ProfitLossForThePeriod,
                Cells = new List<Cell>
                {
                    new Cell(),
                    ReportNumberFormat.Cell(profit > 0 ? profit : default(decimal?), NumberStyle.Currency, model.WholeNumbers),
                    ReportNumberFormat.Cell(profit < 0 ? profit * -1m : default(decimal?), NumberStyle.Currency, model.WholeNumbers),
                    new Cell(),
                    new Cell(),
                },
            });

            foreach (var group in chartOfAccounts.BalanceSheet)
            {
                var row = BuildBalanceSheetRow(group, balanceSheetAccounts, from, to, report.AccountCodes, true, model.WholeNumbers);
                if (row != null) model.Rows.Items.Add(row);
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            model.Prune(report.ExcludeZeroBalances);

            return model;
        }

        private Row BuildProfitAndLossRow(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item, ILookup<Guid, ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> generalLedger, DateTime from, DateTime to, bool showAccountCodes, bool balanceSheet, bool wholeNumbers)
        {
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account account)
            {
                decimal? openingBalance = null;
                if (balanceSheet) openingBalance = generalLedger[account.Key].Where(x => x.Date < from).Sum(x => x.BaseAmount);
                var totalDebits = generalLedger[account.Key].Where(x => x.Date >= from && x.BaseAmount > 0m).Sum(x => x.BaseAmount);
                var totalCredits = generalLedger[account.Key].Where(x => x.Date >= from && x.BaseAmount < 0m).Sum(x => x.BaseAmount) * -1;
                var netMovement = totalDebits - totalCredits;
                decimal? closingBalance = null;
                if (balanceSheet) closingBalance = openingBalance + netMovement;

                var cells = new List<Cell>
                {
                    ReportNumberFormat.Cell(openingBalance, NumberStyle.DebitCredit, wholeNumbers),
                    ReportNumberFormat.Cell(totalDebits, NumberStyle.Currency, wholeNumbers, new Link(new ManagerServer.HttpHandlers.Businesses.Business.Reports.GeneralLedgerSummary.GeneralLedgerSummaryTransactions { ProfitAndLossAccount = account.Key, Debits = true, Business = Business, Referrer = Referrer, From = from, To = to }.ToUrl())),
                    ReportNumberFormat.Cell(totalCredits, NumberStyle.Currency, wholeNumbers, new Link(new ManagerServer.HttpHandlers.Businesses.Business.Reports.GeneralLedgerSummary.GeneralLedgerSummaryTransactions { ProfitAndLossAccount = account.Key, Credits = true, Business = Business, Referrer = Referrer, From = from, To = to }.ToUrl())),
                    ReportNumberFormat.Cell(netMovement, NumberStyle.DebitCredit, wholeNumbers, new Link(new ManagerServer.HttpHandlers.Businesses.Business.Reports.GeneralLedgerSummary.GeneralLedgerSummaryTransactions { ProfitAndLossAccount = account.Key, Debits = true, Credits = true, Business = Business, Referrer = Referrer, From = from, To = to }.ToUrl())),
                    ReportNumberFormat.Cell(closingBalance, NumberStyle.DebitCredit, wholeNumbers),
                };

                if (account.Key == AccountKeys.Suspense && cells.All(c => (c.Value ?? 0m) == 0m)) return null;
                if (account.Inactive && cells.All(c => (c.Value ?? 0m) == 0m)) return null;

                return new Row
                {
                    Name = showAccountCodes ? account.NameWithCode : account.Name,
                    Cells = cells,
                };
            }
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group group2)
            {
                var items = new List<Row>();
                foreach (var e in group2.Items)
                {
                    var inner = BuildProfitAndLossRow(e, generalLedger, from, to, showAccountCodes, balanceSheet, wholeNumbers);
                    if (inner != null) items.Add(inner);
                }
                if (items.Count == 0) return null;
                return new Row
                {
                    Name = group2.Name,
                    Rows = new Rows { Items = items, HideTotals = true },
                };
            }
            throw new InvalidOperationException($"Unknown chart item type: {item.GetType().Name}");
        }

        private Row BuildBalanceSheetRow(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item, ILookup<Guid, ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> generalLedger, DateTime from, DateTime to, bool showAccountCodes, bool balanceSheet, bool wholeNumbers)
        {
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account account)
            {
                decimal? openingBalance = null;
                if (balanceSheet) openingBalance = generalLedger[account.Key].Where(x => x.Date < from).Sum(x => x.BaseAmount);
                var totalDebits = generalLedger[account.Key].Where(x => x.Date >= from && x.BaseAmount > 0m).Sum(x => x.BaseAmount);
                var totalCredits = generalLedger[account.Key].Where(x => x.Date >= from && x.BaseAmount < 0m).Sum(x => x.BaseAmount) * -1;
                var netMovement = totalDebits - totalCredits;
                decimal? closingBalance = null;
                if (balanceSheet) closingBalance = openingBalance + netMovement;

                var cells = new List<Cell>
                {
                    ReportNumberFormat.Cell(openingBalance, NumberStyle.DebitCredit, wholeNumbers),
                    ReportNumberFormat.Cell(totalDebits, NumberStyle.Currency, wholeNumbers, new Link(new ManagerServer.HttpHandlers.Businesses.Business.Reports.GeneralLedgerSummary.GeneralLedgerSummaryTransactions { BalanceSheetAccount = account.Key, Debits = true, Business = Business, Referrer = Referrer, From = from, To = to }.ToUrl())),
                    ReportNumberFormat.Cell(totalCredits, NumberStyle.Currency, wholeNumbers, new Link(new ManagerServer.HttpHandlers.Businesses.Business.Reports.GeneralLedgerSummary.GeneralLedgerSummaryTransactions { BalanceSheetAccount = account.Key, Credits = true, Business = Business, Referrer = Referrer, From = from, To = to }.ToUrl())),
                    ReportNumberFormat.Cell(netMovement, NumberStyle.DebitCredit, wholeNumbers, new Link(new ManagerServer.HttpHandlers.Businesses.Business.Reports.GeneralLedgerSummary.GeneralLedgerSummaryTransactions { BalanceSheetAccount = account.Key, Debits = true, Credits = true, Business = Business, Referrer = Referrer, From = from, To = to }.ToUrl())),
                    ReportNumberFormat.Cell(closingBalance, NumberStyle.DebitCredit, wholeNumbers),
                };

                if (account.Key == AccountKeys.Suspense && cells.All(c => (c.Value ?? 0m) == 0m)) return null;
                if (account.Inactive && cells.All(c => (c.Value ?? 0m) == 0m)) return null;

                return new Row
                {
                    Name = showAccountCodes ? account.NameWithCode : account.Name,
                    Cells = cells,
                };
            }
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group group2)
            {
                var items = new List<Row>();
                foreach (var e in group2.Items)
                {
                    var inner = BuildBalanceSheetRow(e, generalLedger, from, to, showAccountCodes, balanceSheet, wholeNumbers);
                    if (inner != null) items.Add(inner);
                }
                if (items.Count == 0) return null;
                return new Row
                {
                    Name = group2.Name,
                    Rows = new Rows { Items = items, HideTotals = true },
                };
            }
            throw new InvalidOperationException($"Unknown chart item type: {item.GetType().Name}");
        }
    }
}
