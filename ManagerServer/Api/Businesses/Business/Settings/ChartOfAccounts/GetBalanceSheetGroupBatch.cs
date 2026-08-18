namespace ManagerServer.Api.Businesses.Business.Settings.ChartOfAccounts
{
    [ProtoContract]
    internal sealed class GetBalanceSheetGroupBatch : GetObjectBatchEndpoint<Model.BalanceSheetGroup, GetBalanceSheetGroup, PostBalanceSheetGroup, PutBalanceSheetGroup, DeleteBalanceSheetGroup>
    {
    }
}
