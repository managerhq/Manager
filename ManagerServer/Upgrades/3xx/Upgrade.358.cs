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
        private static async Task<IEnumerable<Model.Object>> Upgrade358(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.ForeignCurrency>().Where(x => x.Obsolete_StartingExchangeRate == 0m))
            {
                var first = objects.OfType<ExchangeRate>().Where(x => x.Currency == e.Key).OrderBy(x => x.Date).FirstOrDefault();
                if (first != null)
                {
                    if (first.Obsolete_Type == ExchangeRateType.CounterRate)
                    {
                        if (first.Obsolete_CounterRate != 0m) e.Obsolete_StartingExchangeRate = 1m / first.Obsolete_CounterRate;
                    }
                    else
                    {
                        e.Obsolete_StartingExchangeRate = first.Obsolete_BaseRate;
                    }
                    list.Add(e);
                }
            }
            return list;
        }
    }
}
