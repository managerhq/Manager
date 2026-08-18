using System;
using ManagerServer.HttpHandlers.Businesses.Business.Summary;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatementActualVsBudget
{
    [ProtoContract]
    [Title(nameof(Strings.ProfitAndLossStatementActualVsBudget), nameof(Strings.Transactions))]
    [Guide("Shows detailed transactions for actual vs budget comparisons.")]
    [Guide("Displays actual income and expense transactions for variance analysis.")]
    internal sealed class ProfitAndLossStatementActualVsBudgetTransactions : BaseGeneralLedgerTransactionsInheritable
    {
    }
}