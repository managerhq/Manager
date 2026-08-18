using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.AmortizationCalculationWorksheet
{
    [ProtoContract]
    [Title(nameof(Strings.AmortizationCalculationWorksheet))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`AmortizationCalculationWorksheet` is a tool designed to help you calculate amortization amounts for `IntangibleAssets`.")]
    [Guide("To create a new `AmortizationCalculationWorksheet`, go to `Reports` tab, click `AmortizationCalculationWorksheet`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.AmortizationCalculationWorksheet), name: nameof(Strings.NewReport))]
    internal sealed class AmortizationCalculationWorksheetList : PersistentObjectTable<ManagerServer.Model.AmortizationCalculationWorksheet>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("ad4d75f6-dc3c-4856-8102-936c7ceac16e")]
        public DateTime GetFromDate(ManagerServer.Model.AmortizationCalculationWorksheet o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("f5f69ef8-0b2c-41f5-bf9c-c33ac3e81f98")]
        public DateTime GetToDate(ManagerServer.Model.AmortizationCalculationWorksheet o) => o.ToDate;

        [Guid("b11ba982-5cdf-4054-9828-83232ebfe66a")]
        public string GetDescription(ManagerServer.Model.AmortizationCalculationWorksheet o) => o.Description;        
    }
}