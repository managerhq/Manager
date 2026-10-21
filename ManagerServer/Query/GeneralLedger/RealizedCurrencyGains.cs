using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Query.GeneralLedger
{
    internal static class RealizedCurrencyGains
    {
        internal static IEnumerable<Step> Replay(Database business, string fileId, DateTime toDate)
        {
            var baseCurrency = business.Single<BaseCurrency>();
            var transactions = new GeneralLedger(fileId).Where(x => x.Date <= toDate).ToArray();

            foreach (var e in transactions.Where(x => x.AccountCurrency is ForeignCurrency).GroupBy(x => x.AccountCurrency))
            {
                var currency = (ForeignCurrency)e.Key;

                var foreignCurrencyBalance = 0m;
                var baseCurrencyBalance = 0m;
                var realizationIndex = 0;

                Step Advance(GeneralLedgerTransaction entry, DateTime date, decimal foreignAmount, decimal baseAmount)
                {
                    baseCurrencyBalance += baseAmount;
                    foreignCurrencyBalance += foreignAmount;

                    return new Step
                    {
                        Currency = currency,
                        Entry = entry,
                        Date = date,
                        ForeignAmount = foreignAmount,
                        BaseAmount = baseAmount,
                        ForeignBalance = foreignCurrencyBalance,
                        BaseBalance = baseCurrencyBalance
                    };
                }

                foreach (var e2 in e.GroupBy(x => x.Date).OrderBy(x => x.Key))
                {
                    var foreignCurrencyMovement = e2.Sum(x => x.AccountAmount);

                    if (foreignCurrencyBalance != 0m && foreignCurrencyMovement != 0m && Math.Sign(foreignCurrencyBalance) != Math.Sign(foreignCurrencyMovement))
                    {
                        foreach (var e3 in e2.OrderByDescending(x => x.Transaction.CanBeRealizedCurrencyTransaction(business)).ThenByDescending(x => Math.Sign(x.AccountAmount) == Math.Sign(foreignCurrencyBalance)))
                        {
                            if (Math.Sign(e3.AccountAmount) == Math.Sign(foreignCurrencyBalance))
                            {
                                yield return Advance(e3, e2.Key, e3.AccountAmount, e3.BaseAmount);
                            }
                            else if (foreignCurrencyBalance == 0m)
                            {
                                yield return Advance(e3, e2.Key, e3.AccountAmount, e3.BaseAmount);
                            }
                            else if (!e3.Transaction.CanBeRealizedCurrencyTransaction(business))
                            {
                                yield return Advance(e3, e2.Key, e3.AccountAmount, e3.BaseAmount);
                            }
                            else if (e3.AccountAmount == 0m)
                            {
                                yield return Advance(e3, e2.Key, e3.AccountAmount, e3.BaseAmount);
                            }
                            else
                            {
                                var unrealizedExchangeRate = baseCurrencyBalance / foreignCurrencyBalance;
                                var cost = baseCurrency.Round(e3.AccountAmount * unrealizedExchangeRate);
                                var gain = cost - e3.BaseAmount;

                                var step = Advance(e3, e2.Key, e3.AccountAmount, cost);
                                step.IsRealization = true;
                                step.Cost = cost;
                                step.Gain = gain;
                                step.RealizationIndex = realizationIndex++;

                                yield return step;
                            }
                        }
                    }
                    else
                    {
                        foreach (var e3 in e2)
                        {
                            yield return Advance(e3, e2.Key, e3.AccountAmount, e3.BaseAmount);
                        }
                    }
                }
            }
        }

        internal sealed class Step
        {
            public ForeignCurrency Currency;
            public GeneralLedgerTransaction Entry;
            public DateTime Date;
            public decimal ForeignAmount;
            public decimal BaseAmount;
            public decimal ForeignBalance;
            public decimal BaseBalance;
            public bool IsRealization;
            public decimal Cost;
            public decimal Gain;
            public int RealizationIndex = -1;
        }
    }
}
