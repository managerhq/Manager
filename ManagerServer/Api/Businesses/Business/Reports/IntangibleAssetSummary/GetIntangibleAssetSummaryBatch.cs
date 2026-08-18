using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.IntangibleAssetSummary
{
    [ProtoContract]
    internal sealed class GetIntangibleAssetSummaryBatch : GetObjectBatchEndpoint<Model.IntangibleAssetSummary, GetIntangibleAssetSummary, PostIntangibleAssetSummary, PutIntangibleAssetSummary, DeleteIntangibleAssetSummary>
    {
    }
}
