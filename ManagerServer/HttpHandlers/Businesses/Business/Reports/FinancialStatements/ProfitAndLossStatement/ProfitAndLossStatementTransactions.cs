using System;
using ManagerServer.HttpHandlers.Businesses.Business.Summary;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatement
{
    [ProtoContract]
    [Title(nameof(Strings.ProfitAndLossStatement), nameof(Strings.Transactions))]
    [Guide("Shows detailed transactions for profit and loss statement accounts.")]
    [Guide("Displays income and expense transactions for the selected period.")]
    internal sealed class ProfitAndLossStatementTransactions : BaseGeneralLedgerTransactionsInheritable
    {
    }
}