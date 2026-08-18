using System.Linq;
using System.Collections.Generic;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.FixedAssets
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("a85c0540-32ba-43e5-9486-bc8884e95e5a")]
    [Title(nameof(Strings.FixedAssets))]
    [Guide("The `Fixed Assets` tab helps you track and manage long-term physical assets your business owns and uses in operations.")]
    [Guide("Fixed assets are valuable items that last more than one year, such as buildings, vehicles, equipment, machinery, furniture, and computers.")]
    [Guide("Unlike inventory items you sell, fixed assets are used to run your business and generate revenue over multiple years.")]
    [Guide("From this tab, you can monitor acquisition costs, track depreciation, calculate book values, and manage asset disposals.")]
    [TabScreenshot(icon: "fa-car-building", name: nameof(Strings.FixedAssets))]
    [Guide("The system tracks each asset's acquisition cost, accumulated depreciation, and current book value automatically.")]
    [Header("Creating and Recording Fixed Assets")]
    [Guide("To create a new fixed asset, click the `New Fixed Asset` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.FixedAssets), name: nameof(Strings.NewFixedAsset))]
    [Guide("When you create a new fixed asset, its acquisition cost will initially be zero since no transactions have been allocated to it yet.")]
    [Guide("To set up the acquisition cost, you must create a transaction that represents the purchase of this fixed asset.")]
    [Guide("For instance, if you bought a fixed asset with cash, navigate to the `Payments` tab and click the `New Payment` button.")]
    [Guide("To record your payment, allocate it to the `Fixed assets - At cost` account and then select the specific fixed asset.")]
    [SelectAccountScreenshot(accountName: nameof(Strings.Fixed_assets_at_cost), prepend: nameof(Strings.FixedAsset))]
    [Guide("If you bought this fixed asset on credit from a supplier (through a purchase invoice), navigate to the `Purchase Invoices` tab and click the `New Purchase Invoice` button.")]
    [Guide("Then categorize it in the same manner as you would a payment.")]
    [Header("Disposing of Fixed Assets")]
    [Guide("Every fixed asset will eventually be disposed of by either being sold or written off.")]
    [Guide("When a fixed asset is sold, allocate the sale transaction to the `Fixed assets - At cost` account just like when the fixed asset was originally purchased.")]
    [Guide("The second step is to mark the fixed asset as `Disposed`.")]
    [Guide("To mark a fixed asset as `Disposed`, click the `Edit` button on the fixed asset and check the `Disposed fixed asset` checkbox.")]
    [Guide("Then enter the `Date of disposal`.")]
    [Guide("This will make the system create an automatic transaction that sets the fixed asset book value to zero.")]
    [Guide("The difference is posted to the `Fixed assets - Loss on disposal` account on your `Profit and Loss Statement`.")]
    [Header("Understanding the Display")]
    [Guide("The `Fixed Assets` tab includes several columns:")]
    [Columns]
    internal sealed class FixedAssets : NakedObjectsWithAutomaticRows<ManagerServer.Model.FixedAsset>
    {
        [WarnIfNotUnique]
        [Guid("0ccc70f0-6dee-4797-97fc-04ba1a7ad544")]
        [Guide("A unique code or reference number to identify this fixed asset.")]
        [Guide("Asset codes help with physical asset tracking, inventory counts, and maintenance schedules.")]
        [Guide("Common formats include department prefixes (IT-001) or asset type codes (VEH-2023-01).")]
        public string[] GetCode(ManagerServer.Model.FixedAsset[] rows)
        {
            return rows.Select(x => x.ItemCode).ToArray();
        }

        [Default]
        [Guid("7e1732b2-33de-40a4-83d9-1b561f72ec7a")]
        [Guide("The descriptive name of this fixed asset.")]
        [Guide("Use clear names that help identify the specific asset, like 'Dell Laptop - Marketing' or '2023 Toyota Forklift'.")]
        [Guide("Good naming helps when selecting assets in transactions and generating reports.")]
        public string[] GetName(ManagerServer.Model.FixedAsset[] rows)
        {
            return rows.Select(x => x.ItemName).ToArray();
        }

        [Default]
        [Guid("5eb8b083-7301-423b-8286-489624dbf56d")]
        [Guide("Additional details about the fixed asset such as serial numbers, specifications, or location.")]
        [Guide("Include information that helps identify and track the physical asset.")]
        [Guide("This field is useful for warranty information, maintenance notes, or technical specifications.")]
        public string[] GetDescription(ManagerServer.Model.FixedAsset[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Center]
        [Guid("2ce213ab-08c9-4192-9521-6833a68904ed")]
        [Guide("The annual depreciation rate as a percentage of the asset's cost or book value.")]
        [Guide("This rate determines how quickly the asset loses value for accounting purposes.")]
        [Guide("Common rates: Buildings (2-5%), Vehicles (15-25%), Computers (20-33%), Furniture (10-20%).")]
        public decimal[] GetDepreciationRate(ManagerServer.Model.FixedAsset[] rows)
        {
            return rows.Select(x => x.DepreciationRate).ToArray();
        }

        [Guid("1f85c294-63f9-4602-90b9-9f235964e79a")]
        [Guide("Shows which control account groups this asset on your `Balance Sheet`.")]
        [Guide("By default, all fixed assets appear under a single `Fixed assets - At cost` account.")]
        [Guide("Create custom control accounts to separate asset types like `Vehicles`, `Equipment`, or `Buildings` on financial statements.")]
        public string[] GetControlAccount(ManagerServer.Model.FixedAsset[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => (database.SingleOrDefault<ManagerServer.Model.ControlAccountForFixedAssets>(x.ControlAccountForFixedAssets) as ManagerServer.Model.NamedObject ?? database.Single<ManagerServer.Model.BalanceSheetFixedAssetsAtCostAccount>()).GetName()).ToArray();
        }

        [Guid("6abe5f46-064b-4042-9e2b-f9be37565491")]
        [Guide("Indicates which division or department owns or uses this fixed asset.")]
        [Guide("Assigning assets to divisions helps track costs and generate divisional reports.")]
        [Guide("This column only appears when the `Divisions` feature is activated in your business.")]
        public string[] GetDivision(ManagerServer.Model.FixedAsset[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Division>(x.Division)?.GetCodeAndName()).ToArray();
        }

        /*
        private Dictionary<Manager.Model.FixedAsset, Balance> getBalances = null;
        public Dictionary<Manager.Model.FixedAsset, Balance> GetBalances(Manager.Model.FixedAsset[] rows)
        {
            if (getBalances == null)
            {
                var referrer = this.ToUrl();
                var baseCurrency = Manager.ApplicationData.Businesses.Get(FileID).Single<Manager.Model.BaseCurrency>();
                var generalLedger = new Manager.Query.GeneralLedger.GeneralLedger(FileID).Where(x => x.GeneralLedgerAccount.IsControlAccountForFixedAssets || x.GeneralLedgerAccount.IsControlAccountForFixedAssetsAccumulatedDepreciation).GroupBy(x => x.FixedAsset).ToDictionary(x => x.Key.Key, x => x.ToArray());
                var balances = new Dictionary<Manager.Model.FixedAsset, Balance>();
                foreach (var e in rows)
                {
                    var acquisitionCost = 0m;
                    var depreciation = 0m;
                    var bookValue = 0m;
                    if (generalLedger.TryGetValue(e.Key, out Manager.Query.GeneralLedger.GeneralLedgerTransaction[] transactions))
                    {
                        acquisitionCost = transactions.Where(x => x.GeneralLedgerAccount.IsControlAccountForFixedAssets).Sum(x => x.BaseAmount);
                        depreciation = transactions.Where(x => x.GeneralLedgerAccount.IsControlAccountForFixedAssetsAccumulatedDepreciation).Sum(x => x.BaseAmount) * -1m;
                        bookValue = acquisitionCost - depreciation;
                    }

                    if (e.DisposedFixedAsset && e.DisposalDate.HasValue)
                    {
                        bookValue = 0m;
                    }

                    balances.Add(e, new Balance()
                    {
                        AcquisitionCost = new Tuple<decimal, Manager.Model.Currency, BusinessTemplate>(acquisitionCost, baseCurrency, new FixedAssetTransactions() { FixedAsset = e.Key, FileID = FileID, Referrer = referrer }),
                        Depreciation = new Tuple<decimal, Manager.Model.Currency, BusinessTemplate>(depreciation, baseCurrency, new FixedAssetAccumulatedDepreciationTransactions() { FixedAsset = e.Key, FileID = FileID, Referrer = referrer }),
                        BookValue = new Tuple<decimal, Manager.Model.Currency>(bookValue, baseCurrency)
                    });
                }
                getBalances = balances;
            }
            return getBalances;
        }
        */

        [Default]
        [Right, Sum]
        [Guid("6b31326c-3f3f-4e7f-b9ce-abe70f6f378b")]
        [Guide("The total amount paid to acquire this fixed asset, including purchase price and related costs.")]
        [Guide("Acquisition cost includes the purchase price, delivery charges, installation fees, and any costs to make the asset operational.")]
        [Guide("Click the amount to see all transactions that contributed to this asset's cost.")]
        public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>[] GetAcquisitionCost(ManagerServer.Model.FixedAsset[] rows)
        {
            var referrer = this.ToUrl();
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BaseCurrency>();
            var generalLedger = new GeneralLedger(Business).DisposeFixedAssets();

            return [.. rows.Select(x => new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(generalLedger.GetAggregations().GetFixedAssetAmount(x.Key, DateTime.MinValue, DateTime.MaxValue), baseCurrency, new FixedAssetTransactions() { FixedAsset = x.Key, Business = Business, Referrer = referrer }))];
        }

        [Right, Sum]
        [Guid("460dc101-96f8-4d98-a8f1-1c54ce54ad1c"), Default]
        [Guide("The total depreciation expense recorded for this asset since acquisition.")]
        [Guide("Accumulated depreciation reduces the asset's book value and represents the portion of the cost allocated to expense over time.")]
        [Guide("Click the amount to see all depreciation entries posted for this asset.")]
        public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>[] GetDepreciation(ManagerServer.Model.FixedAsset[] rows)
        {
            var referrer = this.ToUrl();
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BaseCurrency>();
            var generalLedger = new GeneralLedger(Business).DisposeFixedAssets();

            return [.. rows.Select(x => new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(-generalLedger.GetAggregations().GetDepreciationAmount(x.Key, DateTime.MinValue, DateTime.MaxValue), baseCurrency, new FixedAssetAccumulatedDepreciationTransactions() { FixedAsset = x.Key, Business = Business, Referrer = referrer }))];
        }

        [Right, Sum]
        [Guid("b68a8c24-0466-4fae-80bd-70fc7388c41b"), Default, Bold]
        [Guide("The current accounting value of the fixed asset after depreciation.")]
        [Guide("Book value equals acquisition cost minus accumulated depreciation.")]
        [Guide("This represents the remaining value to be depreciated in future periods or recovered upon disposal.")]
        public Tuple<decimal, ManagerServer.Model.Currency>[] GetBookValue(ManagerServer.Model.FixedAsset[] rows)
        {
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BaseCurrency>();
            var acquisitionCosts = GetAcquisitionCost(rows).Select(x => x.Item1);
            var depreciationAmounts = GetDepreciation(rows).Select(x => x.Item1);

            var bookValue = acquisitionCosts.Zip(depreciationAmounts, (x, y) => x - y).ToArray();

            return [.. bookValue.Select(x => new Tuple<decimal, Currency>(x, baseCurrency))];
        }

        [Default]
        [MinWidth, Center, WhitespaceNoWrap]
        [Guid("d03f45ec-9f2d-4f38-8644-115328f1e12a")]
        [Guide("Indicates whether the asset is currently in use or has been disposed of.")]
        [Guide("`Active` assets are still owned and used by the business.")]
        [Guide("`Disposed` assets have been sold, scrapped, or otherwise removed from service.")]
        public Status[] GetStatus(ManagerServer.Model.FixedAsset[] rows)
        {
            return rows.Select(x => x.DisposedFixedAsset && x.DisposalDate.HasValue).Select(x => x ? Status.Disposed : Status.Active).ToArray();
        }

        public enum Status
        {
            [ManagerServer.Model.Attributes.Success] Active,
            Disposed
        }
    }
}
