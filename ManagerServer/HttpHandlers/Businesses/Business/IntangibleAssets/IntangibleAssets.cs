using System.Linq;
using System.Collections.Generic;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.IntangibleAssets
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.IntangibleAssets))]
    [Guid("459f8b4f-be2a-4b7d-bf14-530480271922")]
    [Guide("The **Intangible Assets** tab helps manage and track your business's non-physical assets, such as intellectual property rights, patents, licenses, or goodwill.")]
    [Guide("You can enter detailed information for each intangible asset, monitor its amortization, and track its book value as it changes over time.")]
    [TabScreenshot("fa-wind", nameof(Strings.IntangibleAssets))]
    [Header("Getting Started")]
    [Guide("To create a new intangible asset, click the **New Intangible Asset** button.")]
    [HeroButtonScreenshot(nameof(Strings.IntangibleAssets), nameof(Strings.NewIntangibleAsset))]
    [Guide("When you create a new intangible asset, its acquisition cost will initially be zero. This is because no transaction has been linked to this intangible asset yet.")]
    [Header("Recording Acquisition Cost")]
    [Guide("To set an acquisition cost, you need to record a transaction that represents the purchase of the intangible asset.")]
    [Guide("If you purchased an intangible asset with cash, navigate to the **Payments** tab and click **New Payment**. Record the payment by allocating it to the *Intangible assets at cost* account and select the specific intangible asset.")]
    [Guide("If you purchased the intangible asset on credit from a supplier, navigate to the **Purchase Invoices** tab and click **New Purchase Invoice**. Allocate the purchase invoice in the same manner as you would allocate a payment.")]
    [Header("Understanding the Columns")]
    [Guide("The **Intangible Assets** tab displays the following columns:")]
    [Columns]
    internal sealed class IntangibleAssets : NakedObjectsWithAutomaticRows<ManagerServer.Model.IntangibleAsset>
    {
        [WarnIfNotUnique]
        [Guid("0ccc70f0-6dee-4797-97fc-04ba1a7ad544")]
        [Guide("A unique identifier code for the intangible asset. This optional field helps you track assets using your own coding system.")]
        public string[] GetCode(ManagerServer.Model.IntangibleAsset[] rows)
        {
            return rows.Select(x => x.ItemCode).ToArray();
        }

        [Default]
        [Guid("588319a1-4ddb-4a18-ae23-8dd903fb0530")]
        [Guide("The name of the intangible asset.")]
        public string[] GetName(ManagerServer.Model.IntangibleAsset[] rows)
        {
            return rows.Select(x => x.ItemName).ToArray();
        }

        [Default]
        [Guid("ea12926e-cf6e-4cb3-afc5-bb643a6f589f")]
        [Guide("A detailed description of the intangible asset.")]
        public string[] GetDescription(ManagerServer.Model.IntangibleAsset[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Center]
        [Guid("46305d59-0000-4ed3-994a-6e617a34dd1c")]
        [Guide("The annual amortization rate as a percentage.")]
        public decimal[] GetAmortizationRate(ManagerServer.Model.IntangibleAsset[] rows)
        {
            return rows.Select(x => x.AmortizationRate).ToArray();
        }

        [Guid("a0d3639d-c994-4970-8162-b63477511f06")]
        [Guide("The control account associated with this intangible asset. If you haven't set up custom control accounts, it will default to *Intangible assets at cost*.")]
        public string[] GetControlAccount(ManagerServer.Model.IntangibleAsset[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => (database.SingleOrDefault<ManagerServer.Model.ControlAccountForIntangibleAssets>(x.ControlAccountForIntangibleAssets) as ManagerServer.Model.NamedObject ?? database.Single<ManagerServer.Model.BalanceSheetIntangibleAssetsAtCostAccount>()).GetName()).ToArray();
        }

        private Dictionary<ManagerServer.Model.IntangibleAsset, Balance> getBalances = null;
        public Dictionary<ManagerServer.Model.IntangibleAsset, Balance> GetBalances(ManagerServer.Model.IntangibleAsset[] rows)
        {
            if (getBalances == null)
            {
                var referrer = this.ToUrl();
                var baseCurrency = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BaseCurrency>();
                var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsControlAccountForIntangibleAssets || x.GeneralLedgerAccount.IsControlAccountForIntangibleAssetsAccumulatedAmortization).GroupBy(x => x.IntangibleAsset).ToDictionary(x => x.Key.Key, x => x.ToArray());
                var balances = new Dictionary<ManagerServer.Model.IntangibleAsset, Balance>();
                foreach (var e in rows)
                {
                    var acquisitionCost = 0m;
                    var amortization = 0m;
                    var bookValue = 0m;
                    if (generalLedger.TryGetValue(e.Key, out ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] transactions))
                    {
                        acquisitionCost = transactions.Where(x => x.GeneralLedgerAccount.IsControlAccountForIntangibleAssets).Sum(x => x.BaseAmount);
                        amortization = transactions.Where(x => x.GeneralLedgerAccount.IsControlAccountForIntangibleAssetsAccumulatedAmortization).Sum(x => x.BaseAmount) * -1m;
                        bookValue = acquisitionCost - amortization;
                    }

                    if (e.DisposedIntangibleAsset && e.DisposalDate.HasValue)
                    {
                        bookValue = 0m;
                    }

                    balances.Add(e, new Balance()
                    {
                        AcquisitionCost = new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(acquisitionCost, baseCurrency, new IntangibleAssetTransactions() { IntangibleAsset = e.Key, Business = Business, Referrer = referrer }),
                        Amortization = new Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>(amortization, baseCurrency, new IntangibleAssetAccumulatedAmortizationTransactions() { IntangibleAsset = e.Key, Business = Business, Referrer = referrer }),
                        BookValue = new Tuple<decimal, ManagerServer.Model.Currency>(bookValue, baseCurrency)
                    });
                }

                getBalances = balances;
            }
            return getBalances;
        }

        [Right, Sum]
        [Guid("b8f8d3de-1e9e-435d-8a18-c109b7a47456"), Default]
        [Guide("The total cost of acquiring the intangible asset, calculated from all transactions assigned to this asset.")]
        public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>[] GetAcquisitionCost(ManagerServer.Model.IntangibleAsset[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].AcquisitionCost).ToArray();
        }

        [Right, Sum]
        [Guid("8fffe7cc-38fc-4bfb-8606-e549a5613ae4"), Default]
        [Guide("The accumulated amortization amount, calculated from all amortization entries recorded for this intangible asset.")]
        public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>[] GetAmortization(ManagerServer.Model.IntangibleAsset[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].Amortization).ToArray();
        }

        [Right, Sum]
        [Guid("18353681-dfc2-48c0-a155-58233e8545f4"), Default, Bold]
        [Guide("The current book value of the intangible asset, calculated by subtracting *Amortization* from *Acquisition cost*.")]
        public Tuple<decimal, ManagerServer.Model.Currency>[] GetBookValue(ManagerServer.Model.IntangibleAsset[] rows)
        {
            var balances = GetBalances(rows);
            return rows.Select(x => balances[x].BookValue).ToArray();
        }

        public sealed class Balance
        {
            public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate> AcquisitionCost;
            public Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate> Amortization;
            public Tuple<decimal, ManagerServer.Model.Currency> BookValue;
        }

        [Default]
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("da8caba4-f283-4ffd-840f-c5a16e296fe4")]
        [Guide("Shows whether the intangible asset is currently *Active* or has been *Disposed*.")]
        public Status[] GetStatus(ManagerServer.Model.IntangibleAsset[] rows)
        {
            return rows.Select(x => x.DisposedIntangibleAsset && x.DisposalDate.HasValue).Select(x => x ? Status.Disposed : Status.Active).ToArray();
        }

        public enum Status
        {
            [ManagerServer.Model.Attributes.Success] Active,
            Disposed
        }
    }
}
