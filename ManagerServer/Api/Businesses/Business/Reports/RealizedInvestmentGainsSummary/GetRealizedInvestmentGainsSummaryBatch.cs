using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.RealizedInvestmentGainsSummary
{
    [ProtoContract]
    internal sealed class GetRealizedInvestmentGainsSummaryBatch : GetObjectBatchEndpoint<Model.RealizedInvestmentGainsSummary, GetRealizedInvestmentGainsSummary, PostRealizedInvestmentGainsSummary, PutRealizedInvestmentGainsSummary, DeleteRealizedInvestmentGainsSummary>
    {
    }
}
