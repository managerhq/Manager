using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Query.GeneralLedger;
using ProtoBuf;
using System;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("f71f59af-dbab-492f-83a8-6c0157360be8")]
    [Title(nameof(Strings.StartingBalance))]
    public sealed class SalesInvoiceStartingBalance : ManagerServer.Model.Transaction
    {
        [Guide("Select sales invoice that you have created under `SalesInvoices` tab.")]
        [ProtoMember(1), Autocomplete(typeof(SalesInvoice))] public Guid? SalesInvoice { get; set; }
        [Guide("Enter amount that has been partially paid for this sales invoice.")]
        [ProtoMember(2), Prepend(nameof(Strings.PartialPayment)), AppendCurrency(nameof(SalesInvoice), nameof(Strings.Customer))] public decimal StartingBalance { get; set; }

        public override string GetReference()
        {
            return string.Empty;
        }

        public override string GetName()
        {
            return null;
        }

        public override string GetDescriptionOrNull()
        {
            return null;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }

        public override string TransactionTitle => Strings.StartingBalance;

        public override GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            GeneralLedgerTransaction transaction = null;

            var salesInvoice = database.SingleOrDefault<SalesInvoice>(SalesInvoice);            

            if (salesInvoice != null)
            {
                var startingBalance = -StartingBalance;
                var baseCurrency = database.Single<BaseCurrency>();
                var customer = database.SingleOrDefault<Customer>(salesInvoice?.Customer);
                var currency = (Currency)database.SingleOrDefault<ForeignCurrency>(customer?.Currency) ?? database.Single<BaseCurrency>();
                var exchangeRate = database.Single<StartingExchangeRates>().GetExchangeRate(currency);
                var baseAmount = baseCurrency.GetBaseAmount(startingBalance, exchangeRate.ExchangeRate, exchangeRate.ExchangeRateIsInverse, currency);

                transaction = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    date: DateTime.MinValue,
                    generalLedgerAccount: database.Single<BalanceSheetAccountsReceivableAccount>(),
                    transactionAmount: startingBalance,
                    transactionCurrency: currency,
                    transaction: this,
                    exchangeRate: exchangeRate.ExchangeRate,
                    isExchangeRateInverse: exchangeRate.ExchangeRateIsInverse,
                    baseAmount: baseAmount,
                    customer: customer,
                    salesInvoice: salesInvoice
                );
            }

            if (transaction != null)
            {
                return
                [
                    transaction,
                    new GeneralLedgerTransaction(
                        database: database,
                        date: DateTime.MinValue,
                        generalLedgerAccount: database.Single<BalanceSheetRetainedEarningsAccount>(),
                        transactionAmount: -transaction.TransactionAmount,
                        transactionCurrency: transaction.TransactionCurrency,
                        transaction: this,
                        exchangeRate: transaction.ExchangeRate,
                        isExchangeRateInverse: transaction.IsExchangeRateInverse,
                        baseAmount: -transaction.BaseAmount,
                        isBalancing: true
                    )
                ];
            }

            return [];
        }
    }
}