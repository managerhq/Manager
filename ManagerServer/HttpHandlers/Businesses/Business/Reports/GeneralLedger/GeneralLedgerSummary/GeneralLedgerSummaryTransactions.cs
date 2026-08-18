using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using HttpFramework;
using ManagerServer.Helpers;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.GeneralLedgerSummary
{
    [ProtoContract]
    [Title(nameof(Strings.GeneralLedgerSummary), nameof(Strings.Transactions))]
    [Guide("Shows transactions for specific general ledger accounts within the date range.")]
    [Guide("Displays debits, credits, and account movements with calculated adjustments.")]
    internal sealed class GeneralLedgerSummaryTransactions : TransactionViewer
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;
        [ProtoMember(3)] public bool Debits;
        [ProtoMember(4)] public bool Credits;
        [ProtoMember(5)] public Guid? BalanceSheetAccount;
        [ProtoMember(6)] public Guid? ProfitAndLossAccount;

        protected override bool ShowBaseAmount()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .DisposeFixedAssets()
                .DisposeIntangibleAssets()
                .Revaluate(From, To);

            var transactions = generalLedger.Where(x => x.Date >= From && x.Date <= To);
            if (BalanceSheetAccount.HasValue) transactions = transactions.Where(x => x.BalanceSheetAccount.Key == BalanceSheetAccount.Value);
            if (ProfitAndLossAccount.HasValue) transactions = transactions.Where(x => x.ProfitAndLossAccount != null && x.ProfitAndLossAccount.Key == ProfitAndLossAccount.Value);
            if (!Debits) transactions = transactions.Where(x => !(x.BaseAmount > 0));
            if (!Credits) transactions = transactions.Where(x => !(x.BaseAmount < 0));
            return transactions;
        }
    }
}