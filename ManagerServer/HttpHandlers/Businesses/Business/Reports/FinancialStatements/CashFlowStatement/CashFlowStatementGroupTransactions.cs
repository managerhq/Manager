using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CashFlowStatement
{
    [ProtoContract]
    [Title(nameof(Strings.CashFlowStatement), nameof(Strings.Group), nameof(Strings.Transactions))]
    [Guide("The Cash Flow Statement Group Transactions screen shows cash movements for account groups.")]
    [Guide("It displays transactions grouped by operating, investing, or financing activities.")]
    internal sealed class CashFlowStatementGroupTransactions : TransactionViewer
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;
        [ProtoMember(3)] public Guid CashFlowStatementGroup;

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
                .Where(x => x.GeneralLedgerAccount.GetCashFlowStatementGroup() == CashFlowStatementGroup)
                .Where(x => x.IsCashFlowStatementTransaction)
                .Where(x => x.Transaction is not SalesInvoice && x.Transaction is not PurchaseInvoice && x.Transaction is not CreditNote && x.Transaction is not DebitNote)
                .Where(x => x.Date >= From && x.Date <= To)
                .Where(x => x.BaseAmount != 0m);
        }
    }
}