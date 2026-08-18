using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.InventoryProfitMargin;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryProfitMargin
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryProfitMargin))]
    [Guide("The Inventory Profit Margin report analyzes profitability by inventory item.")]
    [Guide("It shows sales revenue, cost of sales, profit amounts, and profit margins.")]
    [LinkGuide("For more information see:", typeof(InventoryProfitMarginForm))]
    internal sealed class InventoryProfitMarginView : DefaultView<GetInventoryProfitMarginView>
    {
    }
}