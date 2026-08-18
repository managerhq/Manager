using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CapitalAccountsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.CapitalAccountsSummary), nameof(Strings.Transactions))]
    [Guide("Shows detailed transactions for a specific capital account and subaccount.")]
    [Guide("Displays all movements within the selected date range.")]
    internal sealed class CapitalAccountsSummaryTransactions : TransactionViewer
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;
        [ProtoMember(3)] public Guid CapitalAccount;
        [ProtoMember(4)] public Guid? CapitalSubaccount;

        protected override bool MultipleByOne()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsControlAccountForCapitalAccounts && x.Date >= From && x.Date <= To && x.CapitalAccount.Key == CapitalAccount && x.CapitalSubaccount?.Key == CapitalSubaccount);
        }
    }
}