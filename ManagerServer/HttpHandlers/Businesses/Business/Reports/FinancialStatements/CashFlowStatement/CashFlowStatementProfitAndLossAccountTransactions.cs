using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CashFlowStatement
{
    [ProtoContract]
    [Title(nameof(Strings.CashFlowStatement), nameof(Strings.ProfitAndLossStatement), nameof(Strings.Transactions))]
    [Guide("The Cash Flow Statement P&L Account Transactions screen shows non-cash adjustments.")]
    [Guide("It displays specific profit and loss account movements affecting cash flow.")]
    internal sealed class CashFlowStatementProfitAndLossAccountTransactions : TransactionViewer
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
                .Where(x => !x.IsInvoiceTransaction)
                .Where(x => x.GeneralLedgerAccount.CashFlowStatementCategory != CashFlowStatementCategory.OperatingActivities || !x.IsCashFlowStatementTransaction)
                .Where(x => x.Date >= From && x.Date <= To)
                .Where(x => x.BaseAmount != 0m);
        }
    }
}