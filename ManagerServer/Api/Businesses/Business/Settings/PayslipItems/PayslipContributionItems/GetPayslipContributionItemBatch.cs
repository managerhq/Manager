namespace ManagerServer.Api.Businesses.Business.Settings.PayslipItems.PayslipContributionItems
{
    [ProtoContract]
    internal sealed class GetPayslipContributionItemBatch : GetObjectBatchEndpoint<Model.PayslipContributionItem, GetPayslipContributionItem, PostPayslipContributionItem, PutPayslipContributionItem, DeletePayslipContributionItem>
    {
    }
}
