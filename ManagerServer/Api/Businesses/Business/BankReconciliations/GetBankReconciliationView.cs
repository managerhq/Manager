using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.BankReconciliations
{
    [ProtoContract]
    internal sealed class GetBankReconciliationView : GetTransactionView<Model.BankReconciliation>
    {
        protected override TransactionView GetViewData(Model.BankReconciliation o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.BankReconciliation;
            viewData.fields.Add(new TransactionView.Field { text = o.Date.ToLocalShortDisplayString(), label = Strings.Date });

            var bankTransactions = new Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsCashAtBank && x.BankAccount?.Key == o.BankAccount && x.Date <= o.Date).ToArray();
            var currency = bankTransactions.FirstOrDefault()?.AccountCurrency;

            if (o.BankAccount.HasValue)
            {
                var bankAccount = Database.SingleOrDefault<Model.BankOrCashAccount>(o.BankAccount.Value);
                if (bankAccount != null)
                {
                    viewData.description = bankAccount.NameWithCode;
                }
            }

            viewData.table = new TransactionView.Table();

            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Description });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Amount, align = "right", nowrap = true });

            var row = new TransactionView.Row();
            row.cells.Add(new TransactionView.Cell { text = Strings.ClosingBalanceAsPerBank });
            row.cells.Add(new TransactionView.Cell { text = o.StatementBalance.ToCurrencyString(currency, CurrencySymbol.None) });
            viewData.table.rows.Add(row);

            var adjustedClosingBalanceAsPerBankStatement = o.StatementBalance;

            var pendingDeposits = bankTransactions.Where(x => !x.ClearDate.HasValue || x.ClearDate > o.Date).Where(x => x.AccountAmount > 0m);
            var pendingWithdrawals = bankTransactions.Where(x => !x.ClearDate.HasValue || x.ClearDate > o.Date).Where(x => x.AccountAmount < 0m);

            foreach (var e in pendingDeposits)
            {
                var pendingDepositRow = new TransactionView.Row();
                pendingDepositRow.cells.Add(new TransactionView.Cell { text = string.Join(" — ", new[] { Strings.PendingDeposit, e.Date.ToLocalShortDisplayString(), e.Receipt?.Reference, e.Payment?.Reference, e.InterAccountTransfer?.Reference, e.Contact, e.Transaction.GetDescriptionOrNull() }.Where(x => !string.IsNullOrWhiteSpace(x))) });
                pendingDepositRow.cells.Add(new TransactionView.Cell { text = e.AccountAmount.ToCurrencyString(e.AccountCurrency, CurrencySymbol.None) });
                viewData.table.rows.Add(pendingDepositRow);
                adjustedClosingBalanceAsPerBankStatement += e.AccountAmount;
            }

            foreach (var e in pendingWithdrawals)
            {
                var pendingWithdrawalRow = new TransactionView.Row();
                pendingWithdrawalRow.cells.Add(new TransactionView.Cell { text = string.Join(" — ", new[] { Strings.PendingWithdrawal, e.Date.ToLocalShortDisplayString(), e.Receipt?.Reference, e.Payment?.Reference, e.InterAccountTransfer?.Reference, e.Contact, e.Transaction.GetDescriptionOrNull() }.Where(x => !string.IsNullOrWhiteSpace(x))) });
                pendingWithdrawalRow.cells.Add(new TransactionView.Cell { text = e.AccountAmount.ToCurrencyString(e.AccountCurrency, CurrencySymbol.None) });
                viewData.table.rows.Add(pendingWithdrawalRow);
                adjustedClosingBalanceAsPerBankStatement += e.AccountAmount;
            }

            if (pendingDeposits.Any() || pendingWithdrawals.Any()) viewData.table.totals.Add(new TransactionView.Total { label = Strings.AdjustedClosingBalanceAsPerBank, number = adjustedClosingBalanceAsPerBankStatement, text = adjustedClosingBalanceAsPerBankStatement.ToCurrencyString(currency, CurrencySymbol.Short), emphasis = true });

            var closingBalanceAsPerBalanceSheet = bankTransactions.Sum(x => x.AccountAmount);

            viewData.table.totals.Add(new TransactionView.Total { label = Strings.ClosingBalanceAsPerBalanceSheet, number = closingBalanceAsPerBalanceSheet, text = closingBalanceAsPerBalanceSheet.ToCurrencyString(currency, CurrencySymbol.Short), emphasis = true });

            var discrepancy = adjustedClosingBalanceAsPerBankStatement - closingBalanceAsPerBalanceSheet;

            viewData.table.totals.Add(new TransactionView.Total { label = Strings.Discrepancy, number = discrepancy, text = discrepancy.ToCurrencyString(currency, CurrencySymbol.Short) });

            if (discrepancy == 0m)
            {
                viewData.emphasis = new TransactionView.Emphasis { text = Strings.Reconciled, positive = true };
            }
            else
            {
                viewData.emphasis = new TransactionView.Emphasis { text = Strings.NotReconciled, negative = true };
            }

            return viewData;
        }
    }
}
