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
    [Title(nameof(Strings.BalanceSheet), nameof(Strings.Equity), nameof(Strings.Group), nameof(Strings.Name))]
    [Guide("Configure the name for the equity group on the balance sheet.")]
    [Guide("This customizes how the equity section appears in financial reports.")]
    [Fields(typeof(ManagerServer.Model.Equity))]
    internal sealed class BalanceSheetEquityGroupNameForm : NakedVueForm<Equity>
    {
    }
}
