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
using ManagerServer.HttpHandlers.Businesses.Business.FixedAssets;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.IntangibleAssetSummary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.IntangibleAssetSummary
{
    [ProtoContract]
    [Title(nameof(Strings.IntangibleAssetSummary))]
    [Guide("The Intangible Asset Summary report shows movements in intangible assets.")]
    [Guide("It tracks costs, amortization, and disposals for the reporting period.")]
    [LinkGuide("For more information see:", typeof(IntangibleAssetSummaryForm))]
    internal sealed class IntangibleAssetSummaryView : DefaultView<GetIntangibleAssetSummaryView>
    {        
    }
}