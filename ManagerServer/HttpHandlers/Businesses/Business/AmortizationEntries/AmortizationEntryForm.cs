using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.AmortizationEntries
{
    [ProtoContract]
    [Title(nameof(Strings.Amortization), nameof(Strings.Edit))]
    [Guide("The amortization entry form allows you to create new amortization entries or edit existing ones.")]
    [Guide("Use this form to record periodic amortization expenses for your intangible assets.")]
    [Header("Form Fields")]
    [Guide("The form contains the following fields:")]
    [Fields(typeof(AmortizationEntry))]
    internal sealed class AmortizationEntryForm : NakedVueForm<ManagerServer.Model.AmortizationEntry>
    {
        protected override bool CanHaveImage() => true;

        protected override void OnSource(AmortizationEntry form, ManagerServer.Model.Object source)
        {
            if (source is ManagerServer.Model.AmortizationCalculationWorksheet report)
            {
                var items = ManagerServer.Api.Businesses.Business.Reports.AmortizationCalculationWorksheet.GetAmortizationCalculationWorksheetView.GetItems(Business, report);
                form.Lines = items.Where(x => x.Amortization > 0m).Select(x => new AmortizationEntry.Line() { IntangibleAsset = x.IntangibleAsset, Amount = x.Amortization }).ToArray();
                form.Date = report.ToDate;
                form.Description = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());
            }
        }
    }
}