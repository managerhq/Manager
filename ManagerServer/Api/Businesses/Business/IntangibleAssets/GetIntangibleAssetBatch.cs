using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.IntangibleAssets
{
    [ProtoContract]
    internal sealed class GetIntangibleAssetBatch : GetObjectBatchEndpoint<Model.IntangibleAsset, GetIntangibleAsset, PostIntangibleAsset, PutIntangibleAsset, DeleteIntangibleAsset>
    {
    }
}
