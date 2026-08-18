/*
#if DEBUG
using Manager.Model.Enums;
using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CashFlowStatement
{
    [ProtoContract]
    internal sealed class CashFlowStatementCashBalances : BusinessTemplate
    {
        [ProtoMember(1)] public DateTime Date;
        protected override void InnerGet2()
        {
            var generalLedger2 = new Manager.Query.GeneralLedger.GeneralLedger(FileID)
                    .Revaluate(Date)
                    .GroupBy(x => x.GeneralLedgerAccount)
                    .Where(x => x.Key.CashFlowStatementCategory != CashFlowStatementCategory.OperatingActivities)
                    .Where(x => x.Key.CashFlowStatementCategory != CashFlowStatementCategory.FinancingActivities)
                    .Where(x => x.Key.CashFlowStatementCategory != CashFlowStatementCategory.InvestingActivities)
                    .ToArray();

            foreach (var e in generalLedger2)
            {
                Write(e.Key.Key.ToString());
                Write(e.Key.Name);
                Br();
            }
        }
    }
}
#endif
*/