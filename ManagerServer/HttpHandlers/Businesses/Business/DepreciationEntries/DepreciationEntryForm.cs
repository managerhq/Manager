using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.DepreciationEntries
{
    [ProtoContract]
    [Title(nameof(Strings.DepreciationEntry), nameof(Strings.Edit))]
    [Guide("The Depreciation Entry form is used to record depreciation for fixed assets.")]
    [Guide("Depreciation entries reduce the book value of fixed assets over their useful life.")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.DepreciationEntry))]
    internal sealed class DepreciationEntryForm : NakedVueForm<ManagerServer.Model.DepreciationEntry>
    {
        protected override bool CanHaveImage() => true;

        protected override void OnSource(DepreciationEntry form, ManagerServer.Model.Object source)
        {
            if (source is ManagerServer.Model.DepreciationCalculationWorksheet report)
            {
                var items = ManagerServer.Api.Businesses.Business.Reports.DepreciationCalculationWorksheet.GetDepreciationCalculationWorksheetView.GetItems(Business, report);
                form.Lines = items.Where(x => x.RecalculatedDepreciation - x.DepreciationEntries > 0m).Select(x => new DepreciationEntry.Line() { FixedAsset = x.FixedAsset, Amount = x.RecalculatedDepreciation - x.DepreciationEntries }).ToArray();
                form.Date = report.ToDate;
                form.Description = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());
            }
        }
    }
}