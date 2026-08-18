namespace ManagerServer.Api.Businesses.Business.Settings.Footers.PurchaseOrders
{
    [ProtoContract]
    internal sealed class GetPurchaseOrderFooterBatch : GetObjectBatchEndpoint<Model.PurchaseOrderFooter, GetPurchaseOrderFooter, PostPurchaseOrderFooter, PutPurchaseOrderFooter, DeletePurchaseOrderFooter>
    {
    }
}
