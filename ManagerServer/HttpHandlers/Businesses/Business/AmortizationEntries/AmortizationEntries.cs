using System.Linq;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.AmortizationEntries
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("2504301e-2816-4f26-8f87-232186801482")]
    [Title(nameof(Strings.AmortizationEntries))]
    [Guide("The **Amortization Entries** tab documents the gradual reduction in value of intangible assets, a process known as amortization.")]
    [Guide("Here you can record periodic amortization expenses to reflect the declining value of intangible assets over their useful life.")]
    [TabScreenshot("fa-sort-amount-down", nameof(Strings.AmortizationEntries))]
    [Header("Creating Amortization Entries")]
    [Guide("To create a new amortization entry, click the **New Amortization Entry** button.")]
    [HeroButtonScreenshot(nameof(Strings.AmortizationEntries), nameof(Strings.NewAmortizationEntry))]
    [LinkGuide("For more information, see:", typeof(AmortizationEntryForm))]
    [Header("Understanding the Columns")]
    [Guide("The **Amortization Entries** tab displays information in the following columns:")]
    [Columns]
    [Guide("Click the **Edit Columns** button to customize which columns are visible.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [LinkGuide("Learn more about:", typeof(NakedObjectsWithEditColumns<AmortizationEntry>))]
    internal sealed class AmortizationEntries : NakedObjectsWithAutomaticRows<AmortizationEntry>
    {
        [Center]
        [Default]
        [MinWidth]
        [WhitespaceNoWrap]
        [WarnIfFutureDate]
        [Guid("bd79f40b-2b07-49d6-b51f-a5334c431591")]
        [Guide("The **Date** column displays the date of the amortization entry.")]
        public DateTime[] GetDate(AmortizationEntry[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Center]
        [MinWidth]
        [PaddedSorting]
        [WarnIfNotUnique]
        [Guid("bc6cfa44-7758-4d72-8641-2f3b623c3c56")]
        [Guide("The **Reference** column displays the reference number for each amortization entry.")]
        public string[] GetReference(AmortizationEntry[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("92334707-90b0-452b-8c98-d587451ac67d")]
        [Guide("The **Description** column displays the description of the amortization entry.")]
        public string[] GetDescription(AmortizationEntry[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Guid("5d4deea1-b3b5-445b-aa25-2c5cdf817ba7")]
        [Guide("The **Intangible Assets** column displays the intangible assets affected by this amortization entry, separated by commas.")]
        public string[] GetIntangibleAssets(AmortizationEntry[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows
                .Select(x => string.Join(", ", x.GetGeneralLedgerTransactions(database)
                .Where(x => x.IntangibleAsset != null)
                .Select(x => x.IntangibleAsset)
                .Distinct()
                .Select(x => x.NameWithCode)))
                .ToArray();
        }

        [Guid("f22de843-3019-44a2-aa8d-806be6d428c1")]
        [Guide("The **Division** column displays the divisions associated with this amortization entry.")]
        public string[] GetDivision(AmortizationEntry[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows
                .Select(x => string.Join(", ", x.GetGeneralLedgerTransactions(database)
                .Where(x => x.Division != null)
                .Select(x => x.Division)
                .Distinct()
                .Select(x => x.Name)))
                .ToArray();
        }

        [Bold, Right, Sum, Default]
        [Guid("d98d34f6-ab12-4087-a45d-574d04dbc79a")]
        [Guide("The **Amount** column displays the total amortization amount for all lines in the entry.")]
        public Tuple<decimal, Currency>[] GetAmount(AmortizationEntry[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var baseCurrency = database.Single<BaseCurrency>();
            return rows
                .Select(x => new Tuple<decimal, Currency>(x.GetGeneralLedgerTransactions(database)
                .Where(x => x.BaseAmount > 0m)
                .Sum(x => x.BaseAmount), baseCurrency))
                .ToArray();
        }
    }
}
