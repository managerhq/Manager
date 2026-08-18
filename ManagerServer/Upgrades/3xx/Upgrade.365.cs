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
        private static async Task<IEnumerable<Model.Object>> Upgrade365(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var foreignCurrencyEmployees = objects.OfType<Employee>().Where(x => x.Currency.HasValue).ToDictionary(x => x.Key, x => x.Currency.Value);
            foreach (var e in objects.OfType<Payslip>().Where(x => x.employee.HasValue && foreignCurrencyEmployees.ContainsKey(x.employee.Value)).ToArray())
            {
                var currency = foreignCurrencyEmployees[e.employee.Value];
                var exchangeRate = objects.OfType<ExchangeRate>().Where(x => x.Currency == currency && x.Date <= e.Date && (x.Obsolete_BaseRate > 0m || x.Obsolete_CounterRate > 0m)).OrderByDescending(x => x.Date).FirstOrDefault();
                if (exchangeRate != null)
                {
                    if (exchangeRate.Obsolete_Type == ExchangeRateType.BaseRate)
                    {
                        e.ExchangeRate = exchangeRate.Obsolete_BaseRate;
                        e.ExchangeRateIsInverse = true;
                    }
                    else
                    {
                        e.ExchangeRate = exchangeRate.Obsolete_CounterRate;
                        e.ExchangeRateIsInverse = false;
                    }
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
            return list;
        }
    }
}
