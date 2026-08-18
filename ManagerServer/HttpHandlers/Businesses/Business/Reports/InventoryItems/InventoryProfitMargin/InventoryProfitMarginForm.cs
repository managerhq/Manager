using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryProfitMargin
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryProfitMargin))]
    [Guide("The Inventory Profit Margin form configures parameters for profit analysis.")]
    [Guide("Set date ranges to analyze sales margins and profitability by inventory item.")]
    [Fields(typeof(ManagerServer.Model.InventoryProfitMargin))]
    internal sealed class InventoryProfitMarginForm : NakedVueForm<ManagerServer.Model.InventoryProfitMargin>
    {
    }
}
