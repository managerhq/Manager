using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.DepreciationCalculationWorksheet;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.DepreciationCalculationWorksheet
{
    [ProtoContract]
    [Title(nameof(Strings.DepreciationCalculationWorksheet))]
    [Guide("The Depreciation Calculation Worksheet calculates required depreciation entries.")]
    [Guide("It compares calculated depreciation with recorded entries to identify differences.")]
    [LinkGuide("For more information see:", typeof(DepreciationCalculationWorksheetForm))]
    internal sealed class DepreciationCalculationWorksheetView : DefaultView<GetDepreciationCalculationWorksheetView>
    {
        protected override Tuple<string, BusinessTemplate> GetFooterAction()
        {
            return new Tuple<string, BusinessTemplate>(Strings.NewDepreciationEntry, new DepreciationEntries.DepreciationEntryForm() { Business = Business, Source = Key });
        }
    }
}