namespace ManagerServer.Api.Businesses.Business.Settings.BankRules.ReceiptRules
{
    [ProtoContract]
    internal sealed class GetReceiptRuleBatch : GetObjectBatchEndpoint<Model.ReceiptRule, GetReceiptRule, PostReceiptRule, PutReceiptRule, DeleteReceiptRule>
    {
    }
}
