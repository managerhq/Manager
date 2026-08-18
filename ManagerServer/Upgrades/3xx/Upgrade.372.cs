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
        private static async Task<IEnumerable<Model.Object>> Upgrade372(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var foreignCurrencyCustomers = objects.OfType<Customer>().Where(x => x.Currency.HasValue).ToDictionary(x => x.Key, x => x.Currency.Value);
            foreach (var e in objects.OfType<SalesInvoice>().Where(x => x.Customer.HasValue && foreignCurrencyCustomers.ContainsKey(x.Customer.Value)).ToArray())
            {
                var currency = foreignCurrencyCustomers[e.Customer.Value];
                var exchangeRate = objects.OfType<ManagerServer.Model.ExchangeRate>().Where(x => x.Currency == currency && x.Date <= e.IssueDate && x.ExchangeRateValue > 0m).OrderByDescending(x => x.Date).FirstOrDefault();
                if (exchangeRate != null)
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
            foreach (var e in objects.OfType<CreditNote>().Where(x => x.Customer.HasValue && foreignCurrencyCustomers.ContainsKey(x.Customer.Value)).ToArray())
            {
                var currency = foreignCurrencyCustomers[e.Customer.Value];
                var exchangeRate = objects.OfType<ManagerServer.Model.ExchangeRate>().Where(x => x.Currency == currency && x.Date <= e.IssueDate && x.ExchangeRateValue > 0m).OrderByDescending(x => x.Date).FirstOrDefault();
                if (exchangeRate != null)
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
            return list;
        }
    }
}
