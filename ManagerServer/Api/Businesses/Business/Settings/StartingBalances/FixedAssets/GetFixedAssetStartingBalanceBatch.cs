namespace ManagerServer.Api.Businesses.Business.Settings.StartingBalances.FixedAssets
{
    [ProtoContract]
    internal sealed class GetFixedAssetStartingBalanceBatch : GetObjectBatchEndpoint<Model.FixedAssetStartingBalance, GetFixedAssetStartingBalance, PostFixedAssetStartingBalance, PutFixedAssetStartingBalance, DeleteFixedAssetStartingBalance>
    {
    }
}
