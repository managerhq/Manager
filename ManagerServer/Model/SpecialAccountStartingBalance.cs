using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("371c8de0-293e-4b4d-8e16-e1e2548ff8e2")]
    [Title(nameof(Strings.StartingBalance))]
    public sealed class SpecialAccountStartingBalance : ManagerServer.Model.Transaction
    {
        [Guide("Select special account that you have created under `SpecialAccounts` tab.")]
        [ProtoMember(1), Autocomplete(typeof(SpecialAccount))] public Guid? SpecialAccount { get; set; }
        [Guide("Select whether starting balance represents debit or credit amount. Typically, you select `Debit` for asset account and `Credit` for liability accounts.")]
        [ProtoMember(2), NoWrap, Label(nameof(Strings.StartingBalance))] public DebitCredit DebitCredit { get; set; }
        [Guide("Enter the opening balance amount for this special account. This represents the special account balance at the beginning of your accounting period in Manager.")]
        [ProtoMember(3), EmptyLabel, AppendCurrency(nameof(SpecialAccount))] public decimal StartingBalance { get; set; }

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

            var specialAccount = database.SingleOrDefault<SpecialAccount>(SpecialAccount);

            if (specialAccount != null)
            {
                var startingBalance = DebitCredit == DebitCredit.Debit ? StartingBalance : StartingBalance * -1m;
                var baseCurrency = database.Single<BaseCurrency>();
                var currency = (Currency)database.SingleOrDefault<ForeignCurrency>(specialAccount.Currency) ?? database.Single<BaseCurrency>();
                var exchangeRate = database.Single<StartingExchangeRates>().GetExchangeRate(currency);
                var baseAmount = baseCurrency.GetBaseAmount(startingBalance, exchangeRate.ExchangeRate, exchangeRate.ExchangeRateIsInverse, currency);

                transaction = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    date: DateTime.MinValue,
                    generalLedgerAccount: database.Single<BalanceSheetSpecialAccountsAccount>(),
                    transactionAmount: startingBalance,
                    transactionCurrency: currency,
                    transaction: this,
                    exchangeRate: exchangeRate.ExchangeRate,
                    isExchangeRateInverse: exchangeRate.ExchangeRateIsInverse,
                    baseAmount: baseAmount,
                    specialAccount: specialAccount
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