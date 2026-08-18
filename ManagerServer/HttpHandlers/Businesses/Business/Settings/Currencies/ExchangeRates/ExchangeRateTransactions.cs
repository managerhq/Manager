using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Currencies.ExchangeRates
{
    [ProtoContract]
    [Title(nameof(Strings.ExchangeRate), nameof(Strings.Transactions))]
    [Guide("This screen displays all transactions that are using the selected *exchange rate*.")]
    [Guide("When you modify an exchange rate, existing transactions continue to use their original rates unless you specifically update them.")]
    [Header("Updating Transactions")]
    [Guide("To apply the new exchange rate to multiple transactions at once, select the transactions you want to update and click the **Batch Update** button.")]
    [Guide("Only transactions that currently have a different exchange rate will be updated - transactions already using the current rate are automatically excluded.")]
    [Columns]
    internal sealed class ExchangeRateTransactions : Table<ExchangeRateTransactions.Row>
    {
        [ProtoMember(1)] public Guid ExchangeRate;

        protected override Row[] GetObjects()
        {
            var database = ApplicationData.Businesses.Get(Business);

            var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();
            var exchangeRate = database.SingleOrDefault<ExchangeRate>(ExchangeRate);

            if (exchangeRate == null) return new Row[0];

            var foreignCurrency = database.SingleOrDefault<ManagerServer.Model.ForeignCurrency>(exchangeRate.Currency.Value);

            if (foreignCurrency == null) return new Row[0];

            var previousExchangeRate = database.OfType<ExchangeRate>().Where(x => x.Date < exchangeRate.Date && x.Currency == exchangeRate.Currency.Value).Any();
            var nextExchangeRate = database.OfType<ExchangeRate>().Where(x => x.Date > exchangeRate.Date && x.Currency == exchangeRate.Currency.Value).OrderBy(x => x.Date).FirstOrDefault();

            var currencies = database.OfType<ForeignCurrency>().ToDictionary(x => x.Key, x => new HashSet<Guid>() { x.Key });
            foreach (var e in database.OfType<BankOrCashAccount>().Where(x => x.Currency.HasValue && currencies.ContainsKey(x.Currency.Value))) currencies[e.Currency.Value].Add(e.Key);
            foreach (var e in database.OfType<Customer>().Where(x => x.Currency.HasValue && currencies.ContainsKey(x.Currency.Value))) currencies[e.Currency.Value].Add(e.Key);
            foreach (var e in database.OfType<Supplier>().Where(x => x.Currency.HasValue && currencies.ContainsKey(x.Currency.Value))) currencies[e.Currency.Value].Add(e.Key);
            foreach (var e in database.OfType<Employee>().Where(x => x.Currency.HasValue && currencies.ContainsKey(x.Currency.Value))) currencies[e.Currency.Value].Add(e.Key);

            return database
                .UnorderedOfType<Transaction>()
                .OfType<IForeignCurrencyTransaction>()
                .Where(x => x.Currency.HasValue)
                .Where(x => currencies[exchangeRate.Currency.Value].Contains(x.Currency.Value))
                .Where(x => !previousExchangeRate || x.Date >= exchangeRate.Date)
                .Where(x => nextExchangeRate == null || x.Date < nextExchangeRate.Date)
                .Where(x => x is not InterAccountTransfer interAccountTransfer || interAccountTransfer.GetGeneralLedgerTransactions(database)[0].TransactionCurrency is ManagerServer.Model.ForeignCurrency)
                .OrderByDescending(x => x.Date)
                .Select(x => new Row()
                {
                    Date = x.Date,
                    Transaction = (Transaction)x,
                    ExchangeRate = GetExchangeRateDelta(baseCurrency, foreignCurrency, x, exchangeRate)
                })
                .ToArray();
        }

        private Delta GetExchangeRateDelta(ManagerServer.Model.BaseCurrency baseCurrency, ManagerServer.Model.ForeignCurrency foreignCurrency, IForeignCurrencyTransaction transaction, ExchangeRate exchangeRate)
        {
            if (transaction.ExchangeRate == exchangeRate.ExchangeRateValue && transaction.ExchangeRateIsInverse == exchangeRate.ExchangeRateIsInverse)
            {
                var value = baseCurrency.GetDisplayString(foreignCurrency, transaction.ExchangeRate, transaction.ExchangeRateIsInverse);
                return new Delta(value, value);
            }
            else
            {
                return new Delta(baseCurrency.GetDisplayString(foreignCurrency, transaction.ExchangeRate, transaction.ExchangeRateIsInverse), baseCurrency.GetDisplayString(foreignCurrency, exchangeRate.ExchangeRateValue, exchangeRate.ExchangeRateIsInverse));
            }
        }

        protected override byte[] GetCheckbox(Row o)
        {
            if (o.ExchangeRate.oldValue != o.ExchangeRate.newValue)
            {
                return o.Transaction.Key.ToByteArray();
            }
            return null;
        }

        protected override BusinessTemplate GetEdit(Row o, string referrer)
        {
            if (o.Transaction is ManagerServer.Model.InterAccountTransfer) return new InterAccountTransfers.InterAccountTransferForm() { Business = Business, Key = o.Transaction.Key, Referrer = referrer };
            if (o.Transaction is ManagerServer.Model.Payment) return new Payments.PaymentForm() { Business = Business, Key = o.Transaction.Key, Referrer = referrer };
            if (o.Transaction is ManagerServer.Model.Receipt) return new Receipts.ReceiptForm() { Business = Business, Key = o.Transaction.Key, Referrer = referrer };
            if (o.Transaction is ManagerServer.Model.SalesInvoice) return new SalesInvoices.SalesInvoiceForm() { Business = Business, Key = o.Transaction.Key, Referrer = referrer };
            if (o.Transaction is ManagerServer.Model.CreditNote) return new CreditNotes.CreditNoteForm() { Business = Business, Key = o.Transaction.Key, Referrer = referrer };
            if (o.Transaction is ManagerServer.Model.PurchaseInvoice) return new PurchaseInvoices.PurchaseInvoiceForm() { Business = Business, Key = o.Transaction.Key, Referrer = referrer };
            if (o.Transaction is ManagerServer.Model.DebitNote) return new DebitNotes.DebitNoteForm() { Business = Business, Key = o.Transaction.Key, Referrer = referrer };
            if (o.Transaction is ManagerServer.Model.Payslip) return new Payslips.PayslipForm() { Business = Business, Key = o.Transaction.Key, Referrer = referrer };
            if (o.Transaction is ManagerServer.Model.ExpenseClaim) return new ExpenseClaims.ExpenseClaimForm() { Business = Business, Key = o.Transaction.Key, Referrer = referrer };
            if (o.Transaction is ManagerServer.Model.BillableTime) return new BillableTime.BillableTimeEntryForm() { Business = Business, Key = o.Transaction.Key, Referrer = referrer };
            if (o.Transaction is ManagerServer.Model.LatePaymentFee) return new LatePaymentFees.LatePaymentFeeForm() { Business = Business, Key = o.Transaction.Key, Referrer = referrer };
            if (o.Transaction is ManagerServer.Model.WithholdingTaxReceipt) return new WithholdingTaxReceipts.WithholdingTaxReceiptForm() { Business = Business, Key = o.Transaction.Key, Referrer = referrer };
            if (o.Transaction is ManagerServer.Model.JournalEntry) return new JournalEntries.JournalEntryForm() { Business = Business, Key = o.Transaction.Key, Referrer = referrer };
            return null;
        }        

        /*
        public Tuple<string, string>[] GetExchangeRate(Transaction[] rows)
        {
            var baseCurrency = Manager.ApplicationData.Businesses.Get(FileID).Single<Manager.Model.BaseCurrency>();
            var exchangeRate = Manager.ApplicationData.Businesses.Get(FileID).SingleOrDefault<ExchangeRate>(ExchangeRate);
            var foreignCurrency = Manager.ApplicationData.Businesses.Get(FileID).SingleOrDefault<Manager.Model.ForeignCurrency>(exchangeRate.Currency.Value);
            var list = new List<Tuple<string, string>>();
            foreach (var e in rows.Cast<IForeignCurrencyTransaction>())
            {
                if (e.ExchangeRate == exchangeRate.ExchangeRateValue && e.ExchangeRateIsInverse == exchangeRate.ExchangeRateIsInverse)
                {
                    list.Add(new Tuple<string, string>(null, baseCurrency.GetDisplayString(foreignCurrency, e.ExchangeRate, e.ExchangeRateIsInverse)));
                }
                else
                {
                    list.Add(new Tuple<string, string>(baseCurrency.GetDisplayString(foreignCurrency, e.ExchangeRate, e.ExchangeRateIsInverse), baseCurrency.GetDisplayString(foreignCurrency, exchangeRate.ExchangeRateValue, exchangeRate.ExchangeRateIsInverse)));
                }
            }
            return list.ToArray();
        }
        */

        public record Row
        {
            [Guide("The date when each transaction occurred.")]
            [Guide("Transactions are filtered to show only those that fall within this exchange rate's effective period.")]
            [Guide("This helps identify which transactions will be affected if you update the exchange rate.")]
            [MinWidth, WhitespaceNoWrap, Center]
            public DateTime Date { get; set; }

            [Guide("The type and reference number of each transaction using this *exchange rate*.")]
            [Guide("Transaction types include *Sales Invoices*, *Payments*, *Receipts*, *Purchase Invoices*, and other foreign currency transactions.")]
            [Guide("Click on any transaction to open its edit form where you can manually update the exchange rate if needed.")]
            public ManagerServer.Model.Transaction Transaction { get; set; }

            [Guide("Shows the current exchange rate used by each transaction compared to the new exchange rate.")]
            [Guide("Transactions already using the new rate show only the current value.")]
            [Guide("Transactions with different rates show both the old rate (crossed out in red) and the new rate (in green) that will be applied if you use **Batch Update**.")]
            [Right, Bold]
            public Delta ExchangeRate { get; set; }
        }

        protected override void OnCustomCheckbox(byte[][] values)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var exchangeRate = database.SingleOrDefault<ExchangeRate>(ExchangeRate);
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in values)
            {
                var key = new Guid(e);
                var o = database.SingleOrDefault<ManagerServer.Model.Transaction>(key);
                if (o is IForeignCurrencyTransaction o2)
                {
                    o2.ExchangeRate = exchangeRate.ExchangeRateValue;
                    o2.ExchangeRateIsInverse = exchangeRate.ExchangeRateIsInverse;
                    list.Add(o);
                }
            }
            ApplicationData.Businesses.Process(Business, list.ToArray(), GetUserName());
        }        
    }
}
