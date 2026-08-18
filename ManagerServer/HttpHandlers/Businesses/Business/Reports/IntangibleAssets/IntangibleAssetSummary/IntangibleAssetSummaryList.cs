using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.IntangibleAssetSummary
{
    [ProtoContract]
    [Title(nameof(Strings.IntangibleAssetSummary))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`IntangibleAssetSummary` provides a comprehensive overview of all your intangible assets, including detailed information on acquisition costs, amortization, and current book values.")]
    [Guide("To create a new `IntangibleAssetSummary`, go to `Reports` tab, click `IntangibleAssetSummary`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.IntangibleAssetSummary), name: nameof(Strings.NewReport))]
    internal sealed class IntangibleAssetSummaryList : PersistentObjectTable<ManagerServer.Model.IntangibleAssetSummary>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("af8a3f05-a53f-4607-9ab7-90b713189947")]
        public DateTime GetFromDate(ManagerServer.Model.IntangibleAssetSummary o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("980ed78d-906b-42db-b6f5-c724a8530176")]
        public DateTime GetToDate(ManagerServer.Model.IntangibleAssetSummary o) => o.ToDate;

        [Guid("69ee029f-0eaf-4c4e-a507-e9f330c4857c")]
        public string GetDescription(ManagerServer.Model.IntangibleAssetSummary o) => o.Description;
    }
}