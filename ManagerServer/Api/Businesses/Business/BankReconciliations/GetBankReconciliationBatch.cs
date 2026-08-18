using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.BankReconciliations
{
    [ProtoContract]
    internal sealed class GetBankReconciliationBatch : GetObjectBatchEndpoint<Model.BankReconciliation, GetBankReconciliation, PostBankReconciliation, PutBankReconciliation, DeleteBankReconciliation>
    {
    }
}
