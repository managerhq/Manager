using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryValueSummary
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryValueSummary))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`InventoryValueSummary` provides a comprehensive overview of the total value of your inventory items, allowing you to track and manage your associated costs effectively.")]
    [Guide("To create a new `InventoryValueSummary`, go to `Reports` tab, click `InventoryValueSummary`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.InventoryValueSummary), name: nameof(Strings.NewReport))]
    internal sealed class InventoryValueSummaryList : PersistentObjectTable<ManagerServer.Model.InventoryValueSummary>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("173f4f20-c4cb-415b-bcd9-18ea5a01a3dc")]
        public DateTime GetFromDate(ManagerServer.Model.InventoryValueSummary o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("9fe280c2-eb26-45c0-819f-38862abee8bf")]
        public DateTime GetToDate(ManagerServer.Model.InventoryValueSummary o) => o.ToDate;

        [Guid("6b1a3107-af96-4aa8-ba61-ca5da99f1ced")]
        public string GetDescription(ManagerServer.Model.InventoryValueSummary o) => o.Description;
    }
}