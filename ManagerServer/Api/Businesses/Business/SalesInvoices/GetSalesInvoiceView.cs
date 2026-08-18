using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.SalesInvoices
{
    [ProtoContract]
    internal sealed class GetSalesInvoiceView : GetTransactionView<Model.SalesInvoice>
    {
        protected override TransactionView GetViewData(Model.SalesInvoice o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.Invoice;
            viewData.reference = o.Reference;
            viewData.description = o.Description;

            var customInvoiceTitleTaxCodes = Database.OfType<Model.TaxCode>().Where(x => x.CustomSalesInvoiceTitle && !string.IsNullOrWhiteSpace(x.SalesInvoiceTitle)).ToArray();
            var customInvoiceTitleTaxCodeKeys = new HashSet<Guid>(customInvoiceTitleTaxCodes.Select(x => x.Key));
            if (o.Lines != null)
            {
                foreach (var e in o.Lines.Where(x => x.TaxCode.HasValue && customInvoiceTitleTaxCodeKeys.Contains(x.TaxCode.Value)))
                {
                    viewData.title = customInvoiceTitleTaxCodes.First(x => x.Key == e.TaxCode.Value).SalesInvoiceTitle;
                    break;
                }
            }
            if (o.HasSalesInvoiceCustomTitle && !string.IsNullOrWhiteSpace(o.SalesInvoiceCustomTitle)) viewData.title = o.SalesInvoiceCustomTitle;

            viewData.fields.Add(new TransactionView.Field { key = nameof(Strings.InvoiceDate), label = GetBilingualString(o.Bilingual, nameof(Strings.InvoiceDate), "Invoice date"), text = o.IssueDate.ToLocalShortDisplayString() });
            var dueDate = o.GetDueDate();
            if (!o.HideDueDate)
            {
                viewData.fields.Add(new TransactionView.Field { key = nameof(Strings.DueDate), label = GetBilingualString(o.Bilingual, nameof(Strings.DueDate), "Due date"), text = dueDate.ToLocalShortDisplayString() });
            }

            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { key = nameof(Strings.InvoiceNumber), label = GetBilingualString(o.Bilingual, nameof(Strings.InvoiceNumber), "Invoice number"), text = o.Reference });

            var salesQuote = Database.SingleOrDefault<Model.SalesQuote>(o.SalesQuote);
            if (salesQuote != null) viewData.fields.Add(new TransactionView.Field { key = nameof(Strings.QuoteNumber), label = GetBilingualString(o.Bilingual, nameof(Strings.QuoteNumber), "Quote number"), text = salesQuote.GetName() });
            else if (!string.IsNullOrWhiteSpace(o.QuoteNumber)) viewData.fields.Add(new TransactionView.Field { label = GetBilingualString(o.Bilingual, nameof(Strings.QuoteNumber), "Quote number"), text = o.QuoteNumber });

            var salesOrder = Database.SingleOrDefault<Model.SalesOrder>(o.SalesOrder);
            if (salesOrder != null) viewData.fields.Add(new TransactionView.Field { key = nameof(Strings.OrderNumber), label = GetBilingualString(o.Bilingual, nameof(Strings.OrderNumber), "Order number"), text = salesOrder.GetName() });
            else if (!string.IsNullOrWhiteSpace(o.OrderNumber)) viewData.fields.Add(new TransactionView.Field { label = GetBilingualString(o.Bilingual, nameof(Strings.OrderNumber), "Order number"), text = o.OrderNumber });

            var currencies = Query.Currencies.GetCurrencyProvider(Business);
            Guid? currency = null;

            if (o.Customer.HasValue)
            {
                var customer = Database.SingleOrDefault<Model.Customer>(o.Customer.Value);
                if (customer != null)
                {
                    viewData.recipient.code = customer.Code;
                    viewData.recipient.name = customer.Name;
                    viewData.recipient.address = o.BillingAddress;
                    if (string.IsNullOrWhiteSpace(viewData.recipient.address)) viewData.recipient.address = customer.BillingAddress;
                    viewData.recipient.email = customer.Email;

                    currency = customer.Currency;

                    viewData.custom_fields.AddRange(GetCustomFields(typeof(Model.Customer), customer.CustomFields));
                    viewData.custom_fields.AddRange(GetCustomFields2(typeof(Model.Customer), customer.CustomFields2));
                }
            }

            var salesInvoiceTransactions = new Query.GeneralLedger.GeneralLedger(Business).AutomaticallyMatchSalesInvoices(o.Customer.HasValue ? new[] { o.Customer.Value } : null).Where(x => x.GeneralLedgerAccount.IsAccountsReceivable && x.Customer?.Key == o.Customer && x.SalesInvoice != null && x.SalesInvoice.Key == Key).ToArray();
            var salesInvoiceAmountDue = salesInvoiceTransactions.Sum(x => x.AccountAmount);

            viewData.table = BuildTable(o, showTaxAmountOnLineItems: o.ShowTaxAmountColumn, bilingual: o.Bilingual, showLineNumbers: o.HasLineNumber, forceTotals: true, showItemImages: o.ShowItemImages);

            if (!o.HideBalanceDue)
            {
                foreach (var e in salesInvoiceTransactions.OrderBy(x => x.Date))
                {
                    if (e.SalesInvoiceAsTransaction != null) continue;
                    var transactionAmount = string.Empty;
                    if (e.TransactionCurrency != e.AccountCurrency) transactionAmount = (e.TransactionAmount < 0m ? e.TransactionAmount * -1m : e.TransactionAmount).ToCurrencyString(e.TransactionCurrency, CurrencySymbol.Short);
                    var label = string.Join(" — ", new[] { e.Transaction.GetTransactionName(), (e.OriginalDate ?? e.Date).ToLocalShortDisplayString(), transactionAmount }.Where(x => !string.IsNullOrWhiteSpace(x)));
                    viewData.table.totals.Add(new TransactionView.Total { label = label, text = e.AccountAmount.ToCurrencyString(e.AccountCurrency, CurrencySymbol.Short) });
                }
                if (salesInvoiceTransactions.Any(x => x.SalesInvoiceAsTransaction == null))
                {
                    viewData.table.totals.Add(new TransactionView.Total { label = Strings.BalanceDue, emphasis = true, text = salesInvoiceAmountDue.ToCurrencyString(salesInvoiceTransactions.First().AccountCurrency, CurrencySymbol.Short) });
                }

                if (!salesInvoiceTransactions.Any(x => x.CreditNote?.Type == Model.Enums.CreditNoteType.EarlyPaymentDiscount))
                {
                    if (o.EarlyPaymentDiscount && salesInvoiceTransactions.Any())
                    {
                        var earlyPaymentDiscountLabel = Strings.EarlyPaymentDiscount;
                        var earlyPaymentDiscount = 0m;
                        if (o.EarlyPaymentDiscountType == Model.Enums.DiscountType.ExactAmount)
                        {
                            earlyPaymentDiscount = o.EarlyPaymentDiscountAmount * -1m;
                        }
                        if (o.EarlyPaymentDiscountType == Model.Enums.DiscountType.Percentage && o.EarlyPaymentDiscountRate != 0m)
                        {
                            earlyPaymentDiscountLabel += " (" + o.EarlyPaymentDiscountRate.ToString() + "%)";
                            var currency2 = salesInvoiceTransactions.First().AccountCurrency;
                            var invoiceTotal = salesInvoiceTransactions.Where(x => x.SalesInvoiceAsTransaction != null).Single(x => x.IsBalancing).TransactionAmount;
                            earlyPaymentDiscount = currency2.Round(invoiceTotal / 100 * o.EarlyPaymentDiscountRate * -1m);
                        }

                        if (earlyPaymentDiscount != 0m)
                        {
                            viewData.table.totals.Add(new TransactionView.Total { label = earlyPaymentDiscountLabel, text = earlyPaymentDiscount.ToCurrencyString(salesInvoiceTransactions.First().AccountCurrency, CurrencySymbol.Short) });
                            viewData.table.totals.Add(new TransactionView.Total { label = string.Format(Strings.Balance_due_if_paid_by, o.IssueDate.AddDays(o.EarlyPaymentDiscountDays ?? 0).ToShortDateString()), text = (salesInvoiceAmountDue + earlyPaymentDiscount).ToCurrencyString(salesInvoiceTransactions.First().AccountCurrency, CurrencySymbol.Short), emphasis = true });
                        }
                    }
                }
            }

            if (!o.HideBalanceDue)
            {
                if (salesInvoiceAmountDue == 0m)
                {
                    viewData.emphasis.positive = true;
                    viewData.emphasis.negative = false;
                    viewData.emphasis.text = Strings.PaidInFull;
                }
                else if (salesInvoiceAmountDue > 0m && dueDate < DateTime.Today)
                {
                    if (!o.HideDueDate)
                    {
                        viewData.emphasis.positive = false;
                        viewData.emphasis.negative = true;
                        viewData.emphasis.text = Strings.Overdue;
                    }
                }
            }
            
            if (o.TotalAmountInBaseCurrency && currency.HasValue && o.ExchangeRate > 0m)
            {
                var totalField = viewData.table.totals.LastOrDefault(x => x.label == Strings.Total);
                if (totalField != null)
                {
                    var baseCurrency = Database.Single<Model.BaseCurrency>();
                    if (!string.IsNullOrWhiteSpace(baseCurrency.GetCode()))
                    {
                        var totalInBaseCurrency = 0m;
                        if (!o.ExchangeRateIsInverse) totalInBaseCurrency = Math.Round(totalField.number * o.ExchangeRate, baseCurrency.GetDecimalPlaces(), MidpointRounding.AwayFromZero);
                        if (o.ExchangeRateIsInverse) totalInBaseCurrency = Math.Round(totalField.number / o.ExchangeRate, baseCurrency.GetDecimalPlaces(), MidpointRounding.AwayFromZero);
                        viewData.custom_fields.Add(new TransactionView.CustomField { label = "Total amount in " + baseCurrency.GetCode(), text = totalInBaseCurrency.ToCurrencyString(baseCurrency, CurrencySymbol.Long) });
                    }
                }
            }

            return viewData;
        }
    }
}
