namespace ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringPayslips
{
    [ProtoContract]
    internal sealed class GetRecurringPayslipBatch : GetObjectBatchEndpoint<Model.RecurringPayslip, GetRecurringPayslip, PostRecurringPayslip, PutRecurringPayslip, DeleteRecurringPayslip>
    {
    }
}
