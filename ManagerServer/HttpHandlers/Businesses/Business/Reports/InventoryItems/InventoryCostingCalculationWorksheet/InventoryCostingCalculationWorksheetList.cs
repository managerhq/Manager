using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryCostingCalculationWorksheet
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryCostingCalculationWorksheet))]
    [Guide("`InventoryCostingCalculationWorksheet` calculates unit costs for inventory items.")]
    [Guide("To create a new `InventoryCostingCalculationWorksheet`, go to `Reports` tab, click `InventoryCostingCalculationWorksheet`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.InventoryCostingCalculationWorksheet), name: nameof(Strings.NewReport))]
    internal sealed class InventoryCostingCalculationWorksheetList : NakedObjectsWithAutomaticRows<ManagerServer.Model.InventoryCostingCalculationWorksheet>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewReport);
        }

        [Default, MinWidth, WhitespaceNoWrap]
        public DateTime[] GetDate(ManagerServer.Model.InventoryCostingCalculationWorksheet[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [Default]
        public string[] GetValuationMethod(ManagerServer.Model.InventoryCostingCalculationWorksheet[] rows)
        {
            return rows.Select(x => Strings.GetPropertyValue(x.ValuationMethod.ToString())).ToArray();
        }

        [Default]
        public string[] GetDescription(ManagerServer.Model.InventoryCostingCalculationWorksheet[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }
    }
}