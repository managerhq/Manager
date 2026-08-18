using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ReceiptsAndPaymentsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.ReceiptsAndPaymentsSummary), nameof(Strings.Transactions))]
    [Guide("The Receipts & Payments Summary Transactions screen shows detailed cash movements.")]
    [Guide("It displays receipts, payments, transfers, and expense claims for the selected account.")]
    internal sealed class ReceiptsAndPaymentsSummaryTransactions : TransactionViewer
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;
        [ProtoMember(3)] public Guid Account;
        [ProtoMember(4)] public bool ReverseSign;

        protected override bool ShowBaseAmount()
        {
            return true;
        }

        protected override bool MultipleByOne()
        {
            return ReverseSign;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.GeneralLedgerAccount.Key == Account)
                .Where(x => x.Transaction is Receipt || x.Transaction is Payment || x.Transaction is InterAccountTransfer || x.Transaction is ExpenseClaim)
                .Where(x => x.Date >= From && x.Date <= To);
        }
    }
}