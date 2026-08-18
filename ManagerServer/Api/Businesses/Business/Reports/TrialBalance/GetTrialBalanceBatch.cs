using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TrialBalance
{
    [ProtoContract]
    internal sealed class GetTrialBalanceBatch : GetObjectBatchEndpoint<Model.TrialBalance, GetTrialBalance, PostTrialBalance, PutTrialBalance, DeleteTrialBalance>
    {
    }
}
