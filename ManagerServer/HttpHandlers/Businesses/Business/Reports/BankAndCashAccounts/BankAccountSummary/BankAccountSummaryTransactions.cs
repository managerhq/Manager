using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.BankAccountSummary
{
    [ProtoContract]
    [Title(nameof(Strings.BankAccountSummary), nameof(Strings.Transactions))]
    [Guide("The Bank Account Summary Transactions screen shows detailed transactions for a bank account.")]
    [Guide("It displays all debits and credits within the specified date range.")]
    internal sealed class BankAccountSummaryTransactions : TransactionViewer
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;
        [ProtoMember(3)] public Guid GeneralLedgerAccount;
        [ProtoMember(4)] public Guid BankAccount;
        [ProtoMember(5)] public bool Debits;
        [ProtoMember(6)] public bool Credits;

        protected override bool MultipleByOne()
        {
            return Debits;
        }

        protected override bool ShowTransactionAmount()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.Key == GeneralLedgerAccount && x.BankAccount?.Key == BankAccount && x.Date >= From && x.Date <= To);
            if (Credits) return transactions.Where(x => x.TransactionAmount > 0m);
            if (Debits) return transactions.Where(x => x.TransactionAmount < 0m);
            return null;
        }
    }
}