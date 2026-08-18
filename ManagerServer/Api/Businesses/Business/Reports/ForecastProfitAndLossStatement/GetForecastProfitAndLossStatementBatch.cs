using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.ForecastProfitAndLossStatement
{
    [ProtoContract]
    internal sealed class GetForecastProfitAndLossStatementBatch : GetObjectBatchEndpoint<Model.ForecastProfitAndLossStatement, GetForecastProfitAndLossStatement, PostForecastProfitAndLossStatement, PutForecastProfitAndLossStatement, DeleteForecastProfitAndLossStatement>
    {
    }
}
