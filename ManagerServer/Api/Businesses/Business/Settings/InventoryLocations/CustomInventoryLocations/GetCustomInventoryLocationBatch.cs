namespace ManagerServer.Api.Businesses.Business.Settings.InventoryLocations.CustomInventoryLocations
{
    [ProtoContract]
    internal sealed class GetCustomInventoryLocationBatch : GetObjectBatchEndpoint<Model.CustomInventoryLocation, GetCustomInventoryLocation, PostCustomInventoryLocation, PutCustomInventoryLocation, DeleteCustomInventoryLocation>
    {
    }
}
