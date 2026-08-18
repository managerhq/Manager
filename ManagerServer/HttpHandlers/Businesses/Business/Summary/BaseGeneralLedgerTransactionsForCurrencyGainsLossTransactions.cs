using System.Linq;
using ManagerServer;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerTransactionsForCurrencyGainsLossTransactions : BaseGeneralLedgerTransactionsForCurrencyGainsLosses
    {
        [InheritedProtoMember(330)] public bool CurrencyGainsLossesCurrentBalance;
        [InheritedProtoMember(331)] public Guid CurrencyGainsLossesGeneralLedgerAccount;
        [InheritedProtoMember(332)] public Guid CurrencyGainsLossesSubaccount;
        [InheritedProtoMember(333)] public Guid Currency;
        [InheritedProtoMember(334)] public DateTime CurrencyGainsLossesFrom;
        [InheritedProtoMember(335)] public DateTime CurrencyGainsLossesTo;

        protected override void InnerGet4(Context context)
        {
            if (CurrencyGainsLossesCurrentBalance)
            {
                var database = ApplicationData.Businesses.Get(Business);
                var rows = new GeneralLedger(Business)
                    .Revaluate(CurrencyGainsLossesFrom, CurrencyGainsLossesTo)
                    .Where(x => x.GeneralLedgerAccount.Key == CurrencyGainsLossesGeneralLedgerAccount)
                    .Where(x => x.SubAccount.Key == CurrencyGainsLossesSubaccount)
                    .Where(x => x.Date <= CurrencyGainsLossesTo)
                    .OrderByDescending(x => x.Date)
                    .ToList();

                var startingBaseBalance = rows.Where(x => x.Date < CurrencyGainsLossesFrom).Sum(x => x.BaseAmount);
                var startingAccountBalance = rows.Where(x => x.Date < CurrencyGainsLossesFrom).Sum(x => x.AccountAmount);
                var firstRow = rows.FirstOrDefault();

                rows = rows.Where(x => x.Date >= CurrencyGainsLossesFrom && x.Transaction != null).OrderByDescending(x => x.Date).ToList();

                if (startingBaseBalance != 0m || startingAccountBalance != 0m)
                {
                    rows.Add(new GeneralLedgerTransaction(
                        database: database,
                        date: CurrencyGainsLossesFrom.AddDays(-1),
                        transactionAmount: startingAccountBalance,
                        transactionCurrency: firstRow.AccountCurrency,
                        generalLedgerAccount: firstRow.GeneralLedgerAccount,
                        bankAccount: firstRow.BankAccount,
                        specialAccount: firstRow.SpecialAccount,
                        customer: firstRow.Customer,
                        supplier: firstRow.Supplier,
                        employee: firstRow.Employee,
                        baseAmount: startingBaseBalance,
                        accountAmount: startingAccountBalance
                    ));
                }

                var exchangeRates = ApplicationData.Businesses.Get(Business).OfType<ExchangeRate>().Where(x => x.Currency == Currency && x.ExchangeRateValue > 0m).OrderByDescending(x => x.Date).ToArray();

                var newRows = rows
                    .Select(x => new GeneralLedgerTransactionExchangeRatePair()
                    {
                        Transaction = x,
                        MarketExchangeRate = exchangeRates.FirstOrDefault(y => y.Date <= x.Date)
                    }).ToArray();


                context.Set<Array>(newRows);
            }

            base.InnerGet4(context);
        }

        public sealed class GeneralLedgerTransactionExchangeRatePair
        {
            public GeneralLedgerTransaction Transaction;
            public ExchangeRate MarketExchangeRate;
        }

        [Icon("fa-edit")]
        [Default, MinWidth, Center, HideColumnIfAllEmpty]
        public BusinessTemplate[] GetEdit(GeneralLedgerTransactionExchangeRatePair[] rows)
        {
            var referrer = ToUrl();
            return rows.Select(x => TransactionViewer.GetEditHandler(Business, x.Transaction.Transaction, referrer)).ToArray();
        }

        [Icon("fa-eye")]
        [Default, MinWidth, Center, HideColumnIfAllEmpty]
        public BusinessTemplate[] GetView(GeneralLedgerTransactionExchangeRatePair[] rows)
        {
            var referrer = ToUrl();
            return rows.Select(x => TransactionViewer.GetViewHandler(Business, x.Transaction.Transaction, referrer)).ToArray();
        }

        [Default, MinWidth, Center, WhitespaceNoWrap]
        [Guid("6631a9cf-fdc3-4887-bfc3-8a4b8dc5b719")]
        public DateTime[] GetDate(GeneralLedgerTransactionExchangeRatePair[] rows)
        {
            return rows.Select(x => x.Transaction.Date).ToArray();
        }

        [Default, HideColumnIfAllEmpty, WhitespaceNoWrap]
        [Guid("ef489ea3-a85a-40c1-83de-cdd8f5ed89bf")]
        public string[] GetTransaction(GeneralLedgerTransactionExchangeRatePair[] rows)
        {
            return rows.Select(x => x.Transaction.Transaction != null ? x.Transaction.Transaction.GetTransactionName() : Strings.BalanceAtEndOfPeriod).ToArray();
        }

        [Default, Right, WhitespaceNoWrap, HideColumnIfAllEmpty]
        public Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>>[] GetTransactionConversion(GeneralLedgerTransactionExchangeRatePair[] rows)
        {
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            return rows.Select(x => new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>>(
                new Tuple<decimal, Currency>(Math.Abs(x.Transaction.AccountAmount), x.Transaction.AccountCurrency),
                new Tuple<decimal, Currency>(Math.Abs(x.Transaction.BaseAmount), baseCurrency)
            )).ToArray();
        }

        [Default, Center, WhitespaceNoWrap, HideColumnIfAllEmpty]
        public Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>[] GetMarketExchangeRate(GeneralLedgerTransactionExchangeRatePair[] rows)
        {
            var referrer = ToUrl();
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            return rows.Select(x => BaseGeneralLedgerTransactionsForSubaccount.GetExchangeRate(Business, x.MarketExchangeRate, x.Transaction.Date, referrer, baseCurrency, (ForeignCurrency)x.Transaction.AccountCurrency)).ToArray();
        }

        [Default, Right, WhitespaceNoWrap, HideColumnIfAllEmpty]
        public Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>>[] GetMarketConversion(GeneralLedgerTransactionExchangeRatePair[] rows)
        {
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            return rows.Select(x => new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>>(
                new Tuple<decimal, Currency>(Math.Abs(x.Transaction.AccountAmount), x.Transaction.AccountCurrency),
                new Tuple<decimal, Currency>(baseCurrency.Round(Math.Abs(x.Transaction.AccountAmount) / x.MarketExchangeRate?.GetBaseRate() ?? 1m), baseCurrency)
            )).ToArray();
        }

        [Default, Center, WhitespaceNoWrap, HideColumnIfAllEmpty]
        [Guid("2c41fb0a-cc71-4d7c-8f1f-960d5a246e51")]
        public Percentage[] GetConversionMarkup(GeneralLedgerTransactionExchangeRatePair[] rows)
        {
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();

            var output = new Percentage[rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                var baseAmount = Math.Abs(rows[i].Transaction.BaseAmount);
                var marketAmount = baseCurrency.Round(Math.Abs(rows[i].Transaction.AccountAmount) / rows[i].MarketExchangeRate?.GetBaseRate() ?? 1m);

                if (baseAmount != marketAmount && baseAmount != 0m)
                {
                    var percentage = new Percentage() { Value = CalculatePercentageDifference(baseAmount, marketAmount) };
                    if (percentage.Value != 0) output[i] = percentage;
                }
            }

            return output;
        }

        public static decimal CalculatePercentageDifference(decimal value1, decimal value2)
        {
            var difference = Math.Abs(value1 - value2);
            var percentageDifference = difference / value1 * 100;

            return Math.Round(percentageDifference, 0, MidpointRounding.AwayFromZero);
        }

        [Default, Right, Bold, Sum, WhitespaceNoWrap, HideColumnIfAllEmpty]
        [Guid("95a62f1b-1394-41e5-807a-a21fa8c0f9d0")]
        public Tuple<decimal, Currency>[] GetDebit(GeneralLedgerTransactionExchangeRatePair[] rows)
        {
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            return rows.Select(x => x.Transaction.BaseAmount > 0m ? new Tuple<decimal, Currency>(x.Transaction.BaseAmount, baseCurrency) : null).ToArray();
        }

        [Default, Right, Bold, Sum, WhitespaceNoWrap, HideColumnIfAllEmpty]
        [Guid("4320645f-21a4-4de1-b3ad-65e3f8cfbc7f")]
        public Tuple<decimal, Currency>[] GetCredit(GeneralLedgerTransactionExchangeRatePair[] rows)
        {
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();
            return rows.Select(x => x.Transaction.BaseAmount < 0m ? new Tuple<decimal, Currency>(x.Transaction.BaseAmount * -1m, baseCurrency) : null).ToArray();
        }

        [Default, Right, WhitespaceNoWrap, RunningTotal2]
        public Tuple<decimal, ManagerServer.Model.Enums.DebitCredit>[] GetBalance(GeneralLedgerTransactionExchangeRatePair[] rows)
        {
            var balance = rows.Sum(x => x.Transaction.BaseAmount);

            var output = new Tuple<decimal, ManagerServer.Model.Enums.DebitCredit>[rows.Length];
            for (int i = 0; i < output.Length; i++)
            {
                if (balance >= 0m)
                {
                    output[i] = new Tuple<decimal, ManagerServer.Model.Enums.DebitCredit>(balance, ManagerServer.Model.Enums.DebitCredit.Debit);
                }
                else
                {
                    output[i] = new Tuple<decimal, ManagerServer.Model.Enums.DebitCredit>(balance * -1m, ManagerServer.Model.Enums.DebitCredit.Credit);
                }

                balance -= rows[i].Transaction.BaseAmount;
            }
            return output;
        }
    }
}