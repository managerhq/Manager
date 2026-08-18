using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.GeneralLedgerTransactions;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.GeneralLedgerTransactions
{
    [ProtoContract]
    [Title(nameof(Strings.GeneralLedgerTransactions))]
    [Guide("The General Ledger Transactions report shows detailed accounting entries.")]
    [Guide("It displays all debits, credits, and running balances for each account.")]
    [LinkGuide("For more information see:", typeof(GeneralLedgerTransactionsForm))]
    internal sealed class GeneralLedgerTransactionsView : DefaultView<GetGeneralLedgerTransactionsView>
    {
    }
}