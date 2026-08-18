namespace ManagerServer.Api.Businesses.Business.Settings.PayslipItems.PayslipEarningsItems
{
    [ProtoContract]
    internal sealed class GetPayslipEarningsItemBatch : GetObjectBatchEndpoint<Model.PayslipEarningsItem, GetPayslipEarningsItem, PostPayslipEarningsItem, PutPayslipEarningsItem, DeletePayslipEarningsItem>
    {
    }
}
