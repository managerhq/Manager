using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CashFlowStatement
{
    [ProtoContract]
    [Title(nameof(Strings.CashFlowStatement), nameof(Strings.Transactions))]
    [Guide("The Cash Flow Statement Account Transactions screen shows cash movements for specific accounts.")]
    [Guide("It displays transactions that affect cash flow within the reporting period.")]
    internal sealed class CashFlowStatementAccountTransactions : TransactionViewer
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;
        [ProtoMember(3)] public Guid Account;

        protected override bool ShowBaseAmount()
        {
            return true;
        }

        protected override bool MultipleByOne()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .DisposeFixedAssets()
                .DisposeIntangibleAssets()
                .Revaluate(From, To)
                .Where(x => x.GeneralLedgerAccount.Key == Account)
                .Where(x => x.IsCashFlowStatementTransaction)
                .Where(x => x.Transaction is not SalesInvoice && x.Transaction is not PurchaseInvoice && x.Transaction is not CreditNote && x.Transaction is not DebitNote)
                .Where(x => x.Date >= From && x.Date <= To)
                .Where(x => x.BaseAmount != 0m);
        }
    }
}