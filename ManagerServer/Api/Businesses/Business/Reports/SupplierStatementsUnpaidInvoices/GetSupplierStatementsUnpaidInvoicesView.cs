using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.SupplierStatementsUnpaidInvoices
{
    [ProtoContract]
    internal sealed class GetSupplierStatementsUnpaidInvoicesView : GetTransactionView<Model.Supplier>
    {
        protected override TransactionView GetViewData(Model.Supplier o)
        {
            var settings = Database.Single<Model.SupplierStatementsUnpaidInvoices>();

            var viewData = new TransactionView();
            viewData.title = Strings.Statement;
            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = settings.GetDate().ToLocalShortDisplayString() });

            viewData.recipient.code = o.Code;
            viewData.recipient.name = o.Name;
            viewData.recipient.address = o.Address;
            viewData.recipient.email = o.Email;

            var currency = o.Currency;

            var purchaseInvoices = Database.OfType<Model.PurchaseInvoice>().Where(x => x.Supplier == o.Key && x.IssueDate <= settings.GetDate()).ToList();
            var purchaseInvoiceTransactions = new Query.GeneralLedger.GeneralLedger(Business).AutomaticallyMatchPurchaseInvoices(new[] { o.Key }).Where(x => x.Date <= settings.GetDate()).Where(x => x.GeneralLedgerAccount.IsAccountsPayable && x.Supplier?.Key == o.Key && x.PurchaseInvoice != null).ToArray();
            var purchaseInvoiceTotals = purchaseInvoiceTransactions.Where(x => x.Transaction is Model.PurchaseInvoice && x.IsBalancing).ToDictionary(x => x.PurchaseInvoice, x => x.AccountAmount * -1m);
            var purchaseInvoiceBalances = purchaseInvoiceTransactions.GroupBy(x => x.PurchaseInvoice).ToDictionary(x => x.Key, x => x.Sum(y => y.AccountAmount) * -1m);

            var date = settings.GetDate();
            var days30 = date.AddDays(-30);
            var days60 = date.AddDays(-60);
            var days90 = date.AddDays(-90);

            foreach (var e in purchaseInvoices.ToArray())
            {
                if (!purchaseInvoiceBalances.ContainsKey(e)) { purchaseInvoices.Remove(e); continue; }
                if (purchaseInvoiceBalances[e] == 0m) { purchaseInvoices.Remove(e); continue; }
            }

            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Date, align = "center", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Invoice, align = "center", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Description });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.InvoiceTotal, align = "right", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Overdue, align = "center", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.BalanceDue, align = "right", nowrap = true });

            var currencies = Query.Currencies.GetCurrencyProvider(Business);

            var total = 0m;
            foreach (var e in purchaseInvoices.OrderBy(x => x.IssueDate))
            {
                var row = new TransactionView.Row();
                row.cells.Add(new TransactionView.Cell { text = e.IssueDate.ToLocalShortDisplayString() });
                row.cells.Add(new TransactionView.Cell { text = e.Reference });
                row.cells.Add(new TransactionView.Cell { text = e.Description.IfEmptyReplaceWith(Strings.Invoice + " " + e.Reference) });
                row.cells.Add(new TransactionView.Cell { text = purchaseInvoiceTotals[e].ToCurrencyString(currencies.Get(currency), currencySymbol: CurrencySymbol.None) });

                string overdue;
                var days = (settings.GetDate() - e.GetDueDate()).TotalDays;
                if (days > 0)
                {
                    if (days == 1) overdue = Strings._1_day;
                    else overdue = string.Format(Strings.XXX_days, days.ToString());
                }
                else
                {
                    overdue = "-";
                }
                row.cells.Add(new TransactionView.Cell { text = overdue });
                viewData.table.rows.Add(row);

                row.cells.Add(new TransactionView.Cell { text = purchaseInvoiceBalances[e].ToCurrencyString(currencies.Get(currency), CurrencySymbol.Short) });
                total += purchaseInvoiceBalances[e];
            }

            var _comingDue = purchaseInvoiceBalances.Where(y => y.Key.GetDueDate() >= date).Sum(y => y.Value);
            var _days30 = purchaseInvoiceBalances.Where(y => y.Key.GetDueDate() < date && y.Key.GetDueDate() >= days30).Sum(y => y.Value);
            var _days60 = purchaseInvoiceBalances.Where(y => y.Key.GetDueDate() < days30 && y.Key.GetDueDate() >= days60).Sum(y => y.Value);
            var _days90 = purchaseInvoiceBalances.Where(y => y.Key.GetDueDate() < days60 && y.Key.GetDueDate() >= days90).Sum(y => y.Value);
            var _days90Plus = purchaseInvoiceBalances.Where(y => y.Key.GetDueDate() < days90).Sum(y => y.Value);

            viewData.table.totals.Add(new TransactionView.Total { label = Strings.Current, number = _comingDue, text = _comingDue.ToCurrencyString(currencies.Get(currency), CurrencySymbol.Short) });
            viewData.table.totals.Add(new TransactionView.Total { label = Strings._1_30_days_overdue, number = _days30, text = _days30.ToCurrencyString(currencies.Get(currency), CurrencySymbol.Short) });
            viewData.table.totals.Add(new TransactionView.Total { label = Strings._31_60_days_overdue, number = _days60, text = _days60.ToCurrencyString(currencies.Get(currency), CurrencySymbol.Short) });
            viewData.table.totals.Add(new TransactionView.Total { label = Strings._61_90_days_overdue, number = _days90, text = _days90.ToCurrencyString(currencies.Get(currency), CurrencySymbol.Short) });
            viewData.table.totals.Add(new TransactionView.Total { label = Strings._90plus_days_overdue, number = _days90Plus, text = _days90Plus.ToCurrencyString(currencies.Get(currency), CurrencySymbol.Short) });
            viewData.table.totals.Add(new TransactionView.Total { label = Strings.Total, emphasis = true, number = total, text = total.ToCurrencyString(currencies.Get(currency), CurrencySymbol.Short) });
            return viewData;
        }
    }
}
