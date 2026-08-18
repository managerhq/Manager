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
        private static async Task<IEnumerable<Model.Object>> Upgrade361(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.BankOrCashAccount>().Where(x => x.Obsolete_StartingBalance2 != 0m))
            {
                var foreignCurrency = objects.SingleOrDefault<ForeignCurrency>(e.Currency);
                if (foreignCurrency != null)
                {
                    e.Obsolete_ExchangeRate2 = foreignCurrency.Obsolete_StartingExchangeRate > 0m ? foreignCurrency.Obsolete_StartingExchangeRate : 1m;
                    e.Obsolete_ExchangeRateIsInverse2 = true;
                    list.Add(e);
                }
            }
            return list;
        }
    }
}
