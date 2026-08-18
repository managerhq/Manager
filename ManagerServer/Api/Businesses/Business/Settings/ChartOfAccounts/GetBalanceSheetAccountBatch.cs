namespace ManagerServer.Api.Businesses.Business.Settings.ChartOfAccounts
{
    [ProtoContract]
    internal sealed class GetBalanceSheetAccountBatch : GetObjectBatchEndpoint<Model.BalanceSheetAccount, GetBalanceSheetAccount, PostBalanceSheetAccount, PutBalanceSheetAccount, DeleteBalanceSheetAccount>
    {
    }
}
