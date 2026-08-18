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
    [Title(nameof(Strings.ProfitAndLossStatementGroup), nameof(Strings.Edit))]
    [Guide("This form creates or edits groups for the profit and loss statement.")]
    [Guide("Groups organize income and expense accounts for financial reporting.")]
    [Fields(typeof(ProfitAndLossStatementGroup))]
    internal sealed class ProfitAndLossStatementGroupForm : NakedVueForm<ProfitAndLossStatementGroup>
    {
    }
}
