namespace ManagerServer.Api.Businesses.Business.Settings.ExpenseClaimPayers
{
    [ProtoContract]
    internal sealed class GetExpenseClaimsPayerBatch : GetObjectBatchEndpoint<Model.ExpenseClaimsPayer, GetExpenseClaimsPayer, PostExpenseClaimsPayer, PutExpenseClaimsPayer, DeleteExpenseClaimsPayer>
    {
    }
}
