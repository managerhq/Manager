using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatement;
using ManagerServer.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.ProfitAndLossStatementByGroup
{
    [ProtoContract]
    internal sealed class GetProfitAndLossStatementByGroupView : GetReportModelEndpoint<Model.ProfitAndLossStatementByGroup>
    {
        protected override string DefaultTitle => Strings.ProfitAndLossStatement;

        protected override V2.ReportModel2 Build(Database business, Model.ProfitAndLossStatementByGroup report)
        {
            if (!report.Group.HasValue) return null;

            var chartOfAccounts = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);
            var selectedGroup = FindGroup(chartOfAccounts.ProfitAndLossStatement, report.Group.Value);
            if (selectedGroup == null) return null;

            var wholeNumbers = report.Rounding == Rounding.On;
            var model = new V2.ReportModel2();
            model.Title = selectedGroup.Name;
            model.Subtitles.Add(string.Format(Strings.For_the_period_from_XXX_to_XXX, report.Periods[0].FromDate.ToLocalShortDisplayString(), report.Periods[0].ToDate.ToLocalShortDisplayString()));

            var accountingMethods = business.OfType<Model.SalesInvoice>().Any() || business.OfType<Model.PurchaseInvoice>().Any();
            if (accountingMethods)
            {
                model.Subtitles.Add(report.AccountingMethod == AccountingBasis.CashBasis ? Strings.CashBasis : Strings.AccrualBasis);
            }

            var trackingCodes = business.OfType<Model.Division>().ToDictionary(x => x.Key, x => x.Name);

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

            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).DisposeFixedAssets().DisposeIntangibleAssets();
            if (report.AccountingMethod == AccountingBasis.CashBasis)
            {
                transactions = transactions
                    .AutomaticallyMatchSalesInvoices()
                    .AutomaticallyMatchPurchaseInvoices()
                    .ConvertSalesInvoicesToCashBasis2(dates.ToArray())
                    .ConvertPurchaseInvoicesToCashBasis2(dates.ToArray());
            }

            var baseBalances = new Dictionary<Guid, decimal>[report.Periods.Length];
            for (int i = 0; i < report.Periods.Length; i++)
            {
                var period = report.Periods[i];
                var periodTransactions = transactions.Revaluate(period.FromDate, period.ToDate).Where(x => x.ProfitAndLossAccount != null && x.Date >= period.FromDate && x.Date <= period.ToDate);
                if (period.Division.HasValue) periodTransactions = periodTransactions.Where(x => x.Division?.Key == period.Division.Value);
                baseBalances[i] = periodTransactions.GroupBy(x => x.ProfitAndLossAccount.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.BaseAmount));
            }

            var row = BuildRow(selectedGroup, report, baseBalances, wholeNumbers);
            if (row != null)
            {
                var negate = IsUnderExpenseGroup(selectedGroup);

                var totalRow = V2.Row.Combine(row);
                totalRow.IsBold = true;

                if (negate)
                {
                    row.Negate();
                    totalRow.Negate();
                }

                model.Rows.Add(row);
                model.Rows.Add(totalRow);
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

        private static ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group FindGroup(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group[] roots, Guid key)
        {
            foreach (var root in roots)
            {
                if (root.Key == key) return root;
                var found = root.GetAllGroups().FirstOrDefault(g => g.Key == key);
                if (found != null) return found;
            }
            return null;
        }

        private static bool IsUnderExpenseGroup(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group group)
        {
            var current = group;
            while (current != null)
            {
                if (current.IsExpenseGroup) return true;
                current = current.Parent;
            }
            return false;
        }

        private V2.Row BuildRow(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item, Model.ProfitAndLossStatementByGroup report, Dictionary<Guid, decimal>[] baseBalances, bool wholeNumbers)
        {
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account account)
            {
                var cashBasis = report.AccountingMethod == AccountingBasis.CashBasis;
                var cells = new List<V2.Cell>();
                for (int i = 0; i < report.Periods.Length; i++)
                {
                    baseBalances[i].TryGetValue(account.Key, out decimal amount);
                    var link = new Link(new ProfitAndLossStatementTransactions
                    {
                        Business = Business,
                        Referrer = Referrer,
                        GeneralLedgerAccount = account.Key,
                        CashBasis = cashBasis,
                        From = report.Periods[i].FromDate,
                        To = report.Periods[i].ToDate,
                        Division = report.Periods[i].Division,
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
