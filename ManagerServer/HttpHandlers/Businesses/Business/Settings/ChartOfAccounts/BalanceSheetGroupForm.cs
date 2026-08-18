using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using ManagerServer.Query;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ChartOfAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.BalanceSheetGroup), nameof(Strings.Edit))]
    [Guide("The Balance Sheet Group form is used to create and edit balance sheet account groups.")]
    [Guide("Groups organize related accounts on the balance sheet for better financial reporting.")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.BalanceSheetGroup))]
    internal sealed class BalanceSheetGroupForm : NakedVueForm<BalanceSheetGroup>
    {        
    }
}