namespace ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringSalesInvoices
{
    [ProtoContract]
    internal sealed class GetRecurringSalesInvoiceBatch : GetObjectBatchEndpoint<Model.RecurringSalesInvoice, GetRecurringSalesInvoice, PostRecurringSalesInvoice, PutRecurringSalesInvoice, DeleteRecurringSalesInvoice>
    {
    }
}
