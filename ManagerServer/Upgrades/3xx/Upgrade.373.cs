using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using System.Text;
using System.IO;
using ManagerServer.Model;
using System.Reflection;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Obsolete;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade373(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var foreignCurrencyBankOrCashAccounts = objects.OfType<BankOrCashAccount>().Where(x => x.Currency.HasValue).ToDictionary(x => x.Key, x => x.Currency.Value);
            if (foreignCurrencyBankOrCashAccounts.Any())
            {
                var exchangeRates = new Dictionary<Guid, Dictionary<DateTime, ExchangeRate>>();
                foreach (var e in objects.OfType<Receipt>().Where(x => x.ReceivedIn.HasValue && foreignCurrencyBankOrCashAccounts.ContainsKey(x.ReceivedIn.Value)).ToArray())
                {
                    var currency = foreignCurrencyBankOrCashAccounts[e.ReceivedIn.Value];
                    if (!exchangeRates.ContainsKey(currency)) exchangeRates.Add(currency, new Dictionary<DateTime, ExchangeRate>());
                    if (!exchangeRates[currency].ContainsKey(e.Date))
                    {
                        var exchangeRate2 = objects.OfType<ManagerServer.Model.ExchangeRate>().Where(x => x.Currency == currency && x.Date <= e.Date && x.ExchangeRateValue > 0m).OrderByDescending(x => x.Date).FirstOrDefault();
                        if (exchangeRate2 != null)
                        {
                            exchangeRates[currency].Add(e.Date, exchangeRate2);
                        }
                    }
                    if (exchangeRates[currency].TryGetValue(e.Date, out ExchangeRate exchangeRate))
                    {
                        e.ExchangeRate = exchangeRate.ExchangeRateValue;
                        e.ExchangeRateIsInverse = exchangeRate.ExchangeRateIsInverse;
                        list.Add(e);
                    }
                    else
                    {
                        var foreignCurrency = objects.SingleOrDefault<ForeignCurrency>(currency);
                        if (foreignCurrency != null)
                        {
                            e.ExchangeRate = foreignCurrency.Obsolete_StartingExchangeRate > 0m ? foreignCurrency.Obsolete_StartingExchangeRate : 1m;
                            e.ExchangeRateIsInverse = true;
                            list.Add(e);
                        }
                    }
                }
                foreach (var e in objects.OfType<Payment>().Where(x => x.PaidFrom.HasValue && foreignCurrencyBankOrCashAccounts.ContainsKey(x.PaidFrom.Value)).ToArray())
                {
                    var currency = foreignCurrencyBankOrCashAccounts[e.PaidFrom.Value];
                    if (!exchangeRates.ContainsKey(currency)) exchangeRates.Add(currency, new Dictionary<DateTime, ExchangeRate>());
                    if (!exchangeRates[currency].ContainsKey(e.Date))
                    {
                        var exchangeRate2 = objects.OfType<ManagerServer.Model.ExchangeRate>().Where(x => x.Currency == currency && x.Date <= e.Date && x.ExchangeRateValue > 0m).OrderByDescending(x => x.Date).FirstOrDefault();
                        if (exchangeRate2 != null)
                        {
                            exchangeRates[currency].Add(e.Date, exchangeRate2);
                        }
                    }
                    if (exchangeRates[currency].TryGetValue(e.Date, out ExchangeRate exchangeRate))
                    {
                        e.ExchangeRate = exchangeRate.ExchangeRateValue;
                        e.ExchangeRateIsInverse = exchangeRate.ExchangeRateIsInverse;
                        list.Add(e);
                    }
                    else
                    {
                        var foreignCurrency = objects.SingleOrDefault<ForeignCurrency>(currency);
                        if (foreignCurrency != null)
                        {
                            e.ExchangeRate = foreignCurrency.Obsolete_StartingExchangeRate > 0m ? foreignCurrency.Obsolete_StartingExchangeRate : 1m;
                            e.ExchangeRateIsInverse = true;
                            list.Add(e);
                        }
                    }
                }
            }
            return list;
        }
    }
}
