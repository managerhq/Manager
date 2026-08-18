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
        private static async Task<IEnumerable<Model.Object>> Upgrade231(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var originalCurrencies = ManagerServer.Model.Obsolete.Obsolete54.CurrencyKeys.All.ToDictionary(x => x.Key);
            var foreignCurrencies = objects.OfType<ManagerServer.Model.ForeignCurrency>().ToDictionary(x => x.Key);
            foreach (var e in objects.OfType<ManagerServer.Model.ExchangeRate>().Where(x => x.Currency.HasValue).Select(x => x.Currency.Value).Distinct())
            {
                if (!foreignCurrencies.ContainsKey(e))
                {
                    if (originalCurrencies.ContainsKey(e))
                    {
                        var currency2 = originalCurrencies[e];
                        list.Add(new ManagerServer.Model.ForeignCurrency()
                        {
                            Key = e,
                            Code = currency2.Code,
                            Name = currency2.Name,
                            Prefix = currency2.Prefix,
                            DecimalPlaces = currency2.DecimalPlaces == 2 ? null : (int?)currency2.DecimalPlaces,
                        });
                    }
                }
            }
            return list;
        }
    }
}
