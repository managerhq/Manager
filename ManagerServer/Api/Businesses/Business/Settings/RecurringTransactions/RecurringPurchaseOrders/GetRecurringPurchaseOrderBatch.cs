namespace ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringPurchaseOrders
{
    [ProtoContract]
    internal sealed class GetRecurringPurchaseOrderBatch : GetObjectBatchEndpoint<Model.RecurringPurchaseOrder, GetRecurringPurchaseOrder, PostRecurringPurchaseOrder, PutRecurringPurchaseOrder, DeleteRecurringPurchaseOrder>
    {
    }
}
