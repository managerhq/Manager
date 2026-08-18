namespace ManagerServer.Api.Businesses.Business.Settings.NonInventoryItems
{
    [ProtoContract]
    internal sealed class GetNonInventoryItemBatch : GetObjectBatchEndpoint<Model.NonInventoryItem, GetNonInventoryItem, PostNonInventoryItem, PutNonInventoryItem, DeleteNonInventoryItem>
    {
    }
}
