using System.Linq;
using ManagerServer;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerTransactionsForSubaccountTransactions : BaseGeneralLedgerTransactionsForSubaccount
    {
        [InheritedProtoMember(300)] public Guid? Subaccount;

        protected override void InnerGet4(Context context)
        {
            if (Subaccount.HasValue)
            {
                var rows = GetGeneralLedgerTransactions()
                    .Where(x => x.SubAccount?.Key == Subaccount.Value && x.AccountAmount != 0m)
                    .OrderByDescending(x => x.Date)
                    .ToArray();
                context.Set<Array>(rows);
            }

            base.InnerGet4(context);
        }
    }
}