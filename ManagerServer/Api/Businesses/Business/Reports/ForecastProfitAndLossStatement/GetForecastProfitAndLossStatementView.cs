using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.ForecastProfitAndLossStatement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.ForecastProfitAndLossStatement
{
    [ProtoContract]
    internal sealed class GetForecastProfitAndLossStatementView : GetReportView<Model.ForecastProfitAndLossStatement>
    {
        protected override string DefaultTitle => Strings.ForecastProfitAndLossStatement;

        protected override ReportModel Build(Database business, Model.ForecastProfitAndLossStatement report)
        {
            var model = new ReportModel();
            if (!string.IsNullOrWhiteSpace(report.Title)) model.Title = report.Title;
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.Periods[0].FromDate.ToLocalShortDisplayString(), report.Periods[0].ToDate.ToLocalShortDisplayString());

            for (int i = 0; i < report.Periods.Length; i++)
            {
                var columnName = report.Periods[i].ToDate.ToLocalShortDisplayString();
                if (!string.IsNullOrWhiteSpace(report.Periods[i].ColumnName)) columnName = report.Periods[i].ColumnName;
                model.Columns.Add(new Column { Name = columnName, IsBold = (i == 0) });
            }

            var minDate = report.Periods.Select(x => x.FromDate).Where(x => x > DateTime.MinValue).Min();
            var maxDate = report.Periods.Select(x => x.ToDate).Where(x => x > DateTime.MinValue).Max();

            var chartOfAccounts = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);
            var baseCurrency = business.Single<ManagerServer.Model.BaseCurrency>();
            var transactions = business.OfType<ManagerServer.Model.Forecast>().SelectMany(x => x.GetForecastTransactions(baseCurrency, minDate, maxDate)).ToArray();

            var baseBalances = new Dictionary<Guid, decimal>[report.Periods.Length];
            for (int i = 0; i < report.Periods.Length; i++)
            {
                var period = report.Periods[i];
                baseBalances[i] = transactions.Where(x => x.Date >= period.FromDate && x.Date <= period.ToDate).GroupBy(x => x.Account).ToDictionary(x => x.Key, x => x.Sum(y => y.Amount));
            }

            foreach (var group in chartOfAccounts.ProfitAndLossStatement)
            {
                if (group.IsSubtotal)
                {
                    model.Rows.Items.Add(new Row { IsTotalRow = true, Name = group.Name });
                }
                else
                {
                    var row = BuildRow(group, report.Periods, baseBalances, report.AccountCodes, model.WholeNumbers);
                    if (row != null) model.Rows.Items.Add(row);
                }
            }

            model.Footer = report.Footer;
            model.Prune(report.ExcludeZeroBalances);

            // TODO: ViewModel lacks CustomButton — legacy builds a CopyToBudget button here which is not ported

            return model;
        }

        public static Item[] GetItems(string fileId, ManagerServer.Model.ForecastProfitAndLossStatement report)
        {
            var list = new List<Item>();

            var database = ApplicationData.Instance.Businesses.Get(fileId);
            var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();

            var period = report.Periods[0];
            var transactions = database.OfType<ManagerServer.Model.Forecast>().SelectMany(x => x.GetForecastTransactions(baseCurrency, period.FromDate, period.ToDate)).ToArray();
            var baseBalances = transactions.Where(x => x.Date >= period.FromDate && x.Date <= period.ToDate).GroupBy(x => x.Account).ToDictionary(x => x.Key, x => x.Sum(y => y.Amount));

            var chartOfAccounts = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(fileId);

            foreach (var group in chartOfAccounts.ProfitAndLossStatement)
            {
                Collect(group, baseBalances, list);
            }

            return list.ToArray();
        }

        private static void Collect(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item, Dictionary<Guid, decimal> baseBalances, List<Item> list)
        {
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account account)
            {
                baseBalances.TryGetValue(account.Key, out decimal amount);
                list.Add(new Item() { Account = account.Key, Amount = amount * -1m });
                return;
            }
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group group)
            {
                foreach (var e in group.Items)
                {
                    Collect(e, baseBalances, list);
                }
            }
            // subtotal items contribute no budget line
        }

        public sealed class Item
        {
            public Guid Account;
            public decimal Amount;
        }

        private Row BuildRow(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item item, ManagerServer.Model.ForecastProfitAndLossStatement.Period[] periods, Dictionary<Guid, decimal>[] baseBalances, bool showAccountCodes, bool wholeNumbers)
        {
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Account account)
            {
                var cells = new List<Cell>();
                for (int i = 0; i < periods.Length; i++)
                {
                    baseBalances[i].TryGetValue(account.Key, out decimal amount);
                    cells.Add(ReportNumberFormat.Cell(amount * -1m, NumberStyle.CurrencyParentheses, wholeNumbers, new Link(new ForecastProfitAndLossStatementTransactions { Business = Business, Referrer = Referrer, Account = item.Key, From = periods[i].FromDate, To = periods[i].ToDate, ReverseSign = true }.ToUrl())));
                }
                if (account.Inactive && cells.All(c => (c.Value ?? 0m) == 0m)) return null;
                return new Row
                {
                    Name = showAccountCodes ? account.NameWithCode : account.Name,
                    Cells = cells,
                };
            }
            if (item is ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group group)
            {
                var items = new List<Row>();
                foreach (var e in group.Items)
                {
                    var inner = BuildRow(e, periods, baseBalances, showAccountCodes, wholeNumbers);
                    if (inner != null) items.Add(inner);
                }
                return new Row
                {
                    Name = group.Name,
                    Rows = new Rows
                    {
                        Items = items,
                        IsLess = group.IsExpenseGroup,
                    },
                };
            }
            throw new InvalidOperationException($"Unknown chart item type: {item.GetType().Name}");
        }
    }
}
