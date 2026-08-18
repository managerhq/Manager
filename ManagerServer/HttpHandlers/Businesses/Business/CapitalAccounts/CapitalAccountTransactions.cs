using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.CapitalAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.CapitalAccounts), nameof(Strings.Transactions))]
    [Guide("This screen displays all transactions affecting a specific capital account.")]
    [Guide("View capital contributions, drawings, profit allocations, and other equity transactions.")]
    [Guide("The balance at the bottom shows the owner's current equity in the business.")]
    [LinkGuide("Learn about capital accounts:", typeof(CapitalAccountForm))]
    internal sealed class CapitalAccoutTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid CapitalAccount;

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsControlAccountForCapitalAccounts && x.CapitalAccount?.Key == CapitalAccount).OrderByDescending(x => x.Date).ToArray();
        }
    }
}
