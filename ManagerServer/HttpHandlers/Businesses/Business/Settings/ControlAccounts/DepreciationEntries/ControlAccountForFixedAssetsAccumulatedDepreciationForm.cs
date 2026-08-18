using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.DepreciationEntries
{
    [ProtoContract]
    [Title(nameof(Strings.ControlAccount), nameof(Strings.FixedAssets), nameof(Strings.AccumulatedDepreciation))]
    [Guide("Configure the control account for accumulated depreciation of fixed assets.")]
    [Guide("This account tracks the total depreciation recorded for all fixed assets.")]
    [Fields(typeof(ManagerServer.Model.ControlAccountForFixedAssetsAccumulatedDepreciation))]
    internal sealed class ControlAccountForFixedAssetsAccumulatedDepreciationForm : NakedVueForm<ControlAccountForFixedAssetsAccumulatedDepreciation>
    {
    }
}
