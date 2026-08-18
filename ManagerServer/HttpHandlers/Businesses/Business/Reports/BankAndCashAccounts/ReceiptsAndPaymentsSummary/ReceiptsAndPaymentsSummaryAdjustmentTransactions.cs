using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ReceiptsAndPaymentsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.ReceiptsAndPaymentsSummary), nameof(Strings.Adjustments), nameof(Strings.Transactions))]
    [Guide("The Receipts & Payments Summary Adjustment Transactions screen shows journal entries affecting cash.")]
    [Guide("It displays manual adjustments made to bank and cash accounts through journal entries.")]
    internal sealed class ReceiptsAndPaymentsSummaryAdjustmentTransactions : TransactionViewer
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;

        protected override bool ShowBaseAmount()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.GeneralLedgerAccount.IsCashAtBank)
                .Where(x => x.Transaction is JournalEntry)
                .Where(x => x.Date >= From && x.Date <= To);
        }
    }
}