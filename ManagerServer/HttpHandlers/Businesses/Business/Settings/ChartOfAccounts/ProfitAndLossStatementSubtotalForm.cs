using System;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ChartOfAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.ProfitAndLossStatement), nameof(Strings.Subtotal))]
    [Guide("Create subtotals to organize groups of accounts on the profit and loss statement.")]
    [Guide("Subtotals help structure financial reports for better readability.")]
    [Fields(typeof(ManagerServer.Model.Subtotal))]
    internal sealed class ProfitAndLossStatementSubtotalForm : NakedVueForm<Subtotal>
    {
    }
}
