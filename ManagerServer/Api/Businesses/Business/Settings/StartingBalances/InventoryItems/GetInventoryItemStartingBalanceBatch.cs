namespace ManagerServer.Api.Businesses.Business.Settings.StartingBalances.InventoryItems
{
    [ProtoContract]
    internal sealed class GetInventoryItemStartingBalanceBatch : GetObjectBatchEndpoint<Model.InventoryItemStartingBalance, GetInventoryItemStartingBalance, PostInventoryItemStartingBalance, PutInventoryItemStartingBalance, DeleteInventoryItemStartingBalance>
    {
    }
}
