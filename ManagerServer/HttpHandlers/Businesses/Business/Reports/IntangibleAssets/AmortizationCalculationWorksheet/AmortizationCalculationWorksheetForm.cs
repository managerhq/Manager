using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.AmortizationCalculationWorksheet
{
    [ProtoContract]
    [Title(nameof(Strings.AmortizationCalculationWorksheet), nameof(Strings.Edit))]
    [Guide("The Amortization Calculation Worksheet form is used to configure report parameters.")]
    [Guide("Set the date range to calculate amortization expenses for intangible assets.")]
    [Fields(typeof(ManagerServer.Model.AmortizationCalculationWorksheet))]
    internal sealed class AmortizationCalculationWorksheetForm : NakedVueForm<ManagerServer.Model.AmortizationCalculationWorksheet>
    {
    }
}