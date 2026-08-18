using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.CustomerStatementsUnpaidInvoices
{
    [ProtoContract]
    internal sealed class GetCustomerStatementUnpaidInvoicesView : GetTransactionView<Model.Customer>
    {
        protected override TransactionView GetViewData(Model.Customer o)
        {
            var settings = Database.Single<Model.CustomerStatementsUnpaidInvoices>();

            var viewData = new TransactionView();
            viewData.title = Strings.Statement;

            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = settings.GetDate().ToLocalShortDisplayString() });

            viewData.recipient.code = o.Code;
            viewData.recipient.name = o.Name;
            viewData.recipient.address = o.BillingAddress;
            viewData.recipient.email = o.Email;

            var currency = o.Currency;

            var salesInvoices = Database.OfType<Model.SalesInvoice>().Where(x => x.Customer == o.Key && x.IssueDate <= settings.GetDate()).ToList();
            var salesInvoiceTransactions = new Query.GeneralLedger.GeneralLedger(Business).AutomaticallyMatchSalesInvoices(new[] { o.Key }).Where(x => x.Date <= settings.GetDate()).Where(x => x.GeneralLedgerAccount.IsAccountsReceivable && x.Customer?.Key == o.Key && x.SalesInvoice != null).ToArray();
            var salesInvoiceTotals = salesInvoiceTransactions.Where(x => x.Transaction is Model.SalesInvoice && x.IsBalancing).GroupBy(x => x.Transaction).ToDictionary(x => x.Key, x => x.Sum(y => y.AccountAmount));
            var salesInvoiceBalances = salesInvoiceTransactions.GroupBy(x => x.SalesInvoice).ToDictionary(x => x.Key, x => x.Sum(y => y.AccountAmount));

            var days30 = settings.GetDate().AddDays(-30);
            var days60 = settings.GetDate().AddDays(-60);
            var days90 = settings.GetDate().AddDays(-90);

            foreach (var e in salesInvoices.ToArray())
            {
                if (!salesInvoiceBalances.ContainsKey(e)) { salesInvoices.Remove(e); continue; }
                if (salesInvoiceBalances[e] == 0m) { salesInvoices.Remove(e); continue; }
            }

            var showPurchaseOrderNumbers = salesInvoices.Any(x => x.SalesOrder.HasValue || !string.IsNullOrWhiteSpace(x.OrderNumber));

            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Date, align = "center", nowrap = true });
            if (showPurchaseOrderNumbers) viewData.table.columns.Add(new TransactionView.Column { label = Strings.OrderNumber, align = "center" });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Invoice, align = "center", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Description });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.InvoiceTotal, align = "right", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Overdue, align = "center", nowrap = true });
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.BalanceDue, align = "right", nowrap = true });

            var currencies = Query.Currencies.GetCurrencyProvider(Business);

            var total = 0m;
            foreach (var e in salesInvoices.OrderBy(x => x.IssueDate))
            {
                var row = new TransactionView.Row();
                row.cells.Add(new TransactionView.Cell { text = e.IssueDate.ToLocalShortDisplayString() });
                if (showPurchaseOrderNumbers) row.cells.Add(new TransactionView.Cell { text = Database.SingleOrDefault<Model.SalesOrder>(e.SalesOrder)?.GetName() ?? e.OrderNumber });
                row.cells.Add(new TransactionView.Cell { text = e.Reference });
                row.cells.Add(new TransactionView.Cell { text = e.Description.IfEmptyReplaceWith(Strings.Invoice + " " + e.Reference) });
                row.cells.Add(new TransactionView.Cell { text = salesInvoiceTotals[e].ToCurrencyString(currencies.Get(currency), currencySymbol: CurrencySymbol.None) });

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

                row.cells.Add(new TransactionView.Cell { text = salesInvoiceBalances[e].ToCurrencyString(currencies.Get(currency), CurrencySymbol.Short) });
                total += salesInvoiceBalances[e];
            }

            var _comingDue = salesInvoiceBalances.Where(x => x.Key.GetDueDate() >= settings.GetDate()).Sum(x => x.Value);
            var _days30 = salesInvoiceBalances.Where(x => x.Key.GetDueDate() < settings.GetDate() && x.Key.GetDueDate() >= days30).Sum(x => x.Value);
            var _days60 = salesInvoiceBalances.Where(x => x.Key.GetDueDate() < days30 && x.Key.GetDueDate() >= days60).Sum(x => x.Value);
            var _days90 = salesInvoiceBalances.Where(x => x.Key.GetDueDate() < days60 && x.Key.GetDueDate() >= days90).Sum(x => x.Value);
            var _days90Plus = salesInvoiceBalances.Where(x => x.Key.GetDueDate() < days90).Sum(x => x.Value);

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
