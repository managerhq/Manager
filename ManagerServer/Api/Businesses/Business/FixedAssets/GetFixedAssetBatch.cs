using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.FixedAssets
{
    [ProtoContract]
    internal sealed class GetFixedAssetBatch : GetObjectBatchEndpoint<Model.FixedAsset, GetFixedAsset, PostFixedAsset, PutFixedAsset, DeleteFixedAsset>
    {
    }
}
