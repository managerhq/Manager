using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.BalanceSheet;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Master;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.BalanceSheet
{
    [ProtoContract]
    internal sealed class GetBalanceSheetView : GetReportModelEndpoint<Model.BalanceSheet>
    {
        protected override string DefaultTitle => Strings.BalanceSheet;

        protected override V2.ReportModel2 Build(Database business, Model.BalanceSheet report)
        {
            var wholeNumbers = report.Rounding == Rounding.On;
            var model = new V2.ReportModel2();
            if (!string.IsNullOrWhiteSpace(report.Title)) model.Title = report.Title;
            model.Subtitles.Add(string.Format(Strings.As_at_XXX, report.Periods[0].Date.ToLocalShortDisplayString()));

            var accountingMethods = business.OfType<Model.SalesInvoice>().Any() || business.OfType<Model.PurchaseInvoice>().Any();
            if (accountingMethods)
            {
                model.Subtitles.Add(report.AccountingMethod == AccountingBasis.CashBasis ? Strings.CashBasis : Strings.AccrualBasis);
            }

            for (int i = 0; i < report.Periods.Length; i++)
            {
                var columnName = string.IsNullOrWhiteSpace(report.Periods[i].ColumnName)
                    ? report.Periods[i].Date.ToLocalShortDisplayString()
                    : report.Periods[i].ColumnName;
                model.Columns.Add(new V2.Column { Name = columnName, IsBold = (i == 0) });
            }

            var dates = report.Periods.Select(x => x.Date).Distinct().ToArray();

            var chartOfAccounts = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).DisposeFixedAssets().DisposeIntangibleAssets();
            if (report.AccountingMethod == AccountingBasis.CashBasis)
            {
                transactions = transactions
                    .AutomaticallyMatchSalesInvoices()
                    .AutomaticallyMatchPurchaseInvoices()
                    .ConvertSalesInvoicesToCashBasis2(dates)
                    .ConvertPurchaseInvoicesToCashBasis2(dates);
            }

            var baseBalances = new Dictionary<Guid, decimal>[report.Periods.Length];
            for (int i = 0; i < report.Periods.Length; i++)
            {
                var period = report.Periods[i];
                var periodTransactions = transactions.Revaluate(period.Date).Where(x => x.Date <= period.Date);
                if (period.Division.HasValue) periodTransactions = periodTransactions.Where(x => x.Division?.Key == period.Division.Value);
                baseBalances[i] = periodTransactions.GroupBy(x => x.BalanceSheetAccount.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.BaseAmount));
                var interdivisionalLoan = baseBalances[i].Sum(x => x.Value);
                if (interdivisionalLoan != 0m)
                {
                    baseBalances[i].Add(Model.Object.GetGuidByType(typeof(Model.BalanceSheetInterdivisionalLoan)), interdivisionalLoan * -1);
                }
            }

            var assets = chartOfAccounts.BalanceSheet.Single(x => x.Key == ChartOfAccountGroups.Assets);
            var liabilities = chartOfAccounts.BalanceSheet.Single(x => x.Key == ChartOfAccountGroups.Liabilities);
            var equity = chartOfAccounts.BalanceSheet.Single(x => x.Key == ChartOfAccountGroups.Equity);

            if (report.Layout == BalanceSheetLayout.AssetsLiabilitiesEqualsEquity)
            {
                AddSection(model, Strings.Net_assets, assets, liabilities, report, baseBalances, wholeNumbers);
                AddSection(model, Strings.Total_equity, equity, null, report, baseBalances, wholeNumbers);
            }
            if (report.Layout == BalanceSheetLayout.AssetsEqualsLiabilitiesEquity)
            {
                AddSection(model, Strings.Total_assets, assets, null, report, baseBalances, wholeNumbers);
                AddSection(model, Strings.Total_liabilities_and_equity, liabilities, equity, report, baseBalances, wholeNumbers);
            }
            if (report.Layout == BalanceSheetLayout.AssetsEqualsEquityLiabilities)
            {
                AddSection(model, Strings.Total_assets, assets, null, report, baseBalances, wholeNumbers);
                AddSection(model, Strings.Total_liabilities_and_equity, equity, liabilities, report, baseBalances, wholeNumbers);
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

        private void AddSection(V2.ReportModel2 model, string totalText, ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item firstItem, ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item secondItem, Model.BalanceSheet report, Dictionary<Guid, decimal>[] baseBalances, bool wholeNumbers)
        {
            var sections = new List<(V2.Row row, bool neg)>();

            var firstRow = BuildRow(firstItem, report, baseBalances, wholeNumbers);
            if (firstRow != null) sections.Add((firstRow, ShouldNegateForDisplay(firstItem)));

            if (secondItem != null)
            {
                var secondRow = BuildRow(secondItem, report, baseBalances, wholeNumbers);
                if (secondRow != null) sections.Add((secondRow, ShouldNegateForDisplay(secondItem)));
            }

            if (sections.Count == 0) return;

            var totalRow = V2.Row.Combine(sections.Select(s => s.row).ToArray());
            totalRow.Name = totalText;
            totalRow.IsBold = true;

            foreach (var (row, neg) in sections) if (neg) row.Negate();
            if (sections.All(s => s.neg)) totalRow.Negate();

            foreach (var (row, _) in sections) model.Rows.Add(row);
            model.Rows.Add(totalRow);
        }

        private static bool ShouldNegateForDisplay(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item)
            => item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group g
                && (g.Key == ChartOfAccountGroups.Liabilities || g.Key == ChartOfAccountGroups.Equity);

        private V2.Row BuildRow(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item, Model.BalanceSheet report, Dictionary<Guid, decimal>[] baseBalances, bool wholeNumbers)
        {
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account account)
            {
                var cashBasis = report.AccountingMethod == AccountingBasis.CashBasis;
                var interdivisionalLoanKey = Model.Object.GetGuidByType(typeof(Model.BalanceSheetInterdivisionalLoan));
                var cells = new List<V2.Cell>();
                for (int i = 0; i < report.Periods.Length; i++)
                {
                    baseBalances[i].TryGetValue(account.Key, out decimal amount);
                    Link link = null;
                    if (account.Key != interdivisionalLoanKey)
                    {
                        link = new Link(new BalanceSheetTransactions
                        {
                            Business = Business,
                            Referrer = Referrer,
                            GeneralLedgerAccount = account.Key,
                            CashBasis = cashBasis,
                            To = report.Periods[i].Date,
                            Division = report.Periods[i].Division,
                        }.ToUrl());
                    }
                    cells.Add(MakeCell(amount, link));
                }
                var allZero = cells.All(c =>
                {
                    var v = c.Value ?? 0m;
                    if (wholeNumbers) v = Math.Round(v, 0, MidpointRounding.AwayFromZero);
                    return v == 0m;
                });
                if (account.Key == AccountKeys.Suspense && allZero) return null;
                if (account.Inactive && allZero) return null;
                return new V2.Row { Key = account.Key, Name = report.AccountCodes ? account.NameWithCode : account.Name, Cells = cells };
            }
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group group)
            {
                var items = new List<V2.Row>();
                foreach (var e in group.Items)
                {
                    var innerRow = BuildRow(e, report, baseBalances, wholeNumbers);
                    if (innerRow != null) items.Add(innerRow);
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
