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
    [Title(nameof(Strings.Suspense))]
    [Guide("Configure the suspense account for recording unbalanced transactions.")]
    [Guide("This account automatically captures any discrepancies to maintain balance.")]
    [Fields(typeof(ManagerServer.Model.BalanceSheetSuspenseAccount))]
    internal sealed class BalanceSheetSuspenseAccountForm : NakedVueForm<BalanceSheetSuspenseAccount>
    {
    }
}