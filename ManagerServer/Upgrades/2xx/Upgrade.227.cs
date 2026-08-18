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
        private static async Task<IEnumerable<Model.Object>> Upgrade227(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var currencies = ManagerServer.Model.Obsolete.Obsolete54.CurrencyKeys.All.ToDictionary(x => x.Key);
            var list = new List<Model.Object>();
            var baseCurrency = objects.OfType<ManagerServer.Model.BaseCurrency>().SingleOrDefault(x => x.Key == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.BaseCurrency)));
            if (baseCurrency != null && baseCurrency.Obsolete_Currency.HasValue && currencies.ContainsKey(baseCurrency.Obsolete_Currency.Value))
            {
                var currency = currencies[baseCurrency.Obsolete_Currency.Value];
                baseCurrency.Code = currency.Code;
                baseCurrency.Name = currency.Name;
                baseCurrency.Prefix = currency.Prefix;
                baseCurrency.DecimalPlaces = currency.DecimalPlaces == 2 ? null : (int?)currency.DecimalPlaces;
                list.Add(baseCurrency);

                var activeCurrencies = new List<Guid>();
                foreach (var e in objects.OfType<ManagerServer.Model.Customer>().Where(x => x.Currency.HasValue).ToArray()) activeCurrencies.Add(e.Currency.Value);
                foreach (var e in objects.OfType<ManagerServer.Model.Supplier>().Where(x => x.Currency.HasValue).ToArray()) activeCurrencies.Add(e.Currency.Value);
                foreach (var e in objects.OfType<ManagerServer.Model.Employee>().Where(x => x.Currency.HasValue).ToArray()) activeCurrencies.Add(e.Currency.Value);
                foreach (var e in objects.OfType<ManagerServer.Model.BankOrCashAccount>().Where(x => x.Currency.HasValue).ToArray()) activeCurrencies.Add(e.Currency.Value);
                foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete78.CashAccount>().Where(x => x.Currency.HasValue).ToArray()) activeCurrencies.Add(e.Currency.Value);
                foreach (var e in objects.OfType<ManagerServer.Model.JournalEntry>().Where(x => x.Currency.HasValue).ToArray()) activeCurrencies.Add(e.Currency.Value);
                foreach (var e in objects.OfType<ManagerServer.Model.ExpenseClaim>().Where(x => x.Currency.HasValue).ToArray()) activeCurrencies.Add(e.Currency.Value);
                foreach (var e in objects.OfType<ManagerServer.Model.SpecialAccount>().Where(x => x.Currency.HasValue).ToArray()) activeCurrencies.Add(e.Currency.Value);

                var activeForeignCurrencies = activeCurrencies.Distinct().Where(x => x != baseCurrency.Obsolete_Currency.Value).ToArray();
                foreach (var e in activeForeignCurrencies)
                {
                    if (currencies.ContainsKey(e))
                    {
                        var currency2 = currencies[e];
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
