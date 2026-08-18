using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.FixedAssetSummary
{
    [ProtoContract]
    internal sealed class GetFixedAssetSummaryBatch : GetObjectBatchEndpoint<Model.FixedAssetSummary, GetFixedAssetSummary, PostFixedAssetSummary, PutFixedAssetSummary, DeleteFixedAssetSummary>
    {
    }
}
