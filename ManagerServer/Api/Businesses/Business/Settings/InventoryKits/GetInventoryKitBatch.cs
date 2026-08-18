namespace ManagerServer.Api.Businesses.Business.Settings.InventoryKits
{
    [ProtoContract]
    internal sealed class GetInventoryKitBatch : GetObjectBatchEndpoint<Model.InventoryKit, GetInventoryKit, PostInventoryKit, PutInventoryKit, DeleteInventoryKit>
    {
    }
}
