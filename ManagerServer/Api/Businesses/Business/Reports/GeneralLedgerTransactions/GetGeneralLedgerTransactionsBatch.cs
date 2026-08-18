using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.GeneralLedgerTransactions
{
    [ProtoContract]
    internal sealed class GetGeneralLedgerTransactionsBatch : GetObjectBatchEndpoint<Model.GeneralLedgerTransactions, GetGeneralLedgerTransactions, PostGeneralLedgerTransactions, PutGeneralLedgerTransactions, DeleteGeneralLedgerTransactions>
    {
    }
}
