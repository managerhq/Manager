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
using ManagerServer.Api.Businesses.Business.Reports.FixedAssetSummary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.FixedAssetSummary
{
    [ProtoContract]
    [Title(nameof(Strings.FixedAssetSummary))]
    [Guide("The Fixed Asset Summary report shows movements in fixed assets.")]
    [Guide("It tracks costs, depreciation, and disposals for the reporting period.")]
    [LinkGuide("For more information see:", typeof(FixedAssetSummaryForm))]
    internal sealed class FixedAssetSummaryView : DefaultView<GetFixedAssetSummaryView>
    {
    }
}