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
        private static async Task<IEnumerable<Model.Object>> Upgrade230(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var currencies = ManagerServer.Model.Obsolete.Obsolete54.CurrencyKeys.All.ToDictionary(x => x.Key);
            var list = new List<Model.Object>();
            var baseCurrency = objects.OfType<ManagerServer.Model.BaseCurrency>().SingleOrDefault(x => x.Key == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BaseCurrency)));
            if (baseCurrency != null && baseCurrency.Obsolete_Currency.HasValue && currencies.ContainsKey(baseCurrency.Obsolete_Currency.Value))
            {
                var baseExchangeRate = currencies[baseCurrency.Obsolete_Currency.Value].ExchangeRate;
                foreach (var e in objects.OfType<ManagerServer.Model.ForeignCurrency>().Where(x => currencies.ContainsKey(x.Key)).ToList())
                {
                    var exchangeRate = currencies[e.Key].ExchangeRate;
                    var decimals = 4;
                    if (baseExchangeRate != 1m) exchangeRate = Math.Round(currencies[e.Key].ExchangeRate / baseExchangeRate, decimals);
                    while (exchangeRate == 0m)
                    {
                        decimals++;
                        exchangeRate = Math.Round(currencies[e.Key].ExchangeRate / baseExchangeRate, decimals);
                    }

                    list.Add(new ManagerServer.Model.ExchangeRate() { Key = Guid.CreateVersion7(), Date = DateTime.MinValue, Currency = e.Key, Obsolete_BaseRate = exchangeRate });
                }
            }
            return list;
        }
    }
}
