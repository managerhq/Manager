using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Query
{
    public static class Currencies
    {
        public static CurrencyProvider GetCurrencyProvider(string fileId)
        {
            var baseCurrency = GetBaseCurrency(fileId);
            var foreignCurrencies = ApplicationData.Instance.Businesses.Get(fileId).OfType<ManagerServer.Model.ForeignCurrency>().ToArray();
            return new CurrencyProvider(baseCurrency, foreignCurrencies);
        }

        private static ManagerServer.Model.Currency GetBaseCurrency(string fileId)
        {
            return ApplicationData.Instance.Businesses.Get(fileId).Single<ManagerServer.Model.BaseCurrency>();
        }
    }

    public sealed class CurrencyProvider
    {
        private ManagerServer.Model.Currency baseCurrency;
        private Dictionary<Guid, ManagerServer.Model.Currency> foreignCurrencies;

        public CurrencyProvider(ManagerServer.Model.Currency baseCurrency, ManagerServer.Model.Currency[] foreignCurrencies)
        {
            this.baseCurrency = baseCurrency;
            this.foreignCurrencies = foreignCurrencies.ToDictionary(x => x.Key);
        }

        public ManagerServer.Model.Currency Get(Guid? currency)
        {
            if (currency.HasValue && foreignCurrencies.ContainsKey(currency.Value)) return foreignCurrencies[currency.Value];
            return baseCurrency;
        }
    }
}
