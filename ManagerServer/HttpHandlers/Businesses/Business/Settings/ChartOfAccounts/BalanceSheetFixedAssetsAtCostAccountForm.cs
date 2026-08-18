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

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ChartOfAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.Account), nameof(Strings.Fixed_assets_at_cost))]
    [Guide("This form allows to rename built-in `Fixed_assets_at_cost` account.")]
    [Guide("To access this form, go to `Settings`, then `ChartOfAccounts`, then click `Edit` button for `Fixed_assets_at_cost` account.")]
    [Guide("The form contains the following fields:")]
    [Fields(typeof(BalanceSheetFixedAssetsAtCostAccount))]
    [Guide("Click `Update` button to save your changes.")]
    [Guide("This account cannot be deleted, it is automatically added to your `ChartOfAccounts` when you have created at least one fixed asset.")]
    [LinkGuide("For more information see:", typeof(FixedAssets.FixedAssets))]
    internal sealed class BalanceSheetFixedAssetsAtCostAccountForm : NakedVueForm<BalanceSheetFixedAssetsAtCostAccount>
    {
    }
}