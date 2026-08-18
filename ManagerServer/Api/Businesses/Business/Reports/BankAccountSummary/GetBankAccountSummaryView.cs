using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.BankAccountSummary;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.BankAccountSummary
{
    [ProtoContract]
    internal sealed class GetBankAccountSummaryView : GetReportView<Model.BankAccountSummary>
    {
        protected override string DefaultTitle => Strings.BankAccountSummary;

        protected override ReportModel Build(Database business, Model.BankAccountSummary report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.Periods?[0].FromDate.ToLocalShortDisplayString(), report.Periods?[0].ToDate.ToLocalShortDisplayString());

            if (!report.BankAccount.HasValue) return model;

            var bankAccount = business.SingleOrDefault<BankOrCashAccount>(report.BankAccount.Value);
            if (bankAccount == null) return model;

            model.Subtitle2 = bankAccount.NameWithCode;

            for (int i = 0; i < report.Periods.Length; i++)
            {
                var columnName = report.Periods[i].ToDate.ToLocalShortDisplayString();
                model.Columns.Add(new Column { Name = columnName, IsBold = (i == 0) });
            }

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.CurrencyParentheses, model.WholeNumbers, link);

            var accounts = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => !x.GeneralLedgerAccount.IsCashAtBank && (x.Transaction is Receipt || x.Transaction is Payment) && x.BankAccount == bankAccount).GroupBy(x => x.GeneralLedgerAccount.Key);

            for (int i = 0; i < 2; i++)
            {
                var category = (i == 0 ? Strings.Inflows : Strings.Outflows);
                var groupRows = new Rows { IsLess = (i == 1) };
                foreach (var e in accounts.OrderBy(x => (report.AccountCodes ? x.First().GeneralLedgerAccount.GetCodeAndName() : x.First().GeneralLedgerAccount.GetName())))
                {
                    var inactive = false;
                    if (e.First().GeneralLedgerAccount is ProfitAndLossStatementAccount profitAndLossStatementAccount) inactive = profitAndLossStatementAccount.Inactive;
                    if (e.First().GeneralLedgerAccount is BalanceSheetAccount balanceSheetAccount) inactive = balanceSheetAccount.Inactive;

                    var cells = new System.Collections.Generic.List<Cell>();
                    for (int i2 = 0; i2 < report.Periods.Length; i2++)
                    {
                        var transactions = e.Where(x => x.Date >= report.Periods[i2].FromDate && x.Date <= report.Periods[i2].ToDate).ToArray();
                        var amount = 0m;
                        if (i == 0)
                        {
                            amount = transactions.Where(x => x.TransactionAmount < 0m).Sum(x => x.TransactionAmount) * -1m;
                        }
                        if (i == 1)
                        {
                            amount = transactions.Where(x => x.TransactionAmount > 0m).Sum(x => x.TransactionAmount) * -1m;
                        }
                        cells.Add(Make(amount, new Link(new BankAccountSummaryTransactions { Business = Business, Referrer = Referrer, GeneralLedgerAccount = e.Key, BankAccount = report.BankAccount.Value, From = report.Periods[i2].FromDate, To = report.Periods[i2].ToDate, Debits = (i == 0), Credits = (i == 1) }.ToUrl())));
                    }

                    if (inactive && cells.All(c => (c.Value ?? 0m) == 0m)) continue;

                    groupRows.Items.Add(new Row
                    {
                        Name = (report.AccountCodes ? e.First().GeneralLedgerAccount.GetCodeAndName() : e.First().GeneralLedgerAccount.GetName()),
                        Cells = cells,
                    });
                }

                model.Rows.Items.Add(new Row { Name = category, Rows = groupRows });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true, Name = Strings.NetIncreaseOrDecreaseInCashHeld });

            var cashAtTheBeginningOfThePeriodRow = new Row { Name = Strings.CashAtTheBeginningOfThePeriod, Cells = new System.Collections.Generic.List<Cell>() };
            for (int i = 0; i < report.Periods.Length; i++)
            {
                var openingBalance = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsCashAtBank && x.BankAccount.Key == report.BankAccount.Value && (x.Date < report.Periods[i].FromDate)).Sum(x => x.AccountAmount);
                cashAtTheBeginningOfThePeriodRow.Cells.Add(Make(openingBalance));
            }
            model.Rows.Items.Add(cashAtTheBeginningOfThePeriodRow);

            var interAccountTransfers = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsCashAtBank && x.Transaction is InterAccountTransfer && x.BankAccount == bankAccount).ToArray();
            if (interAccountTransfers.Any())
            {
                var row = new Row { Name = Strings.InterAccountTransfers, Cells = new System.Collections.Generic.List<Cell>() };
                for (int i2 = 0; i2 < report.Periods.Length; i2++)
                {
                    var amount = interAccountTransfers.Where(x => x.Date >= report.Periods[i2].FromDate && x.Date <= report.Periods[i2].ToDate).Sum(x => x.AccountAmount);
                    row.Cells.Add(Make(amount));
                }
                model.Rows.Items.Add(row);
            }

            var journalEntries = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsCashAtBank && x.Transaction is JournalEntry && x.BankAccount == bankAccount).ToArray();
            if (journalEntries.Any())
            {
                var row = new Row { Name = Strings.JournalEntries, Cells = new System.Collections.Generic.List<Cell>() };
                for (int i2 = 0; i2 < report.Periods.Length; i2++)
                {
                    var amount = journalEntries.Where(x => x.Date >= report.Periods[i2].FromDate && x.Date <= report.Periods[i2].ToDate).Sum(x => x.AccountAmount);
                    row.Cells.Add(Make(amount));
                }
                model.Rows.Items.Add(row);
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true, Name = Strings.CashAtTheEndOfThePeriod });

            model.Prune(report.ExcludeZeroBalances);

            return model;
        }
    }
}
