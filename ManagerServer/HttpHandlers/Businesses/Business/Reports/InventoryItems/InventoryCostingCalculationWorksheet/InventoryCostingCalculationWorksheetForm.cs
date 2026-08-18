using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryCostingCalculationWorksheet
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryCostingCalculationWorksheet))]
    [Guide("The Inventory Costing Calculation Worksheet form configures parameters for cost analysis.")]
    [Guide("Set date ranges to calculate inventory costs using selected valuation methods.")]
    [Fields(typeof(ManagerServer.Model.InventoryCostingCalculationWorksheet))]
    internal sealed class InventoryCostingCalculationWorksheetForm : NakedVueForm<ManagerServer.Model.InventoryCostingCalculationWorksheet>
    {
    }
}