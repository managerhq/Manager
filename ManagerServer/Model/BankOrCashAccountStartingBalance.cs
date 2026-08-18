using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Query.GeneralLedger;
using ProtoBuf;
using System;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("0c1000da-6cc3-4448-8245-6f1eeccab8d6")]
    [Title(nameof(Strings.StartingBalance))]
    public sealed class BankOrCashAccountStartingBalance : ManagerServer.Model.Transaction
    {
        [Guide("Select the bank or cash account for which you want to set a starting balance.")]
        [Guide("Starting balances are used when you begin using the software with existing bank account balances.")]
        [ProtoMember(1), Autocomplete(typeof(BankOrCashAccount))] public Guid? BankOrCashAccount { get; set; }
        [Guide("Enter the cleared balance from your bank statement as of your start date.")]
        [Guide("This should be the actual cleared balance shown on your bank statement, not your available balance.")]
        [Guide("Important: Do NOT include pending transactions in this balance:")]
        [Guide("• Outstanding checks should be entered as separate payments with pending status")]
        [Guide("• Deposits in transit should be entered as separate receipts with pending status")]
        [Guide("• This ensures accurate bank reconciliation when pending items clear")]
        [Guide("The starting balance date is typically the day before you begin recording new transactions in the system.")]
        [ProtoMember(2), Prepend(nameof(Strings.ClearedBalance)), AppendCurrency(nameof(BankOrCashAccount))] public decimal StartingBalance { get; set; }

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

            var bankOrCashAccount = database.SingleOrDefault<BankOrCashAccount>(BankOrCashAccount);

            if (bankOrCashAccount != null)
            {
                var baseCurrency = database.Single<BaseCurrency>();
                var currency = (Currency)database.SingleOrDefault<ForeignCurrency>(bankOrCashAccount.Currency) ?? baseCurrency;
                var exchangeRate = database.Single<StartingExchangeRates>().GetExchangeRate(currency);
                var baseAmount = baseCurrency.GetBaseAmount(StartingBalance, exchangeRate.ExchangeRate, exchangeRate.ExchangeRateIsInverse, currency);

                transaction = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    date: DateTime.MinValue,
                    generalLedgerAccount: database.Single<BalanceSheetCashAtBankAccount>(),
                    transactionAmount: StartingBalance,
                    transactionCurrency: currency,
                    exchangeRate: exchangeRate.ExchangeRate,
                    isExchangeRateInverse: exchangeRate.ExchangeRateIsInverse,
                    baseAmount: baseAmount,
                    transaction: this,
                    bankAccount: bankOrCashAccount
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
                        exchangeRate: transaction.ExchangeRate,
                        isExchangeRateInverse: transaction.IsExchangeRateInverse,
                        baseAmount: -transaction.BaseAmount,
                        transaction: this,
                        isBalancing: true
                    )
                ];
            }

            return [];
        }
    }
}