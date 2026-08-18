using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.IntangibleAssetSummary
{
    [ProtoContract]
    [Title(nameof(Strings.IntangibleAssetSummary))]
    [Guide("The Intangible Asset Summary form configures parameters for intangible asset reports.")]
    [Guide("Set the reporting period to view asset values and amortization.")]
    [Fields(typeof(ManagerServer.Model.IntangibleAssetSummary))]
    internal sealed class IntangibleAssetSummaryForm : NakedVueForm<ManagerServer.Model.IntangibleAssetSummary>
    {
    }
}
