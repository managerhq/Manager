using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryQuantitySummary
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryQuantitySummary))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`InventoryQuantitySummary` provides a comprehensive overview of the quantities of inventory items on hand, helping you manage stock levels effectively and streamline your inventory operations.")]
    [Guide("To create a new `InventoryQuantitySummary`, go to `Reports` tab, click `InventoryQuantitySummary`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.InventoryQuantitySummary), name: nameof(Strings.NewReport))]
    internal sealed class InventoryQuantitySummaryList : PersistentObjectTable<ManagerServer.Model.InventoryQuantitySummary>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("40de6a29-1504-4a46-9379-65f7eebee2c7")]
        public DateTime GetFromDate(ManagerServer.Model.InventoryQuantitySummary o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("dc99150e-8645-4e4d-bb31-3a1658530717")]
        public DateTime GetToDate(ManagerServer.Model.InventoryQuantitySummary o) => o.ToDate;

        [Guid("2a0943f4-8842-44e2-9a13-f2bcdbd0bf3d")]
        public string GetDescription(ManagerServer.Model.InventoryQuantitySummary o) => o.Description;
    }
}