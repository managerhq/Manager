using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.DepreciationCalculationWorksheet
{
    [ProtoContract]
    [Title(nameof(Strings.DepreciationCalculationWorksheet))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`DepreciationCalculationWorksheet` is a tool designed to help you calculate depreciation amounts for `FixedAssets`.")]
    [Guide("To create a new `DepreciationCalculationWorksheet`, go to `Reports` tab, click `DepreciationCalculationWorksheet`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.DepreciationCalculationWorksheet), name: nameof(Strings.NewReport))]
    internal sealed class DepreciationCalculationWorksheetList : PersistentObjectTable<ManagerServer.Model.DepreciationCalculationWorksheet>
    {
        [Guid("887b8027-0c9c-44e0-92a9-193195521abc")]
        [Center, MinWidth, WhitespaceNoWrap]
        public DateTime GetFromDate(ManagerServer.Model.DepreciationCalculationWorksheet o) => o.FromDate;

        [Guid("177ca111-d958-4610-b27b-5758f1855caa")]
        [Center, MinWidth, WhitespaceNoWrap]
        public DateTime GetToDate(ManagerServer.Model.DepreciationCalculationWorksheet o) => o.ToDate;

        [Guid("357d2409-48e3-4790-bd46-dbc21ef238cd")]
        public string GetDescription(ManagerServer.Model.DepreciationCalculationWorksheet o) => o.Description;
    }
}