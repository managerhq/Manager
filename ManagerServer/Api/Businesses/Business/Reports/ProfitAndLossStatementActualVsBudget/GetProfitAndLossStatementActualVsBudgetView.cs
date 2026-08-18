using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatementActualVsBudget;
using ManagerServer.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.ProfitAndLossStatementActualVsBudget
{
    [ProtoContract]
    internal sealed class GetProfitAndLossStatementActualVsBudgetView : GetReportView<Model.ProfitAndLossStatementActualVsBudget>
    {
        protected override string DefaultTitle => Strings.ProfitAndLossStatementActualVsBudget;

        protected override ReportModel Build(Database business, Model.ProfitAndLossStatementActualVsBudget report)
        {
            var model = new ReportModel();
            model.WholeNumbers = report.RoundDecimals;
            if (!string.IsNullOrWhiteSpace(report.Title)) model.Title = report.Title;
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            if (report.Division.HasValue)
            {
                var trackingCode = business.SingleOrDefault<ManagerServer.Model.Division>(report.Division.Value);
                if (trackingCode != null)
                {
                    model.Subtitle2 = trackingCode.Name;
                }
            }

            model.Columns.Add(new Column { Name = Strings.Actual, IsBold = true });
            model.Columns.Add(new Column { Name = Strings.Budget });
            model.Columns.Add(new Column { Name = Strings.Percentage, HideTotals = true });
            model.Columns.Add(new Column { Name = Strings.Remaining, HideTotals = true });

            var chartOfAccounts = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).DisposeFixedAssets().DisposeIntangibleAssets().Revaluate(report.FromDate, report.ToDate);
            if (report.AccountingMethod == AccountingBasis.CashBasis) transactions = transactions.AutomaticallyMatchSalesInvoices().AutomaticallyMatchPurchaseInvoices().ConvertSalesInvoicesToCashBasis2(report.FromDate.SafeAddDays(-1), report.ToDate).ConvertPurchaseInvoicesToCashBasis2(report.FromDate.SafeAddDays(-1), report.ToDate);

            Dictionary<Guid, decimal> baseBalances;
            if (report.Division.HasValue)
                baseBalances = transactions.Where(x => x.ProfitAndLossAccount != null && x.Date >= report.FromDate && x.Date <= report.ToDate && x.Division?.Key == report.Division.Value).GroupBy(x => x.ProfitAndLossAccount.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.BaseAmount));
            else
                baseBalances = transactions.Where(x => x.ProfitAndLossAccount != null && x.Date >= report.FromDate && x.Date <= report.ToDate).GroupBy(x => x.ProfitAndLossAccount.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.BaseAmount));

            foreach (var group in chartOfAccounts.ProfitAndLossStatement)
            {
                if (group.IsSubtotal)
                {
                    model.Rows.Items.Add(new Row { IsTotalRow = true, Name = group.Name });
                }
                else
                {
                    var row = BuildRow(group, report, baseBalances, model.WholeNumbers);
                    if (row != null) model.Rows.Items.Add(row);
                }
            }

            model.Footer = report.Footer;
            model.Prune(report.ExcludeZeroBalances);

            return model;
        }

        private Row BuildRow(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item, Model.ProfitAndLossStatementActualVsBudget report, Dictionary<Guid, decimal> baseBalances, bool wholeNumbers)
        {
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account account)
            {
                baseBalances.TryGetValue(account.Key, out decimal actualAmount);
                actualAmount *= -1m;
                var budgetAmount = report.Lines.Where(x => x.Account == account.Key).Sum(x => x.Amount);

                var percentage = 0m;
                if (budgetAmount != 0m)
                {
                    if (actualAmount >= 0m && budgetAmount >= 0m)
                    {
                        if (actualAmount == 0m) percentage = 0m;
                        else percentage = Math.Round(actualAmount / (budgetAmount / 100m), 0, MidpointRounding.AwayFromZero);
                    }
                    if (actualAmount <= 0m && budgetAmount <= 0m)
                    {
                        if (actualAmount == 0m) percentage = 0m;
                        else percentage = Math.Round(actualAmount / (budgetAmount / 100m), 0, MidpointRounding.AwayFromZero) * -1m;
                    }
                }

                var cells = new List<Cell>
                {
                    ReportNumberFormat.Cell(actualAmount, NumberStyle.CurrencyParentheses, wholeNumbers, new Link(new ProfitAndLossStatementActualVsBudgetTransactions { Business = Business, Referrer = Referrer, GeneralLedgerAccount = item.Key, From = report.FromDate, To = report.ToDate, Division = report.Division, CashBasis = (report.AccountingMethod == AccountingBasis.CashBasis) }.ToUrl())),
                    ReportNumberFormat.Cell(budgetAmount, NumberStyle.CurrencyParentheses, wholeNumbers),
                    ReportNumberFormat.Cell(percentage, NumberStyle.Percentage, wholeNumbers),
                    ReportNumberFormat.Cell(budgetAmount - actualAmount, NumberStyle.CurrencyParentheses, wholeNumbers),
                };

                if (account.Inactive && actualAmount == 0m && budgetAmount == 0m) return null;
                return new Row { Name = account.NameWithCode, Cells = cells };
            }
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group group)
            {
                var items = new List<Row>();
                foreach (var e in group.Items)
                {
                    var inner = BuildRow(e, report, baseBalances, wholeNumbers);
                    if (inner != null) items.Add(inner);
                }
                return new Row
                {
                    Name = group.Name,
                    Rows = new Rows { Items = items, IsLess = group.IsExpenseGroup },
                };
            }
            throw new InvalidOperationException($"Unknown chart item type: {item.GetType().Name}");
        }
    }
}
