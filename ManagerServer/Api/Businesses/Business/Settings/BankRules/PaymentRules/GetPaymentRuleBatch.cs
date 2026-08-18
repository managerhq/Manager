namespace ManagerServer.Api.Businesses.Business.Settings.BankRules.PaymentRules
{
    [ProtoContract]
    internal sealed class GetPaymentRuleBatch : GetObjectBatchEndpoint<Model.PaymentRule, GetPaymentRule, PostPaymentRule, PutPaymentRule, DeletePaymentRule>
    {
    }
}
