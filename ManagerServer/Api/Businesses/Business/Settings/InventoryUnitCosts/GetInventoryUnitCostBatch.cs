namespace ManagerServer.Api.Businesses.Business.Settings.InventoryUnitCosts
{
    [ProtoContract]
    internal sealed class GetInventoryUnitCostBatch : GetObjectBatchEndpoint<Model.InventoryUnitCost, GetInventoryUnitCost, PostInventoryUnitCost, PutInventoryUnitCost, DeleteInventoryUnitCost>
    {
    }
}
