using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.ProfitAndLossStatement
{
    [ProtoContract]
    internal sealed class GetProfitAndLossStatementBatch : GetObjectBatchEndpoint<Model.ProfitAndLossStatement, GetProfitAndLossStatement, PostProfitAndLossStatement, PutProfitAndLossStatement, DeleteProfitAndLossStatement>
    {
    }
}
