using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatement;
using ManagerServer.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.ProfitAndLossStatement
{
    [ProtoContract]
    internal sealed class GetProfitAndLossStatementView : GetReportModelEndpoint<Model.ProfitAndLossStatement>
    {
        protected override string DefaultTitle => Strings.ProfitAndLossStatement;

        protected override V2.ReportModel2 Build(Database business, Model.ProfitAndLossStatement report)
        {
            var wholeNumbers = report.Rounding == Rounding.On;
            var model = new V2.ReportModel2();
            if (!string.IsNullOrWhiteSpace(report.Title)) model.Title = report.Title;
            model.Subtitles.Add(string.Format(Strings.For_the_period_from_XXX_to_XXX, report.Periods[0].FromDate.ToLocalShortDisplayString(), report.Periods[0].ToDate.ToLocalShortDisplayString()));

            var accountingMethods = business.OfType<ManagerServer.Model.SalesInvoice>().Any() || business.OfType<ManagerServer.Model.PurchaseInvoice>().Any();
            if (accountingMethods)
            {
                model.Subtitles.Add(report.AccountingMethod == AccountingBasis.CashBasis ? Strings.CashBasis : Strings.AccrualBasis);
            }

            var trackingCodes = business.OfType<ManagerServer.Model.Division>().ToDictionary(x => x.Key, x => x.Name);

            for (int i = 0; i < report.Periods.Length; i++)
            {
                var columnName = report.Periods[i].ToDate.ToLocalShortDisplayString();
                if (report.Periods[i].Division.HasValue && trackingCodes.ContainsKey(report.Periods[i].Division.Value)) columnName = trackingCodes[report.Periods[i].Division.Value];
                if (!string.IsNullOrWhiteSpace(report.Periods[i].ColumnName)) columnName = report.Periods[i].ColumnName;
                model.Columns.Add(new V2.Column { Name = columnName, IsBold = (i == 0) });
            }

            var dates = new List<DateTime>();
            dates.AddRange(report.Periods.Select(x => x.FromDate).Where(x => x > DateTime.MinValue).Select(x => x.AddDays(-1)));
            dates.AddRange(report.Periods.Select(x => x.ToDate));
            dates = dates.Distinct().ToList();

            var chartOfAccounts = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).DisposeFixedAssets().DisposeIntangibleAssets();
            if (report.AccountingMethod == AccountingBasis.CashBasis) transactions = transactions.AutomaticallyMatchSalesInvoices().AutomaticallyMatchPurchaseInvoices().ConvertSalesInvoicesToCashBasis2(dates.ToArray()).ConvertPurchaseInvoicesToCashBasis2(dates.ToArray());

            var baseBalances = new Dictionary<Guid, decimal>[report.Periods.Length];
            for (int i = 0; i < report.Periods.Length; i++)
            {
                var period = report.Periods[i];
                if (period.Division.HasValue)
                    baseBalances[i] = transactions.Revaluate(period.FromDate, period.ToDate).Where(x => x.ProfitAndLossAccount != null && x.Date >= period.FromDate && x.Date <= period.ToDate && x.Division?.Key == period.Division.Value).GroupBy(x => x.ProfitAndLossAccount.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.BaseAmount));
                else
                    baseBalances[i] = transactions.Revaluate(period.FromDate, period.ToDate).Where(x => x.ProfitAndLossAccount != null && x.Date >= period.FromDate && x.Date <= period.ToDate).GroupBy(x => x.ProfitAndLossAccount.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.BaseAmount));
            }

            // Cumulative running sum for IsSubtotal placeholders. Accumulates RAW cell values
            // (after the *-1 sign flip in MakeAccountCells, before any expense-group display
            // negation), matching V1's PrepareTotals-then-ApplyIsLess ordering.
            var runningSum = new decimal?[report.Periods.Length];

            foreach (var group in chartOfAccounts.ProfitAndLossStatement)
            {
                if (group.IsSubtotal)
                {
                    var subtotalCells = new List<V2.Cell>();
                    for (int i = 0; i < report.Periods.Length; i++)
                    {
                        subtotalCells.Add(MakeCell(runningSum[i]));
                    }
                    model.Rows.Add(new V2.Row { Name = group.Name, IsBold = true, Cells = subtotalCells });
                }
                else
                {
                    var row = BuildRow(group, report.Periods, baseBalances, report.AccountingMethod, report.AccountCodes, wholeNumbers);
                    if (row == null) continue;

                    // Accumulate to running sum BEFORE display negation. Group rows return
                    // auto-computed sums via Cells getter; leaf rows return their value cells.
                    var rowCells = row.Cells;
                    if (rowCells != null)
                    {
                        for (int i = 0; i < report.Periods.Length && i < rowCells.Count; i++)
                        {
                            if (rowCells[i]?.Value is decimal v) runningSum[i] = (runningSum[i] ?? 0m) + v;
                        }
                    }

                    if (group.IsExpenseGroup)
                    {
                        row.Name = Strings.Less + ": " + row.Name;
                        row.Negate();
                    }

                    model.Rows.Add(row);
                }
            }

            model.Footer = report.Footer;
            if (report.GroupsToCollapse != null && report.GroupsToCollapse.Length > 0)
            {
                model.Collapse(report.GroupsToCollapse.Select(k => (Guid?)k).ToArray());
            }
            if (wholeNumbers) model.Round();
            model.Prune(report.ExcludeZeroBalances);
            model.Format();
            return model;
        }

        private V2.Row BuildRow(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item, ManagerServer.Model.ProfitAndLossStatement.Period[] periods, Dictionary<Guid, decimal>[] baseBalances, AccountingBasis accountingBasis, bool showAccountCodes, bool wholeNumbers)
        {
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account account)
            {
                var cashBasis = accountingBasis == AccountingBasis.CashBasis;
                var cells = new List<V2.Cell>();
                for (int i = 0; i < periods.Length; i++)
                {
                    baseBalances[i].TryGetValue(account.Key, out decimal amount);
                    var link = new Link(new ProfitAndLossStatementTransactions
                    {
                        Business = Business,
                        Referrer = Referrer,
                        GeneralLedgerAccount = item.Key,
                        CashBasis = cashBasis,
                        From = periods[i].FromDate,
                        To = periods[i].ToDate,
                        Division = periods[i].Division,
                    }.ToUrl());
                    cells.Add(MakeCell(amount * -1m, link));
                }
                var allZero = cells.All(c =>
                {
                    var v = c.Value ?? 0m;
                    if (wholeNumbers) v = Math.Round(v, 0, MidpointRounding.AwayFromZero);
                    return v == 0m;
                });
                if (account.Inactive && allZero) return null;
                return new V2.Row { Key = account.Key, Name = showAccountCodes ? account.NameWithCode : account.Name, Cells = cells };
            }
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group group)
            {
                var items = new List<V2.Row>();
                foreach (var e in group.Items)
                {
                    var inner = BuildRow(e, periods, baseBalances, accountingBasis, showAccountCodes, wholeNumbers);
                    if (inner != null) items.Add(inner);
                }
                return new V2.Row { Key = group.Key, Name = group.Name, Rows = items };
            }
            throw new InvalidOperationException($"Unknown chart item type: {item.GetType().Name}");
        }

        private static V2.Cell MakeCell(decimal? value, Link link = null) => new V2.Cell
        {
            Value = value,
            Link = link,
            Style = V2.NumberStyle.CurrencyParentheses,
        };
    }
}
