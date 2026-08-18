namespace ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringReceipts
{
    [ProtoContract]
    internal sealed class GetRecurringReceiptBatch : GetObjectBatchEndpoint<Model.RecurringReceipt, GetRecurringReceipt, PostRecurringReceipt, PutRecurringReceipt, DeleteRecurringReceipt>
    {
    }
}
