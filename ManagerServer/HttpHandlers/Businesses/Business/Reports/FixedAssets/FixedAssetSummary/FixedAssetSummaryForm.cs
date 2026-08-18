using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.FixedAssetSummary
{
    [ProtoContract]
    [Title(nameof(Strings.FixedAssetSummary))]
    [Guide("The Fixed Asset Summary form configures parameters for fixed asset reports.")]
    [Guide("Set the reporting date to view asset values and accumulated depreciation.")]
    [Fields(typeof(ManagerServer.Model.FixedAssetSummary))]
    internal sealed class FixedAssetSummaryForm : NakedVueForm<ManagerServer.Model.FixedAssetSummary>
    {
    }
}
