using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.DepreciationEntries
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("1f23f820-3ed7-461c-8088-c656c79526b7")]
    [Title(nameof(Strings.DepreciationEntries))]
    [Guide("The **Depreciation Entries** tab enables users to monitor the decrease in value of the company's fixed assets throughout their anticipated lifespan.")]
    [TabScreenshot("fa-sort-size-down", nameof(Strings.DepreciationEntries))]
    [Guide("To create a new depreciation entry, click the **New Depreciation Entry** button.")]
    [HeroButtonScreenshot(nameof(Strings.DepreciationEntries), nameof(Strings.NewDepreciationEntry))]
    [Guide("The **Depreciation Entries** tab displays the following columns:")]
    [Columns]
    internal sealed class DepreciationEntries : NakedObjectsWithAutomaticRows<ManagerServer.Model.DepreciationEntry>
    {
        [Center]
        [Default]
        [MinWidth]
        [WarnIfFutureDate]
        [WhitespaceNoWrap]
        [Guid("4dab039e-d380-41fa-8ffa-669253c3fed6")]
        [Guide("The date when the depreciation was recorded")]
        public DateTime[] GetDate(ManagerServer.Model.DepreciationEntry[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Center]
        [MinWidth]
        [PaddedSorting]
        [WarnIfNotUnique]
        [Guid("4c34b43d-713c-4a58-9605-0acdf31e913a")]
        [Guide("A unique reference number for the depreciation entry")]
        public string[] GetReference(ManagerServer.Model.DepreciationEntry[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("2f40f756-b7e8-4094-a9d4-493f52d0808c")]
        [Guide("A description or explanation of the depreciation entry")]
        public string[] GetDescription(ManagerServer.Model.DepreciationEntry[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Guid("f4850c28-6c4f-435e-92ed-04ab05b547bf")]
        [Guide("The names of *fixed assets* included in this depreciation entry")]
        public string[] GetFixedAssets(ManagerServer.Model.DepreciationEntry[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => string.Join(", ", x.GetGeneralLedgerTransactions(database).Where(x => x.FixedAsset != null).Select(x => x.FixedAsset).Distinct().Select(x => x.NameWithCode))).ToArray();
        }

        [Guid("2e3e6730-2a13-4d2a-bc1e-89a22e053753")]
        [Guide("The names of *divisions* associated with the depreciation entry (if divisional accounting is enabled)")]
        public string[] GetDivisions(ManagerServer.Model.DepreciationEntry[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => string.Join(", ", x.GetGeneralLedgerTransactions(database).Where(x => x.Division != null).Select(x => x.Division).Distinct().Select(x => x.Name))).ToArray();
        }

        [Bold, Right, Sum, Default]
        [Guid("66499d74-5486-412c-b328-09f8315173b7")]
        [Guide("The total depreciation amount for this entry")]
        public Tuple<decimal, ManagerServer.Model.Currency>[] GetAmount(ManagerServer.Model.DepreciationEntry[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();
            return rows.Select(x => new Tuple<decimal, ManagerServer.Model.Currency>(x.GetGeneralLedgerTransactions(database).Where(x => x.BaseAmount > 0m).Sum(x => x.BaseAmount), baseCurrency)).ToArray();
        }
    }
}
