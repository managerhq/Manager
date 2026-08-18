using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.FixedAssets
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(FixedAssets))]
    [Guid("a11e5804-14c5-4827-ad02-afbbfb62feeb")]
    [Title(nameof(Strings.StartingBalances), nameof(Strings.FixedAssets))]
    [Guide("This screen allows you to set up starting balances for fixed assets you have created under the **Fixed Assets** tab.")]
    [Guide("To create a new starting balance for a fixed asset, click the **New Starting Balance** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.FixedAssets), name: nameof(Strings.NewStartingBalance))]
    [Guide("You will be taken to the *Starting Balance* screen for the selected *fixed asset*.")]
    [LinkGuide("For more information, see:", typeof(FixedAssetStartingBalanceForm))]
    internal sealed class FixedAssetStartingBalanceList : NakedObjectsWithAutomaticRows<FixedAssetStartingBalance>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewStartingBalance);
        }

        [Default]
        [Guid("6d8b3ea9-9bb2-4a1b-9534-4eb0acace09e")]
        public NamedObject[] GetFixedAsset(FixedAssetStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<FixedAsset>(x.FixedAsset)).ToArray();
        }

        [Default, Right, Sum]
        [Guid("da02c432-c167-4930-a677-b61ab72d6fdf")]
        public Tuple<decimal, Currency>[] GetAcquisitionCost(FixedAssetStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();

            return rows.Select(x => new Tuple<decimal, Currency>(baseCurrency.Round(x.StartingBalance), baseCurrency)).ToArray();
        }

        [Default, Right, Sum]
        [Guid("ed37ab1e-bda6-434f-9034-0afa529e73b5")]
        public Tuple<decimal, Currency>[] GetAccumulatedDepreciation(FixedAssetStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();

            return rows.Select(x => new Tuple<decimal, Currency>(baseCurrency.Round(x.StartingBalanceAccumulatedDepreciation), baseCurrency)).ToArray();
        }

        [Default, Right, Sum, Bold]
        [Guid("d1481b67-79a2-4f91-8e32-c670bbf77139")]
        public Tuple<decimal, Currency>[] GetBookValue(FixedAssetStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();

            return rows.Select(x => new Tuple<decimal, Currency>(baseCurrency.Round(x.StartingBalance) - baseCurrency.Round(x.StartingBalanceAccumulatedDepreciation), baseCurrency)).ToArray();
        }
    }
}