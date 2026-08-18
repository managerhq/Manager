using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.DepreciationCalculationWorksheet
{
    [ProtoContract]
    [Title(nameof(Strings.DepreciationCalculationWorksheet))]
    [Guide("The Depreciation Calculation Worksheet form configures depreciation report parameters.")]
    [Guide("Set the date range to calculate depreciation expenses for your fixed assets.")]
    [Fields(typeof(ManagerServer.Model.DepreciationCalculationWorksheet))]
    internal sealed class DepreciationCalculationWorksheetForm : NakedVueForm<ManagerServer.Model.DepreciationCalculationWorksheet>
    {
    }
}