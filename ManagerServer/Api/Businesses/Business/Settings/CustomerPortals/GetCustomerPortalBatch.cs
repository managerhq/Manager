namespace ManagerServer.Api.Businesses.Business.Settings.CustomerPortals
{
    [ProtoContract]
    internal sealed class GetCustomerPortalBatch : GetObjectBatchEndpoint<Model.CustomerPortal, GetCustomerPortal, PostCustomerPortal, PutCustomerPortal, DeleteCustomerPortal>
    {
    }
}
