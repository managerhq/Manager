namespace ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringPurchaseInvoices
{
    [ProtoContract]
    internal sealed class GetRecurringPurchaseInvoiceBatch : GetObjectBatchEndpoint<Model.RecurringPurchaseInvoice, GetRecurringPurchaseInvoice, PostRecurringPurchaseInvoice, PutRecurringPurchaseInvoice, DeleteRecurringPurchaseInvoice>
    {
    }
}
