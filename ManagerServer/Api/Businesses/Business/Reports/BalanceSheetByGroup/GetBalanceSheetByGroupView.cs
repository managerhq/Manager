using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.BalanceSheet;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Master;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.BalanceSheetByGroup
{
    [ProtoContract]
    internal sealed class GetBalanceSheetByGroupView : GetReportModelEndpoint<Model.BalanceSheetByGroup>
    {
        protected override string DefaultTitle => Strings.BalanceSheet;

        protected override V2.ReportModel2 Build(Database business, Model.BalanceSheetByGroup report)
        {
            if (!report.Group.HasValue) return null;

            var chartOfAccounts = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);
            var selectedGroup = FindGroup(chartOfAccounts.BalanceSheet, report.Group.Value);
            if (selectedGroup == null) return null;

            var wholeNumbers = report.Rounding == Rounding.On;
            var model = new V2.ReportModel2();
            model.Title = selectedGroup.Name;
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

            var row = BuildRow(selectedGroup, report, baseBalances, wholeNumbers);
            if (row != null)
            {
                var negate = IsUnderCreditSide(selectedGroup);

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

        private static bool IsUnderCreditSide(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group group)
        {
            var current = group;
            while (current != null)
            {
                if (current.Key == ChartOfAccountGroups.Liabilities || current.Key == ChartOfAccountGroups.Equity) return true;
                current = current.Parent;
            }
            return false;
        }

        private V2.Row BuildRow(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item, Model.BalanceSheetByGroup report, Dictionary<Guid, decimal>[] baseBalances, bool wholeNumbers)
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
