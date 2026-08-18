using System.Linq;
using System.Collections.Generic;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Currencies.ExchangeRates
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("9606e19e-7d7b-4ce6-b67b-035ceb340fa9")]
    [Title(nameof(Strings.ExchangeRates))]
    [Guide("The **Exchange Rates** screen allows you to create and manage the list of exchange rates for your foreign currencies.")]
    [Guide("To access the **Exchange Rates** screen, go to the **Settings** tab, then click **Currencies**.")]
    [SettingsItemScreenshot(icon: "fa-coin", name: nameof(Strings.Currencies))]
    [Guide("Within the **Currencies** screen, click **Exchange Rates**.")]
    [Guide("To create a new exchange rate, click the **New Exchange Rate** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.ExchangeRates), name: nameof(Strings.NewExchangeRate))]
    internal sealed class ExchangeRates : NakedObjectsWithAutomaticRows<ManagerServer.Model.ExchangeRate>
    {
        [ProtoMember(1)] public Guid? ForeignCurrency;
        [ProtoMember(2)] public DateTime? Date;

        protected override ExchangeRate[] OnGetRows(ExchangeRate[] rows)
        {
            if (ForeignCurrency.HasValue) rows = rows.Where(x => x.Currency == ForeignCurrency.Value).ToArray();
            if (Date.HasValue) rows = rows.Where(x => x.Date <= Date.Value).ToArray();
            return base.OnGetRows(rows);
        }

        [Default]
        [MinWidth, Center, WarnIfFutureDate]
        [WhitespaceNoWrap]
        [Guid("1919138b-e501-491b-9821-5e237cb7b748")]
        public DateTime[] GetDate(ExchangeRate[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("30119bff-621f-47c3-a118-6ccb8acdcba8")]
        public string[] GetCurrency(ExchangeRate[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ForeignCurrency>(x.Currency)?.GetCodeAndName()).ToArray();
        }

        [Default]
        [Guid("1fece75d-2b25-4507-be6e-255528ac54d2")]
        public string[] GetRate(ExchangeRate[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();

            return rows.Select(x => ToString(GetRate(database, baseCurrency, x))).ToArray();
        }

        private Tuple<decimal, Currency, decimal, Currency> GetRate(ManagerServer.Database database, ManagerServer.Model.BaseCurrency baseCurrency, ExchangeRate exchangeRate)
        {
            if (exchangeRate.ExchangeRateValue == 0m) return null;

            var foreignCurrency = database.SingleOrDefault<ForeignCurrency>(exchangeRate.Currency);

            if (foreignCurrency == null) return null;
            else if (exchangeRate.ExchangeRateIsInverse) return new Tuple<decimal, Currency, decimal, Currency>(1m, baseCurrency, exchangeRate.ExchangeRateValue, foreignCurrency);
            else return new Tuple<decimal, Currency, decimal, Currency>(1m, foreignCurrency, exchangeRate.ExchangeRateValue, baseCurrency);
        }

        private string ToString(Tuple<decimal, Currency, decimal, Currency> rate)
        {
            if (rate == null) return null;
            return $"{rate.Item1.ToNumberString()} {rate.Item2.GetCode()} = {rate.Item3.ToNumberString()} {rate.Item4.GetCode()}";
        }

        [Default]
        [Center, MinWidth]
        [Guid("8285c525-bac8-4d66-993a-1a04b910aa50")]
        public Tuple<int, BusinessTemplate>[] GetTransactions(ExchangeRate[] rows)
        {
            var referrer = this.ToUrl();
            var database = ApplicationData.Businesses.Get(Business);
            var exchangeRateDates = database.OfType<ExchangeRate>().Where(x => x.Currency.HasValue && x.ExchangeRateValue > 0m).GroupBy(x => x.Currency.Value).ToDictionary(x => x.Key, x => x.Select(x => x.Date).OrderBy(x => x.Date).ToArray());
            var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();

            var currencies = database.OfType<ForeignCurrency>().ToDictionary(x => x.Key, x => new HashSet<Guid>() { x.Key });
            foreach (var e in database.OfType<BankOrCashAccount>().Where(x => x.Currency.HasValue && currencies.ContainsKey(x.Currency.Value))) currencies[e.Currency.Value].Add(e.Key);
            foreach (var e in database.OfType<Customer>().Where(x => x.Currency.HasValue && currencies.ContainsKey(x.Currency.Value))) currencies[e.Currency.Value].Add(e.Key);
            foreach (var e in database.OfType<Supplier>().Where(x => x.Currency.HasValue && currencies.ContainsKey(x.Currency.Value))) currencies[e.Currency.Value].Add(e.Key);
            foreach (var e in database.OfType<Employee>().Where(x => x.Currency.HasValue && currencies.ContainsKey(x.Currency.Value))) currencies[e.Currency.Value].Add(e.Key);

            var output = new List<Tuple<int, BusinessTemplate>>();

            foreach (var e in rows)
            {
                if (e.ExchangeRateValue > 0m)
                {
                    var foreignCurrency = database.SingleOrDefault<ForeignCurrency>(e.Currency);
                    if (foreignCurrency != null)
                    {
                        var fromDate = DateTime.MinValue;
                        if (exchangeRateDates[foreignCurrency.Key].Any(x => x.Date < e.Date)) fromDate = e.Date;
                        var toDate = exchangeRateDates[foreignCurrency.Key].FirstOrDefault(x => x > e.Date);

                        var count = database.UnorderedOfType<Transaction>()
                            .OfType<IForeignCurrencyTransaction>()
                            .Where(x => x.Currency.HasValue)
                            .Where(x => x.Date >= fromDate)
                            .Where(x => toDate == default || x.Date < toDate)
                            .Where(x => currencies[foreignCurrency.Key].Contains(x.Currency.Value))
                            .Where(x => x is not InterAccountTransfer interAccountTransfer || interAccountTransfer.GetGeneralLedgerTransactions(database)[0].TransactionCurrency is ManagerServer.Model.ForeignCurrency)
                            .Count();
                        output.Add(new Tuple<int, BusinessTemplate>(count, new ExchangeRateTransactions() { Business = Business, ExchangeRate = e.Key, Referrer = referrer }));
                    }
                    else
                    {
                        output.Add(null);
                    }
                }
                else
                {
                    output.Add(null);
                }
            }

            return output.ToArray();
        }
    }
}
