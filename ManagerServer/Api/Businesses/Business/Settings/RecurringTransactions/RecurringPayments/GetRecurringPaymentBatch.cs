namespace ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringPayments
{
    [ProtoContract]
    internal sealed class GetRecurringPaymentBatch : GetObjectBatchEndpoint<Model.RecurringPayment, GetRecurringPayment, PostRecurringPayment, PutRecurringPayment, DeleteRecurringPayment>
    {
    }
}
