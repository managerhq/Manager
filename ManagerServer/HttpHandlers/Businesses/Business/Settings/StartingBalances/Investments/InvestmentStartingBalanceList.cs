using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.Investments
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Investments))]
    [Guid("c5c8b519-ecdb-4787-960d-13d29c89b33a")]
    [Title(nameof(Strings.StartingBalances), nameof(Strings.Investments))]
    [Guide("This screen allows you to set up starting balances for investments you have created under the **Investments** tab.")]
    [Guide("To find this screen, go to the **Settings** tab, then click **Starting Balances**, then click **Investments**.")]
    [SettingsItemScreenshot("fa-wand-magic-sparkles", nameof(Strings.StartingBalances), "fa-chart-pie", nameof(Strings.Investments))]
    [Guide("To create a new starting balance for an investment, click the **New Starting Balance** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.Investments), name: nameof(Strings.NewStartingBalance))]
    [Guide("You will be taken to the *Starting Balance* form for *Investments*.")]
    [LinkGuide("For more information, see:", typeof(InvestmentStartingBalanceForm))]
    internal sealed class InvestmentStartingBalanceList : NakedObjectsWithAutomaticRows<InvestmentStartingBalance>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewStartingBalance);
        }

        [Default]
        [Guid("c9a70f53-c11e-4549-8c28-27feef07cb1d")]
        public NamedObject[] GetInvestment(InvestmentStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<Investment>(x.Investment)).ToArray();
        }

        [Default, Right]
        [Guid("719490b2-b3a4-4875-960f-c478e8f99bd1")]
        public decimal[] GetQtyOwned(InvestmentStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();

            return rows.Select(x => x.StartingBalance).ToArray();
        }

        [Default, Right]
        [Guid("8cf3102f-379e-4d8a-a622-897560c18973")]
        public Tuple<decimal, Currency>[] GetMarketPrice(InvestmentStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();

            return rows.Select(x => new Tuple<decimal, Currency>(x.MarketPrice, baseCurrency)).ToArray();
        }

        [Default, Right, Sum, Bold]
        [Guid("19048ba7-63f6-4dcc-bb19-3044419235be")]
        public Tuple<decimal, Currency>[] GetMarketValue(InvestmentStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();

            return rows.Select(x => new Tuple<decimal, Currency>(baseCurrency.Round(x.StartingBalance * x.MarketPrice), baseCurrency)).ToArray();
        }
    }
}