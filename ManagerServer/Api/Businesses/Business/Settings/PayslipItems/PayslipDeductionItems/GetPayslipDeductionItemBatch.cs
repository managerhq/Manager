namespace ManagerServer.Api.Businesses.Business.Settings.PayslipItems.PayslipDeductionItems
{
    [ProtoContract]
    internal sealed class GetPayslipDeductionItemBatch : GetObjectBatchEndpoint<Model.PayslipDeductionItem, GetPayslipDeductionItem, PostPayslipDeductionItem, PutPayslipDeductionItem, DeletePayslipDeductionItem>
    {
    }
}
