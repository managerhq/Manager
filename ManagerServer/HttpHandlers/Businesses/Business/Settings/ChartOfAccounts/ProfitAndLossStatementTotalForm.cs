using System;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ChartOfAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.ProfitAndLossStatement), nameof(Strings.Total))]
    [Guide("Create totals to sum up sections on the profit and loss statement.")]
    [Guide("Totals provide summary calculations for groups of accounts.")]
    [Fields(typeof(ManagerServer.Model.ProfitAndLossStatementTotal))]
    internal sealed class ProfitAndLossStatementTotalForm : NakedVueForm<ProfitAndLossStatementTotal>
    {
    }
}
