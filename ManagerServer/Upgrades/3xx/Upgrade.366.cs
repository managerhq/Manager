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
        private static async Task<IEnumerable<Model.Object>> Upgrade366(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ForeignCurrency>().Where(x => x.Obsolete_DefaultExchangeRate == 0m).ToArray())
            {
                var exchangeRate = objects.OfType<ExchangeRate>().Where(x => x.Currency == e.Key && (x.Obsolete_BaseRate > 0m || x.Obsolete_CounterRate > 0m)).OrderByDescending(x => x.Date).FirstOrDefault();
                if (exchangeRate != null)
                {
                    if (exchangeRate.Obsolete_Type == ExchangeRateType.BaseRate)
                    {
                        e.Obsolete_DefaultExchangeRate = exchangeRate.Obsolete_BaseRate;
                        e.Obsolete_DefaultExchangeRateIsInverse = true;
                    }
                    else
                    {
                        e.Obsolete_DefaultExchangeRate = exchangeRate.Obsolete_CounterRate;
                        e.Obsolete_DefaultExchangeRateIsInverse = false;
                    }
                }
                else
                {
                    e.Obsolete_DefaultExchangeRate = e.Obsolete_StartingExchangeRate > 0m ? e.Obsolete_StartingExchangeRate : 1m;
                    e.Obsolete_DefaultExchangeRateIsInverse = true;
                }
                list.Add(e);
            }
            return list;
        }
    }
}
