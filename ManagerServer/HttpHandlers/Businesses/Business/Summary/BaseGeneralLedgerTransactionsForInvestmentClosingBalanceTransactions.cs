using System.Linq;
using ManagerServer;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerTransactionsForInvestmentClosingBalanceTransactions : BaseGeneralLedgerTransactionsForInventoryItemTransactions
    {
        [InheritedProtoMember(360)] public Guid? ClosingBalanceInvestment;

        protected override void InnerGet4(Context context)
        {
            if (ClosingBalanceInvestment.HasValue)
            {
                var rows = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                    .Revaluate(GetRoot().From.Value.SafeAddDays(-1))
                    .Where(x => x.SubAccount?.Key == ClosingBalanceInvestment.Value)
                    .Where(x => x.Date <= To)
                    .OrderByDescending(x => x.Date)
                    .ThenByDescending(x => x.Transaction == null)
                    .ToArray();

                context.Set<Array>(rows);
            }

            base.InnerGet4(context);
        }
    }
}