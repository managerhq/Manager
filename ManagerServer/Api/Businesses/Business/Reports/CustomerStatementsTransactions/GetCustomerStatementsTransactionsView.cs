using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.CustomerStatementsTransactions
{
    [ProtoContract]
    internal sealed class GetCustomerStatementsTransactionsView : GetTransactionView<Model.Customer>
    {
        protected override TransactionView GetViewData(Model.Customer o)
        {
            var settings = Database.Single<Model.CustomerStatementsTransactions>();

            var viewData = new TransactionView();
            viewData.title = Strings.Statement;

            viewData.fields.Add(new TransactionView.Field { label = Strings.FromDate, text = settings.FromDate.ToLocalShortDisplayString() });
            viewData.fields.Add(new TransactionView.Field { label = Strings.ToDate, text = settings.GetToDate().ToLocalShortDisplayString() });

            viewData.recipient.code = o.Code;
            viewData.recipient.name = o.Name;
            viewData.recipient.address = o.BillingAddress;
            viewData.recipient.email = o.Email;

            var currencies = Query.Currencies.GetCurrencyProvider(Business);
            var currency = o.Currency;

            var customerTransactions = new Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.Date <= settings.GetToDate() && x.GeneralLedgerAccount.IsAccountsReceivable && x.Customer?.Key == o.Key)
                .OrderBy(x => x.Date)
                .ThenByDescending(x => x.AccountAmount > 0m)
                .ThenBy(x => x.Transaction?.GetReference())
                .ToArray();

            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Date, align = "center", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Description });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Debit, align = "right", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Credit, align = "right", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Balance, align = "right", nowrap = true });

            var openingBalance = 0m;
            var balance = customerTransactions.Where(x => x.Date < settings.FromDate).Sum(x => x.AccountAmount);
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

            foreach (var e in customerTransactions.Where(x => x.Date >= settings.FromDate))
            {
                balance += e.AccountAmount;
                var row = new TransactionView.Row();
                row.cells.Add(new TransactionView.Cell { text = e.Date.ToLocalShortDisplayString() });
                string description = null;
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
                    if (string.IsNullOrWhiteSpace(description)) description = Strings.Refund;
                    if (!string.IsNullOrWhiteSpace(e.Payment.Reference)) description += " " + e.Payment.Reference;

                    if (!string.IsNullOrWhiteSpace(e.TransactionLine?.GetDescriptionOrNull(e.Transaction))) description += " — " + e.TransactionLine.GetDescriptionOrNull(e.Transaction);
                    else if (!string.IsNullOrWhiteSpace(e.Transaction.GetDescriptionOrNull())) description += " — " + e.Transaction.GetDescriptionOrNull();
                }
                else if (e.Transaction is Model.SalesInvoice salesInvoice)
                {
                    description = Strings.Invoice;
                    if (!string.IsNullOrWhiteSpace(salesInvoice.Reference)) description += " " + salesInvoice.Reference;
                    if (!string.IsNullOrWhiteSpace(e.Transaction.GetDescriptionOrNull())) description += " — " + e.Transaction.GetDescriptionOrNull();
                }
                else
                {
                    description = e.Transaction.GetNameAndDescription();
                }

                if (!(e.Transaction is Model.SalesInvoice) && e.SalesInvoice != null)
                {
                    description = (Strings.Invoice + " " + e.SalesInvoice.Reference).Trim() + " — " + description;
                }

                row.cells.Add(new TransactionView.Cell { text = description });
                row.cells.Add(new TransactionView.Cell { text = (e.AccountAmount >= 0m ? e.AccountAmount.ToCurrencyString(currencies.Get(currency), currencySymbol: CurrencySymbol.None) : null) });
                row.cells.Add(new TransactionView.Cell { text = (e.AccountAmount < 0m ? (e.AccountAmount * -1m).ToCurrencyString(currencies.Get(currency), currencySymbol: CurrencySymbol.None) : null) });
                row.cells.Add(new TransactionView.Cell { text = balance.ToCurrencyStringAsDrCr(currencies.Get(currency), currencySymbol: CurrencySymbol.None) });
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
