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
    [Title(nameof(Strings.Account), nameof(Strings.InterAccountTransfers))]
    [Guide("This form allows to rename built-in `InterAccountTransfers` account.")]
    [Guide("To access this form, go to `Settings`, then `ChartOfAccounts`, then click `Edit` button for `InterAccountTransfers` account.")]
    [Guide("The form contains the following fields:")]
    [Fields(typeof(BalanceSheetInterAccountTransfers))]
    [Guide("Click `Update` button to save your changes.")]
    [Guide("The purpose of this account is to faciliate inter account transfers.")]
    [LinkGuide("For more information see:", typeof(InterAccountTransfers.InterAccountTransfers))]
    [Guide("This account cannot be deleted, it is automatically added to your `ChartOfAccounts` when you have created at least one bank or cash account.")]
    [Guide("On reports, if this account's balance is zero, you it can be hidden if your reports are set to `ExcludeZeroBalances`.")]
    internal sealed class BalanceSheetInterAccountTransfersForm : NakedVueForm<BalanceSheetInterAccountTransfers>
    {
    }
}