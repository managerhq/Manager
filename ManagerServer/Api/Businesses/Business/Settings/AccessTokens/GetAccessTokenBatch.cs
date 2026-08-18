namespace ManagerServer.Api.Businesses.Business.Settings.AccessTokens
{
    [ProtoContract]
    internal sealed class GetAccessTokenBatch : GetObjectBatchEndpoint<Model.AccessToken, GetAccessToken, PostAccessToken, PutAccessToken, DeleteAccessToken>
    {
    }
}
