using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.ReceiptsAndPaymentsSummary;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.ReceiptsAndPaymentsSummary
{
    [ProtoContract]
    internal sealed class GetReceiptsAndPaymentsSummaryView : GetReportView<Model.ReceiptsAndPaymentsSummary>
    {
        protected override string DefaultTitle => Strings.ReceiptsAndPaymentsSummary;

        protected override ReportModel Build(Database business, Model.ReceiptsAndPaymentsSummary report)
        {
            var model = new ReportModel();
            if (!string.IsNullOrWhiteSpace(report.Title)) model.Title = report.Title;

            if (report.Periods == null) return model;

            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.Periods[0].FromDate.ToLocalShortDisplayString(), report.Periods[0].ToDate.ToLocalShortDisplayString());

            for (int i = 0; i < report.Periods.Length; i++)
            {
                var columnName = report.Periods[i].ToDate.ToLocalShortDisplayString();
                if (!string.IsNullOrWhiteSpace(report.Periods[i].ColumnName)) columnName = report.Periods[i].ColumnName;
                model.Columns.Add(new Column { Name = columnName, IsBold = (i == 0) });
            }

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.CurrencyParentheses, model.WholeNumbers, link);

            var accounts = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.Transaction is Receipt || x.Transaction is Payment || x.Transaction is InterAccountTransfer || x.Transaction is ExpenseClaim).GroupBy(x => x.GeneralLedgerAccount);

            for (int i = 0; i < 2; i++)
            {
                var category = (i == 0 ? Strings.Receipts : Strings.Payments);
                var groupRows = new Rows { IsLess = (i == 1) };
                foreach (var e in accounts.OrderBy(x => (report.AccountCodes ? x.Key.GetCodeAndName() : x.Key.GetName())))
                {
                    if (e.Key.IsCashAtBank) continue;

                    var inactive = false;
                    if (e.Key is ProfitAndLossStatementAccount profitAndLossStatementAccount) inactive = profitAndLossStatementAccount.Inactive;
                    if (e.Key is BalanceSheetAccount balanceSheetAccount) inactive = balanceSheetAccount.Inactive;

                    var cells = new List<Cell>();
                    for (int i2 = 0; i2 < report.Periods.Length; i2++)
                    {
                        var transactions = e.Where(x => x.Date >= report.Periods[i2].FromDate && x.Date <= report.Periods[i2].ToDate).ToArray();
                        var amount = transactions.Sum(x => x.BaseAmount);
                        if (i == 0 && amount > 0m) amount = 0m;
                        if (i == 1 && amount < 0m) amount = 0m;
                        amount *= -1m;
                        cells.Add(Make(amount, new Link(new ReceiptsAndPaymentsSummaryTransactions { Business = Business, Referrer = Referrer, Account = e.Key.Key, From = report.Periods[i2].FromDate, To = report.Periods[i2].ToDate, ReverseSign = (i == 0) }.ToUrl())));
                    }

                    if (inactive && cells.All(c => (c.Value ?? 0m) == 0m)) continue;

                    groupRows.Items.Add(new Row
                    {
                        Name = (report.AccountCodes ? e.Key.GetCodeAndName() : e.Key.GetName()),
                        Cells = cells,
                    });
                }
                model.Rows.Items.Add(new Row { Name = category, Rows = groupRows });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true, Name = Strings.NetIncreaseOrDecreaseInCashHeld });

            var cashAtTheBeginningOfThePeriodRow = new Row { Name = Strings.CashAtTheBeginningOfThePeriod, Cells = new List<Cell>() };
            var foreignExchangeRevaluation = new Row { Name = Strings.ForeignExchangeGain, Cells = new List<Cell>() };
            var journalEntries = new Row { Name = Strings.Adjustments, Cells = new List<Cell>() };

            for (int i = 0; i < report.Periods.Length; i++)
            {
                var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                    .Revaluate(report.Periods[i].FromDate, report.Periods[i].ToDate)
                    .GroupBy(x => x.GeneralLedgerAccount)
                    .Where(x => x.Key.IsCashAtBank)
                    .SelectMany(x => x)
                    .ToArray();

                var amount = generalLedger.Where(x => x.Date < report.Periods[i].FromDate).Sum(x => x.BaseAmount);
                var amount2 = generalLedger.Where(x => x.Date == report.Periods[i].ToDate && x.Transaction == null).Sum(x => x.BaseAmount);
                var amount3 = generalLedger.Where(x => x.Date >= report.Periods[i].FromDate && x.Date <= report.Periods[i].ToDate && x.Transaction is JournalEntry).Sum(x => x.BaseAmount);

                cashAtTheBeginningOfThePeriodRow.Cells.Add(Make(amount));
                foreignExchangeRevaluation.Cells.Add(Make(amount2));
                journalEntries.Cells.Add(Make(amount3, new Link(new ReceiptsAndPaymentsSummaryAdjustmentTransactions { Business = Business, Referrer = Referrer, From = report.Periods[i].FromDate, To = report.Periods[i].ToDate }.ToUrl())));
            }
            model.Rows.Items.Add(cashAtTheBeginningOfThePeriodRow);

            if (foreignExchangeRevaluation.Cells.Any(x => x.Value.HasValue && x.Value.Value != 0m)) model.Rows.Items.Add(foreignExchangeRevaluation);
            if (journalEntries.Cells.Any(x => x.Value.HasValue && x.Value.Value != 0m)) model.Rows.Items.Add(journalEntries);

            model.Rows.Items.Add(new Row { IsTotalRow = true, Name = Strings.CashAtTheEndOfThePeriod });

            model.Footer = report.Footer;
            model.Prune(report.ExcludeZeroBalances);

            return model;
        }
    }
}
