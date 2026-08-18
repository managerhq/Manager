using System;
using ManagerServer.HttpHandlers.Businesses.Business.Summary;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.BalanceSheet
{
    [ProtoContract]
    [Title(nameof(Strings.BalanceSheet), nameof(Strings.Transactions))]
    [Guide("The Balance Sheet Transactions screen shows the detailed transactions for balance sheet accounts.")]
    [Guide("It provides a drill-down view of the general ledger entries that make up account balances.")]
    internal sealed class BalanceSheetTransactions : BaseGeneralLedgerTransactionsInheritable
    {
    }
}