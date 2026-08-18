using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.InventoryCostingCalculationWorksheet;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryCostingCalculationWorksheet
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryCostingCalculationWorksheet))]
    [Guide("The Inventory Costing Calculation Worksheet calculates inventory values.")]
    [Guide("It shows quantities, average costs, and total values using FIFO or weighted average methods.")]
    [LinkGuide("For more information see:", typeof(InventoryCostingCalculationWorksheetForm))]
    internal sealed class InventoryCostingCalculationWorksheetView : DefaultView<GetInventoryCostingCalculationWorksheetView>
    {
    }
}