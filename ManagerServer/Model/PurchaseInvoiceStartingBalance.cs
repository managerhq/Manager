using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("e9333683-ea93-4949-99f3-28e60278b68d")]
    [Title(nameof(Strings.StartingBalance))]
    public sealed class PurchaseInvoiceStartingBalance : ManagerServer.Model.Transaction
    {
        [Guide("Select purchase invoice that you have created under `PurchaseInvoices` tab.")]
        [ProtoMember(1), Autocomplete(typeof(PurchaseInvoice))] public Guid? PurchaseInvoice { get; set; }
        [Guide("Enter amount that has been partially paid for this purchase invoice.")]
        [ProtoMember(2), Prepend(nameof(Strings.PartialPayment)), AppendCurrency(nameof(PurchaseInvoice), nameof(Strings.Supplier))] public decimal StartingBalance { get; set; }

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

            var purchaseInvoice = database.SingleOrDefault<PurchaseInvoice>(PurchaseInvoice);            

            if (purchaseInvoice != null)
            {
                var baseCurrency = database.Single<BaseCurrency>();
                var supplier = database.SingleOrDefault<Supplier>(purchaseInvoice?.Supplier);
                var currency = (Currency)database.SingleOrDefault<ForeignCurrency>(supplier?.Currency) ?? database.Single<BaseCurrency>();
                var exchangeRate = database.Single<StartingExchangeRates>().GetExchangeRate(currency);
                var baseAmount = baseCurrency.GetBaseAmount(StartingBalance, exchangeRate.ExchangeRate, exchangeRate.ExchangeRateIsInverse, currency);

                transaction = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    date: DateTime.MinValue,
                    generalLedgerAccount: database.Single<BalanceSheetAccountsPayableAccount>(),
                    transactionAmount: StartingBalance,
                    transactionCurrency: currency,
                    transaction: this,
                    supplier: supplier,
                    exchangeRate: exchangeRate.ExchangeRate,
                    isExchangeRateInverse: exchangeRate.ExchangeRateIsInverse,
                    baseAmount: baseAmount,
                    purchaseInvoice: purchaseInvoice
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
                        transactionAmount: transaction.TransactionAmount * -1m,
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