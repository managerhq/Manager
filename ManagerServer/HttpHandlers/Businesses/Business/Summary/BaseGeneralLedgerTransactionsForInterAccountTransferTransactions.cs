using System.Linq;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    internal abstract class BaseGeneralLedgerTransactionsForInterAccountTransferTransactions : BaseGeneralLedgerTransactionsForInterAccountTransfers
    {
        [InheritedProtoMember(310)] public Tuple<Guid, Guid> InterAccountTransferPair;

        protected override void InnerGet4(Context context)
        {
            if (InterAccountTransferPair != null)
            {
                var rows = GetGeneralLedgerTransactions()
                    .Where(x => x.InterAccountTransferPair?.Item1.Key == InterAccountTransferPair.Item1)
                    .Where(x => x.InterAccountTransferPair?.Item2.Key == InterAccountTransferPair.Item2)
                    .OrderByDescending(x => x.Date)
                    .ToArray();

                context.Set<Array>(rows);
            }

            base.InnerGet4(context);
        }
    }
}