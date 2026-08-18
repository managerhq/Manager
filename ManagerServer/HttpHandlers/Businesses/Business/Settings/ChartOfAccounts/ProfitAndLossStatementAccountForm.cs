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
    [Title(nameof(Strings.ProfitAndLossStatementAccount), nameof(Strings.Edit))]
    [Guide("The Profit and Loss Statement Account form is used to create income and expense accounts.")]
    [Guide("These accounts track revenues and expenses for profitability reporting.")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.ProfitAndLossStatementAccount))]
    internal sealed class ProfitAndLossStatementAccountForm : NakedVueForm<ProfitAndLossStatementAccount>
    {
    }
}
