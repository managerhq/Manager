namespace ManagerServer.Api.Businesses.Business.Settings.Footers.Payments
{
    [ProtoContract]
    internal sealed class GetPaymentFooterBatch : GetObjectBatchEndpoint<Model.PaymentFooter, GetPaymentFooter, PostPaymentFooter, PutPaymentFooter, DeletePaymentFooter>
    {
    }
}
