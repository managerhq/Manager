using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CashFlowStatement
{
    [ProtoContract]
    [Title(nameof(Strings.CashFlowStatement), nameof(Strings.ChangesInWorkingCapital), nameof(Strings.Group), nameof(Strings.Transactions))]
    [Guide("The Cash Flow Statement Working Capital Group Transactions screen shows grouped working capital changes.")]
    [Guide("It displays how groups of current assets and liabilities affect cash flow.")]
    internal sealed class CashFlowStatementWorkingCapitalGroupTransactions : TransactionViewer
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
            var output = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();

            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .DisposeFixedAssets()
                .DisposeIntangibleAssets()
                .Revaluate(From, To);

            output.AddRange(generalLedger
                .Where(x => x.GeneralLedgerAccount.GetCashFlowStatementGroup() == CashFlowStatementGroup)
                .Where(x => x.IsCashFlowStatementTransaction || (x.IsInvoiceTransaction && x.IsBalancing))
                .Where(x => x.Date >= From && x.Date <= To));

            output.AddRange(generalLedger
                .Where(x => x.GeneralLedgerAccount.GetCashFlowStatementGroup() == CashFlowStatementGroup)
                .Where(x => x.IsInvoiceTransaction && x.IsBalancing)
                .Where(x => x.Date >= From && x.Date <= To)
                .SelectMany(x => x.ContraTransactions)
                .Where(x => !x.GeneralLedgerAccount.IsProfitAndLossAccount));

            return output;
        }
    }
}