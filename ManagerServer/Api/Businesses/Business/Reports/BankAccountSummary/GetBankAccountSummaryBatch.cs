using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.BankAccountSummary
{
    [ProtoContract]
    internal sealed class GetBankAccountSummaryBatch : GetObjectBatchEndpoint<Model.BankAccountSummary, GetBankAccountSummary, PostBankAccountSummary, PutBankAccountSummary, DeleteBankAccountSummary>
    {
    }
}
