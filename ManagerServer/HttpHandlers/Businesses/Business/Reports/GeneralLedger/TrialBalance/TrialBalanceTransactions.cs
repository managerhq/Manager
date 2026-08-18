using System;
using ManagerServer.HttpHandlers.Businesses.Business.Summary;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TrialBalance
{
    [ProtoContract]
    [Title(nameof(Strings.TrialBalance), nameof(Strings.Transactions))]
    [Guide("Shows individual transactions that make up account balances in the trial balance.")]
    [Guide("Click on account balances to see their detailed transaction history.")]
    internal sealed class TrialBalanceTransactions : BaseGeneralLedgerTransactionsInheritable
    {
    }
}