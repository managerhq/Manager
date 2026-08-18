using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using HttpFramework;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ExpenseClaimsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.ExpenseClaimsSummary), nameof(Strings.Transactions))]
    [Guide("Shows expense claim transactions for a specific payer within the date range.")]
    [Guide("Displays expense claims and their related payment transactions.")]
    internal sealed class ExpenseClaimsSummaryTransactions : TransactionViewer
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;
        [ProtoMember(3)] public bool ExpenseClaims;
        [ProtoMember(4)] public bool Payments;
        [ProtoMember(5)] public Guid ExpenseClaimsPayer;

        protected override bool MultipleByOne()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.Key == ManagerServer.Model.Master.AccountKeys.ExpenseClaims && x.ExpenseClaimPayer.Key == ExpenseClaimsPayer).Where(x => x.Date >= From && x.Date <= To);
            if (ExpenseClaims) transactions = transactions.Where(x => x.Transaction is ExpenseClaim);
            if (Payments) transactions = transactions.Where(x => !(x.Transaction is ExpenseClaim));
            return transactions;
        }
    }
}