using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryProfitMargin
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryProfitMargin))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`InventoryProfitMargin` provides a comprehensive analysis of the profitability of your inventory items by calculating the margin between their sales price and cost price.")]
    [Guide("To create a new `InventoryProfitMargin`, go to `Reports` tab, click `InventoryProfitMargin`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.InventoryProfitMargin), name: nameof(Strings.NewReport))]
    internal sealed class InventoryProfitMarginList : PersistentObjectTable<ManagerServer.Model.InventoryProfitMargin>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("740c7172-2e88-47f1-bf22-cb4a2eb7f75a")]
        public DateTime GetFromDate(ManagerServer.Model.InventoryProfitMargin o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("6847c791-c5d5-4831-856b-4d8f487ee624")]
        public DateTime GetToDate(ManagerServer.Model.InventoryProfitMargin o) => o.ToDate;

        [Guid("0d74dff0-8953-4e15-bd84-008b0a55b350")]
        public string GetDescription(ManagerServer.Model.InventoryProfitMargin o) => o.Description;
    }
}