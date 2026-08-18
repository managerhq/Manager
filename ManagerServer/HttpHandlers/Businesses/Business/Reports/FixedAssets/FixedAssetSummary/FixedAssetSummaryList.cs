using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.FixedAssetSummary
{
    [ProtoContract]
    [Title(nameof(Strings.FixedAssetSummary))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`FixedAssetSummary` provides a comprehensive overview of all your fixed assets, including detailed information on acquisition costs, depreciation, and current book values.")]
    [Guide("To create a new `FixedAssetSummary`, go to `Reports` tab, click `FixedAssetSummary`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.FixedAssetSummary), name: nameof(Strings.NewReport))]
    internal sealed class FixedAssetSummaryList : PersistentObjectTable<ManagerServer.Model.FixedAssetSummary>
    {
        [Guid("dcb15e8c-ddb4-4c34-9f17-44dbbe2d9b17")]
        [Center, MinWidth, WhitespaceNoWrap]
        public DateTime GetFromDate(ManagerServer.Model.FixedAssetSummary o) => o.FromDate;

        [Guid("d2ab5dac-eee7-4fd1-90a3-4e4144c3eb36")]
        [Center, MinWidth, WhitespaceNoWrap]
        public DateTime GetToDate(ManagerServer.Model.FixedAssetSummary o) => o.ToDate;

        [Guid("4fe54077-9a97-465a-b1df-0fa6f324c416")]
        public string GetDescription(ManagerServer.Model.FixedAssetSummary o) => o.Description;
    }
}