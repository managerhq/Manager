using System.Linq;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerTransactionsForInvestments : BaseGeneralLedgerTransactionsForInvestmentGainsLosses
    {
        protected override void InnerGet4(Context context)
        {
            if (!GetRoot().Investment.HasValue)
            {
                var balanceSheetInvestmentsAccount = ApplicationData.Instance.Businesses.Get(Business).Single<BalanceSheetInvestmentsAccount>();
                var controlAccountForInvestments = ApplicationData.Instance.Businesses.Get(Business).SingleOrDefault<ControlAccountForInvestments>(GeneralLedgerAccount);

                if (controlAccountForInvestments != null || GeneralLedgerAccount == balanceSheetInvestmentsAccount.Key)
                {
                    var accountBalances = GetGeneralLedgerTransactions()
                        .GroupBy(x => x.Investment)
                        .Select(x => new { Investment = x.Key, Balance = x.Sum(y => y.BaseAmount), Qty = x.Sum(y => y.Qty ?? 0m) })
                        .OrderByDescending(x => Math.Abs(x.Balance))
                        .ThenBy(x => x.Investment.IsInactive())
                        .ThenBy(x => x.Investment.GetCodeAndName())
                        .Select(x => new InvestmentBalance()
                        {
                            Investment = x.Investment,
                            Balance = x.Balance,
                            Qty = x.Qty,
                            IsInactive = x.Investment.IsInactive()
                        })
                        .ToArray();

                    context.Set<Array>(accountBalances);
                }
            }

            base.InnerGet4(context);
        }

        public sealed class InvestmentBalance : IsInactive
        {
            public Investment Investment;
            public decimal Qty;
            public decimal Balance;
            public bool IsInactive;

            bool IsInactive.IsInactive => IsInactive;
        }

        [Default]
        [Guid("8475bad0-a4a7-46da-b13a-561db67382f0")]
        public string[] GetName(InvestmentBalance[] rows)
        {
            return rows.Select(x => x.Investment.GetCodeAndName()).ToArray();
        }

        [Default, Right]
        [Guid("ce24b91f-a910-45ab-9d98-c31e67150e38")]
        public Tuple<decimal, BusinessTemplate>[] GetQty(InvestmentBalance[] rows)
        {
            var referrer = ToUrl();

            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(
                x.Qty,
                GetHttpHandlerWithInvestment(x.Investment.Key, referrer)
            )).ToArray();
        }

        private BusinessTemplate GetHttpHandlerWithInvestment(Guid investment, string referrer)
        {
            var businessTemplate = Serializer.NonGeneric.DeepClone(this) as BaseGeneralLedgerTransactionsForSubaccount;
            businessTemplate.Investment = investment;
            businessTemplate.Referrer = referrer;
            businessTemplate.SortBy = null;
            return businessTemplate;
        }

        [Default, Right]
        [Guid("559bc704-0e11-432a-a657-6a8bae8d593d")]
        public Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>[] GetMarketPrice(InvestmentBalance[] rows)
        {
            var referrer = ToUrl();
            return GetInvestmentMarketPrices(GetRoot().To, Business, rows.Select(x => x.Investment).ToArray(), referrer);
        }

        [Default, Right, Sum, Bold]
        [Guid("825429c0-3b6e-47c2-839b-e117e871b38e")]
        public DebitCreditAmount[] GetMarketValue(InvestmentBalance[] rows)
        {
            return rows.Select(x => new DebitCreditAmount(x.Balance)).ToArray();
        }

        public static Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>[] GetInvestmentMarketPrices(DateTime date, string fileId, Investment[] rows, string referrer)
        {
            var database = ApplicationData.Instance.Businesses.Get(fileId);
            var baseCurrency = database.Single<BaseCurrency>();

            var investmentMarketPrices = database.OfType<InvestmentMarketPrice>()
                .Where(x => x.Date <= date)
                .Where(x => x.Investment.HasValue)
                .Where(x => x.MarketPrice > 0m)
                .OrderByDescending(x => x.Date)
                .GroupBy(x => x.Investment.Value)
                .ToDictionary(x => x.Key, x => x.First());

            var output = new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>[rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i] == null) continue;

                var row = rows[i];

                var httpHandler = new Settings.InvestmentMarketPrices.InvestmentMarketPriceForm()
                {
                    Business = fileId,
                    Referrer = referrer
                };

                if (investmentMarketPrices.TryGetValue(row.Key, out InvestmentMarketPrice investmentMarketPrice))
                {
                    if (investmentMarketPrice.Date == date)
                    {
                        httpHandler.Key = investmentMarketPrice.Key;
                    }
                    else
                    {
                        httpHandler.Date = date;
                        httpHandler.Investment = investmentMarketPrice.Investment;
                        httpHandler.Currency = investmentMarketPrice.Currency;
                        httpHandler.MarketPrice = investmentMarketPrice.MarketPrice;
                        httpHandler.ExchangeRate = investmentMarketPrice.ExchangeRate;
                        httpHandler.ExchangeRateIsInverse = investmentMarketPrice.ExchangeRateIsInverse;
                    }

                    Tuple<decimal, Currency> foreignCurrencyAmount = null;
                    var foreignCurrency = database.SingleOrDefault<ForeignCurrency>(investmentMarketPrice.Currency);
                    if (foreignCurrency != null)
                    {
                        foreignCurrencyAmount = new Tuple<decimal, Currency>(investmentMarketPrice.MarketPrice, foreignCurrency);
                    }

                    output[i] = new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>(
                        foreignCurrencyAmount,
                        new Tuple<decimal, Currency>(investmentMarketPrice.GetMarketPriceInBaseCurrency(baseCurrency).Value, baseCurrency),
                        httpHandler);
                }
                else
                {
                    httpHandler.Date = date;
                    httpHandler.Investment = row.Key;

                    output[i] = new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>, BusinessTemplate>(null, null, httpHandler);
                }
            }

            return output;
        }
    }
}