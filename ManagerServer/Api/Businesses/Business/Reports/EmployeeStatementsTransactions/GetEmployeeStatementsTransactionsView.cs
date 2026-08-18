using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.EmployeeStatementsTransactions
{
    [ProtoContract]
    internal sealed class GetEmployeeStatementsTransactionsView : GetTransactionView<Model.Employee>
    {
        protected override TransactionView GetViewData(Model.Employee o)
        {
            var settings = Database.Single<Model.EmployeeStatementsTransactions>();

            var viewData = new TransactionView();
            viewData.title = Strings.Statement;

            viewData.fields.Add(new TransactionView.Field { label = Strings.FromDate, text = settings.FromDate.ToLocalShortDisplayString() });
            viewData.fields.Add(new TransactionView.Field { label = Strings.ToDate, text = settings.GetToDate().ToLocalShortDisplayString() });

            viewData.recipient.code = o.Code;
            viewData.recipient.name = o.Name;
            viewData.recipient.address = o.Address;
            viewData.recipient.email = o.Email;

            var currency = o.Currency;

            var employeeTransactions = new Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.Date <= settings.GetToDate() && x.GeneralLedgerAccount.IsEmployeeClearingAccount && x.Employee?.Key == o.Key)
                .OrderBy(x => x.Date)
                .ThenByDescending(x => x.AccountAmount < 0m)
                .ThenBy(x => x.Transaction?.GetReference())
                .ToArray();

            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Date, align = "center", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Description });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Debit, align = "right", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Credit, align = "right", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Balance, align = "right", nowrap = true });

            var currencies = Query.Currencies.GetCurrencyProvider(Business);

            var openingBalance = 0m;
            var balance = employeeTransactions.Where(x => x.Date < settings.FromDate).Sum(x => x.AccountAmount);
            if (balance != 0m)
            {
                openingBalance = balance;
                var row = new TransactionView.Row();
                row.cells.Add(new TransactionView.Cell { text = settings.FromDate.ToLocalShortDisplayString() });
                row.cells.Add(new TransactionView.Cell { text = Strings.OpeningBalance });
                row.cells.Add(new TransactionView.Cell());
                row.cells.Add(new TransactionView.Cell());
                row.cells.Add(new TransactionView.Cell { text = balance.ToCurrencyStringAsDrCr(currencies.Get(currency), currencySymbol: CurrencySymbol.None) });
                viewData.table.rows.Add(row);
            }

            var totalDebits = 0m;
            var totalCredits = 0m;

            foreach (var e in employeeTransactions.Where(x => x.Date >= settings.FromDate))
            {
                balance += e.AccountAmount;
                var row = new TransactionView.Row();
                row.cells.Add(new TransactionView.Cell { text = e.Date.ToLocalShortDisplayString() });

                string description;
                if (e.Transaction is Model.JournalEntry)
                {
                    description = e.TransactionLine?.GetLineDescription(e.Transaction);
                    if (string.IsNullOrWhiteSpace(description)) description = e.Transaction.GetDescriptionOrNull();
                    if (string.IsNullOrWhiteSpace(description)) description = e.Transaction.GetTransactionName();
                }
                else if (e.Transaction is Model.Receipt)
                {
                    description = e.Transaction.TransactionTitle;
                    if (string.IsNullOrWhiteSpace(description)) description = Strings.Receipt;
                    if (!string.IsNullOrWhiteSpace(e.Receipt.Reference)) description += " " + e.Receipt.Reference;

                    if (!string.IsNullOrWhiteSpace(e.TransactionLine?.GetDescriptionOrNull(e.Transaction))) description += " — " + e.TransactionLine.GetDescriptionOrNull(e.Transaction);
                    else if (!string.IsNullOrWhiteSpace(e.Transaction.GetDescriptionOrNull())) description += " — " + e.Transaction.GetDescriptionOrNull();
                }
                else if (e.Transaction is Model.Payment)
                {
                    description = e.Transaction.TransactionTitle;
                    if (string.IsNullOrWhiteSpace(description)) description = Strings.Payment;
                    if (!string.IsNullOrWhiteSpace(e.Payment.Reference)) description += " " + e.Payment.Reference;

                    if (!string.IsNullOrWhiteSpace(e.TransactionLine?.GetDescriptionOrNull(e.Transaction))) description += " — " + e.TransactionLine.GetDescriptionOrNull(e.Transaction);
                    else if (!string.IsNullOrWhiteSpace(e.Transaction.GetDescriptionOrNull())) description += " — " + e.Transaction.GetDescriptionOrNull();
                }
                else if (e.Transaction is Model.Payslip payslip)
                {
                    description = Strings.Payslip;
                    if (!string.IsNullOrWhiteSpace(payslip.Reference)) description += " " + payslip.Reference;
                    if (!string.IsNullOrWhiteSpace(e.Transaction.GetDescriptionOrNull())) description += " — " + e.Transaction.GetDescriptionOrNull();
                }
                else if (e.Transaction is Model.ExpenseClaim expenseClaim)
                {
                    description = Strings.ExpenseClaim;
                    if (!string.IsNullOrWhiteSpace(expenseClaim.Reference)) description += " " + expenseClaim.Reference;
                    if (!string.IsNullOrWhiteSpace(e.Transaction.GetDescriptionOrNull())) description += " — " + e.Transaction.GetDescriptionOrNull();
                }
                else
                {
                    description = e.Transaction.GetNameAndDescription();
                }

                row.cells.Add(new TransactionView.Cell { text = description });

                row.cells.Add(new TransactionView.Cell { text = (e.AccountAmount > 0m ? e.AccountAmount.ToCurrencyString(e.AccountCurrency, currencySymbol: CurrencySymbol.None) : null) });
                row.cells.Add(new TransactionView.Cell { text = (e.AccountAmount <= 0m ? (e.AccountAmount * -1m).ToCurrencyString(e.AccountCurrency, currencySymbol: CurrencySymbol.None) : null) });
                row.cells.Add(new TransactionView.Cell { text = balance.ToCurrencyStringAsDrCr(e.AccountCurrency, currencySymbol: CurrencySymbol.None) });
                viewData.table.rows.Add(row);

                if (e.AccountAmount > 0m) totalDebits += e.AccountAmount;
                if (e.AccountAmount < 0m) totalCredits += e.AccountAmount;
            }

            if (openingBalance != 0m) viewData.table.totals.Add(new TransactionView.Total { label = Strings.OpeningBalance, number = totalDebits, text = openingBalance.ToCurrencyStringAsDrCr(currencies.Get(currency), currencySymbol: CurrencySymbol.Short) });
            viewData.table.totals.Add(new TransactionView.Total { label = Strings.Total_debits, number = totalDebits, text = totalDebits.ToCurrencyStringAsDrCr(currencies.Get(currency), currencySymbol: CurrencySymbol.Short) });
            viewData.table.totals.Add(new TransactionView.Total { label = Strings.Total_credits, number = totalCredits, text = totalCredits.ToCurrencyStringAsDrCr(currencies.Get(currency), currencySymbol: CurrencySymbol.Short) });
            viewData.table.totals.Add(new TransactionView.Total { label = Strings.ClosingBalance, emphasis = true, number = balance, text = balance.ToCurrencyStringAsDrCr(currencies.Get(currency), currencySymbol: CurrencySymbol.Short) });
            return viewData;
        }
    }
}
