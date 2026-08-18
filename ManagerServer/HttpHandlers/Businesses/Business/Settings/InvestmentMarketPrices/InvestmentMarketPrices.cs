using System.Linq;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.InvestmentMarketPrices
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("67220fbb-e911-4d22-8983-436672d10232")]
    [Title(nameof(Strings.InvestmentMarketPrices))]
    [Guide("*Investment market prices* are used to enter up-to-date market prices for your *investments*.")]
    [Guide("To access investment market prices, go to the **Settings** tab, then click **Investment Market Prices**.")]
    [SettingsItemScreenshot(icon: "fa-chart-waterfall", name: nameof(Strings.InvestmentMarketPrices))]
    [Guide("To create a new investment market price, click the **New Investment Market Price** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.InvestmentMarketPrices), name: nameof(Strings.NewInvestmentMarketPrice))]
    internal sealed class InvestmentMarketPrices : NakedObjectsWithAutomaticRows<InvestmentMarketPrice>
    {
        [ProtoMember(1)] public Guid? Investment;

        protected override InvestmentMarketPrice[] OnGetRows(InvestmentMarketPrice[] rows)
        {
            if (Investment.HasValue) rows = rows.Where(x => x.Investment == Investment.Value).ToArray();
            return rows;
        }

        [Default]
        [WarnIfFutureDate]
        [Center, MinWidth]
        [WhitespaceNoWrap]
        [Guid("846f2925-a610-45c7-9497-0406dd1778de")]
        public DateTime[] GetDate(ManagerServer.Model.InvestmentMarketPrice[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        [Guid("4b399462-a68e-4fc0-8a3b-c6ca20941960")]
        public string[] GetInvestment(ManagerServer.Model.InvestmentMarketPrice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Investment>(x.Investment)?.GetCodeAndName()).ToArray();
        }

        [Bold]
        [Right]
        [Default]
        [Guid("93fe49fd-4fb8-4006-8399-4de650a74017")]
        public Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>>[] GetMarketPrice(ManagerServer.Model.InvestmentMarketPrice[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            var foreignCurrencies = database.OfType<ForeignCurrency>().ToDictionary(x => x.Key);

            var output = new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>>[rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                var price1 = new Tuple<decimal, Currency>(rows[i].MarketPrice, database.SingleOrDefault<ForeignCurrency>(rows[i].Currency) as Currency ?? baseCurrency);
                var price2 = new Tuple<decimal, Currency>(rows[i].GetMarketPriceInBaseCurrency(baseCurrency) ?? 0m, baseCurrency);

                if (price1.Item2 == price2.Item2)
                {
                    output[i] = new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>>(price1, null);
                }
                else
                {
                    output[i] = new Tuple<Tuple<decimal, Currency>, Tuple<decimal, Currency>>(price1, price2);
                }
            }

            return output;
        }
    }
}