using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.IntangibleAssets
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(IntangibleAssets))]
    [Guid("669d4fa1-c485-4967-85d5-f77b41232fc0")]
    [Title(nameof(Strings.StartingBalances), nameof(Strings.IntangibleAssets))]
    [Guide("This screen allows you to set up starting balances for intangible assets that you have created under the **Intangible Assets** tab.")]
    [Guide("Starting balances represent the initial values of your intangible assets at the time you begin using this accounting software.")]
    [Guide("To create a new starting balance for an intangible asset, click the **New Starting Balance** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.IntangibleAssets), name: nameof(Strings.NewStartingBalance))]
    [Guide("You will be taken to the *Starting Balance* screen where you can enter details for your *intangible asset*.")]
    [LinkGuide("For more information, see:", typeof(IntangibleAssetStartingBalanceForm))]
    internal sealed class IntangibleAssetStartingBalanceList : NakedObjectsWithAutomaticRows<IntangibleAssetStartingBalance>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewStartingBalance);
        }

        [Default]
        [Guid("737479d5-98ca-4ddc-9727-7e6a40405237")]
        public NamedObject[] GetIntangibleAsset(IntangibleAssetStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<IntangibleAsset>(x.IntangibleAsset)).ToArray();
        }

        [Default, Right, Sum]
        [Guid("37909c81-8297-46b5-a853-9c3df349287e")]
        public Tuple<decimal, Currency>[] GetAcquisitionCost(IntangibleAssetStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();

            return rows.Select(x => new Tuple<decimal, Currency>(baseCurrency.Round(x.StartingBalance), baseCurrency)).ToArray();
        }

        [Default, Right, Sum]
        [Guid("5379f8ea-0f2a-4cc1-90c9-e4182aaae185")]
        public Tuple<decimal, Currency>[] GetAccumulatedAmortization(IntangibleAssetStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();

            return rows.Select(x => new Tuple<decimal, Currency>(baseCurrency.Round(x.StartingBalanceAccumulatedAmortization), baseCurrency)).ToArray();
        }

        [Default, Right, Sum, Bold]
        [Guid("786fae45-e69b-47b3-af7c-5339cacfa280")]
        public Tuple<decimal, Currency>[] GetBookValue(IntangibleAssetStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();

            return rows.Select(x => new Tuple<decimal, Currency>(baseCurrency.Round(x.StartingBalance) - baseCurrency.Round(x.StartingBalanceAccumulatedAmortization), baseCurrency)).ToArray();
        }
    }
}