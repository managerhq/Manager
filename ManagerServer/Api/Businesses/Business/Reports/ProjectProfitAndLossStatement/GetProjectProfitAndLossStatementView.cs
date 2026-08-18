using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.ProjectProfitAndLossStatement
{
    [ProtoContract]
    internal sealed class GetProjectProfitAndLossStatementView : GetReportModelEndpoint<Model.Project>
    {
        protected override string DefaultTitle => Strings.Project;

        protected override V2.ReportModel2 Build(Database business, Model.Project report)
        {
            var model = new V2.ReportModel2();
            model.Subtitles.Add(report.Name);

            var projectTransactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.Project?.Key == Key)
                .ToList();

            if (projectTransactions.Any())
            {
                model.Subtitles.Add(string.Format(Strings.For_the_period_from_XXX_to_XXX, projectTransactions.Min(x => x.Date).ToLocalShortDisplayString(), projectTransactions.Max(x => x.Date).ToLocalShortDisplayString()));
            }

            model.Columns.Add(new V2.Column());

            var chartOfAccounts = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);
            var baseBalances = projectTransactions
                .Where(x => x.ProfitAndLossAccount != null)
                .GroupBy(x => x.ProfitAndLossAccount.Key)
                .ToDictionary(x => x.Key, x => x.Sum(y => y.BaseAmount));

            // Running sum for IsSubtotal placeholders. Accumulates RAW cell values
            // (after the *-1 sign flip), before any expense-group display negation.
            decimal? runningSum = null;

            foreach (var group in chartOfAccounts.ProfitAndLossStatement)
            {
                if (group.IsSubtotal)
                {
                    model.Rows.Add(new V2.Row { Name = group.Name, IsBold = true, Cells = new List<V2.Cell> { MakeCell(runningSum) } });
                }
                else
                {
                    var row = BuildRow(group, baseBalances);
                    if (row == null) continue;

                    if (row.Cells?.FirstOrDefault()?.Value is decimal v) runningSum = (runningSum ?? 0m) + v;

                    if (group.IsExpenseGroup)
                    {
                        row.Name = Strings.Less + ": " + row.Name;
                        row.Negate();
                    }

                    model.Rows.Add(row);
                }
            }

            model.Prune(true);
            model.Format();
            return model;
        }

        private V2.Row BuildRow(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item, Dictionary<Guid, decimal> baseBalances)
        {
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account account)
            {
                baseBalances.TryGetValue(account.Key, out decimal amount);
                if (account.Inactive && amount == 0m) return null;
                return new V2.Row { Key = account.Key, Name = account.Name, Cells = new List<V2.Cell> { MakeCell(amount * -1m) } };
            }
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group group)
            {
                var items = new List<V2.Row>();
                foreach (var e in group.Items)
                {
                    var inner = BuildRow(e, baseBalances);
                    if (inner != null) items.Add(inner);
                }
                return new V2.Row { Key = group.Key, Name = group.Name, Rows = items };
            }
            throw new InvalidOperationException($"Unknown chart item type: {item.GetType().Name}");
        }

        private static V2.Cell MakeCell(decimal? value) => new V2.Cell
        {
            Value = value,
            Style = V2.NumberStyle.CurrencyParentheses,
        };
    }
}
