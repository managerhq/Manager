using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.AmortizationCalculationWorksheet;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.AmortizationCalculationWorksheet
{
    [ProtoContract]
    [Title(nameof(Strings.AmortizationCalculationWorksheet), nameof(Strings.View))]
    [Guide("The Amortization Calculation Worksheet shows amortization amounts for intangible assets.")]
    [Guide("It calculates amortization based on asset book values and amortization rates.")]
    [LinkGuide("For more information see:", typeof(AmortizationCalculationWorksheetForm))]
    internal sealed class AmortizationCalculationWorksheetView : DefaultView<GetAmortizationCalculationWorksheetView>
    {
        protected override Tuple<string, BusinessTemplate> GetFooterAction()
        {
            return new Tuple<string, BusinessTemplate>(Strings.NewAmortizationEntry, new AmortizationEntries.AmortizationEntryForm() { Business = Business, Source = Key });
        }
    }
}