using System.Linq;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Query.GeneralLedger;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerTransactionsForCurrencyGainsLosses : BaseGeneralLedgerTransactionsBase
    {
        protected override void InnerGet4(Context context)
        {
            var currencyGainsLosses = ApplicationData.Businesses.Get(Business).Single<ProfitAndLossStatementAccountCurrencyGainsLosses>();

            if (GeneralLedgerAccount == currencyGainsLosses.Key)
            {
                var baseCurrency = ApplicationData.Businesses.Get(Business).Single<BaseCurrency>();

                var generalLedger = GetGeneralLedgerTransactions();

                var balances = new GeneralLedger(Business)
                .Revaluate(GetRoot().From ?? DateTime.MinValue, GetRoot().To)
                .Where(x => x.Date <= GetRoot().To)
                .GroupBy(x => x.ForeignCurrencyAccount)
                .Where(x => x.Key != null)
                .Select(x => new CurrencyGainLoss(baseCurrency)
                {
                    GeneralLedgerAccount = x.First().GeneralLedgerAccount,
                    ForeignCurrencyAccount = x.First().ForeignCurrencyAccount,
                    Currency = (ForeignCurrency)x.First().AccountCurrency,
                    CurrencyBalance = x.Sum(y => y.AccountAmount),
                    BaseBalance = x.Where(x => x.Transaction != null || x.Date < GetRoot().From).Sum(x => x.BaseAmount),
                    IncrementForThePeriod = x.Where(x => x.Transaction == null && x.Date > GetRoot().From).Sum(y => y.BaseAmount) * -1m
                })
                .Where(x => x.CurrencyBalance != 0m || x.BaseBalance != 0m || x.IncrementForThePeriod != 0m)
                .OrderBy(x => x.GeneralLedgerAccount.GetCodeAndName())
                .ThenBy(x => x.ForeignCurrencyAccount.GetCodeAndName())
                .ToArray();

                context.Set<Array>(balances);
            }

            base.InnerGet4(context);
        }

        public sealed class CurrencyGainLoss
        {
            public BaseCurrency BaseCurrency { get; init; }

            public CurrencyGainLoss(BaseCurrency baseCurrency)
            {
                BaseCurrency = baseCurrency;
            }

            public IGeneralLedgerAccount GeneralLedgerAccount;
            public NamedObject ForeignCurrencyAccount;
            public ForeignCurrency Currency;
            public decimal CurrencyBalance;
            public decimal BaseBalance;
            public decimal RevaluatedBalance => BaseBalance - IncrementForThePeriod;
            public decimal IncrementForThePeriod;
        }

        [Default]
        public string[] GetAccount(CurrencyGainLoss[] rows)
        {
            return rows.Select(x => $"{x.GeneralLedgerAccount.GetCodeAndName()} — {x.ForeignCurrencyAccount.GetCodeAndName()}").ToArray();
        }

        [Default, Sum, Right, WhitespaceNoWrap]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetForeignBalance(CurrencyGainLoss[] rows)
        {
            var referrer = ToUrl();

            return rows.Select(x => new Tuple<decimal, Currency, BusinessTemplate>(
                x.CurrencyBalance,
                x.Currency,
                CreateHttpHandlerForForeignBalance(x.GeneralLedgerAccount.Key, GetRoot().To, x.ForeignCurrencyAccount.Key, referrer)
            )).ToArray();
        }

        private BusinessTemplate CreateHttpHandlerForForeignBalance(Guid generalLedgerAccount, DateTime date, Guid subaccount, string referrer)
        {
            var httpHandler = (BaseGeneralLedgerTransactionsInheritable)Activator.CreateInstance(GetType());
            httpHandler.Business = Business;
            httpHandler.GeneralLedgerAccount = generalLedgerAccount;
            httpHandler.To = date;
            httpHandler.Subaccount = subaccount;
            httpHandler.Referrer = referrer;
            httpHandler.HttpContext = HttpContext;
            return httpHandler;
        }

        [Default, Center, HideColumnIfAllEmpty]
        public Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>[] GetExchangeRate(CurrencyGainLoss[] rows)
        {
            return GetExchangeRate(rows.Select(x => x.Currency).ToArray());
        }

        [Default, Sum, Right, WhitespaceNoWrap]
        public Tuple<decimal, Currency>[] GetConvertedBalance(CurrencyGainLoss[] rows)
        {
            return rows.Select(x => new Tuple<decimal, Currency>(x.RevaluatedBalance, x.BaseCurrency)).ToArray();
        }

        [Default, Sum, Right, WhitespaceNoWrap]
        public Tuple<decimal, Currency, BusinessTemplate>[] GetClosingBalance(CurrencyGainLoss[] rows)
        {
            var referrer = ToUrl();

            return rows.Select(x => new Tuple<decimal, Currency, BusinessTemplate>(
                x.BaseBalance,
                x.BaseCurrency,
                CreateHttpHandlerForCurrentBalance(x.GeneralLedgerAccount.Key, GetRoot().From.Value, GetRoot().To, x.ForeignCurrencyAccount.Key, x.Currency.Key, referrer)
            )).ToArray();
        }

        private BusinessTemplate CreateHttpHandlerForCurrentBalance(Guid generalLedgerAccount, DateTime from, DateTime to, Guid subaccount, Guid foreignCurrency, string referrer)
        {
            var httpHandler = (BaseGeneralLedgerTransactionsInheritable)Activator.CreateInstance(GetType());
            httpHandler.Business = Business;
            httpHandler.CurrencyGainsLossesGeneralLedgerAccount = generalLedgerAccount;
            httpHandler.CurrencyGainsLossesSubaccount = subaccount;
            httpHandler.Currency = foreignCurrency;
            httpHandler.CurrencyGainsLossesFrom = from;
            httpHandler.CurrencyGainsLossesTo = to;
            httpHandler.CurrencyGainsLossesCurrentBalance = true;
            httpHandler.Referrer = referrer;
            httpHandler.HttpContext = HttpContext;
            return httpHandler;
        }

        [Default, Bold, Sum, Right, WhitespaceNoWrap]
        public Tuple<decimal, DebitCredit>[] GetGainLoss(CurrencyGainLoss[] rows)
        {
            return rows.Select(x => new Tuple<decimal, DebitCredit>(
                Math.Abs(x.IncrementForThePeriod),
                x.IncrementForThePeriod >= 0 ? DebitCredit.Debit : DebitCredit.Credit
            )).ToArray();
        }

        private Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>[] GetExchangeRate(ForeignCurrency[] rows)
        {
            var referrer = ToUrl();
            return BaseGeneralLedgerTransactionsForSubaccount.GetExchangeRates(GetRoot().To, Business, rows, referrer);
        }
    }
}