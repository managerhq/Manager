namespace ManagerServer.Api.Businesses.Business.Settings.UserPermissions
{
    [ProtoContract]
    internal sealed class GetUserPermissionsBatch : GetObjectBatchEndpoint<Model.UserPermissions, GetUserPermissions, PostUserPermissions, PutUserPermissions, DeleteUserPermissions>
    {
    }
}
