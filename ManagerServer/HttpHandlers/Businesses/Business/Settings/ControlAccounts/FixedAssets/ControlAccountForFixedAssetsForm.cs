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

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.FixedAssets
{
    [ProtoContract]
    [Title(nameof(Strings.Fixed_assets_at_cost), nameof(Strings.Edit))]
    [Guide("This form configures the control account for fixed assets at cost.")]
    [Guide("The control account tracks the purchase cost of fixed assets on the balance sheet.")]
    [Fields(typeof(ControlAccountForFixedAssets))]
    internal sealed class ControlAccountForFixedAssetsForm : NakedVueForm<ControlAccountForFixedAssets>
    {
    }
}
