using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System;
using System.Collections.Generic;

namespace ManagerServer.Api.Businesses.Business.InterAccountTransfers
{
    [ProtoContract]
    internal sealed class GetInterAccountTransferView : GetTransactionView<Model.InterAccountTransfer>
    {
        private sealed class Account
        {
            public string Name;
            public Guid? Currency;
        }

        protected override TransactionView GetViewData(Model.InterAccountTransfer o)
        {
            var currencies = Query.Currencies.GetCurrencyProvider(Business);

            var viewData = new TransactionView();
            viewData.title = Strings.InterAccountTransfer;
            viewData.reference = o.Reference;
            viewData.description = o.Description;

            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = o.Date.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            var bankCashAccounts = new Dictionary<Guid, Account>();
            foreach (var e in Database.OfType<Model.BankOrCashAccount>()) bankCashAccounts.Add(e.Key, new Account { Name = e.Name, Currency = e.Currency });

            Account creditAccount = null;
            if (o.PaidFrom.HasValue && bankCashAccounts.ContainsKey(o.PaidFrom.Value)) creditAccount = bankCashAccounts[o.PaidFrom.Value];
            var creditAccountCurrency = creditAccount?.Currency;
            Account debitAccount = null;
            if (o.ReceivedIn.HasValue && bankCashAccounts.ContainsKey(o.ReceivedIn.Value)) debitAccount = bankCashAccounts[o.ReceivedIn.Value];
            var debitAccountCurrency = debitAccount?.Currency;

            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Account });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Outflows, align = "center", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Inflows, align = "center", nowrap = true });

            var row1 = new TransactionView.Row();
            row1.cells.Add(new TransactionView.Cell { text = creditAccount?.Name });
            row1.cells.Add(new TransactionView.Cell { value = o.PaidFrom, text = o.CreditAmount.ToCurrencyString(currencies.Get(creditAccountCurrency), CurrencySymbol.Long) });
            row1.cells.Add(new TransactionView.Cell());
            viewData.table.rows.Add(row1);

            var row2 = new TransactionView.Row();
            row2.cells.Add(new TransactionView.Cell { text = debitAccount?.Name });
            row2.cells.Add(new TransactionView.Cell());
            if (creditAccountCurrency == debitAccountCurrency)
            {
                row2.cells.Add(new TransactionView.Cell { value = o.CreditAmount, text = o.CreditAmount.ToCurrencyString(currencies.Get(creditAccountCurrency), CurrencySymbol.Long) });
            }
            else
            {
                row2.cells.Add(new TransactionView.Cell { value = o.DebitAmount, text = o.DebitAmount.ToCurrencyString(currencies.Get(debitAccountCurrency), CurrencySymbol.Long) });
            }
            viewData.table.rows.Add(row2);

            if (creditAccountCurrency != debitAccountCurrency)
            {
                var text = (1m).ToCurrencyString(currencies.Get(creditAccountCurrency), CurrencySymbol.Long);
                text += " = ";
                if (o.DebitAmount != 0m && o.CreditAmount != 0m)
                {
                    text += decimal.Round(o.DebitAmount / o.CreditAmount, 4).ToCurrencyString(currencies.Get(debitAccountCurrency), CurrencySymbol.Long);
                }
                else
                {
                    text += 0m.ToCurrencyString(currencies.Get(debitAccountCurrency), CurrencySymbol.Long);
                }
                text += "\n";
                text += o.CreditAmount.ToCurrencyString(currencies.Get(creditAccountCurrency), CurrencySymbol.Long);
                text += " = ";
                text += o.DebitAmount.ToCurrencyString(currencies.Get(debitAccountCurrency), CurrencySymbol.Long);
                viewData.custom_fields.Add(new TransactionView.CustomField { label = Strings.ExchangeRate, text = text });
            }

            return viewData;
        }
    }
}
