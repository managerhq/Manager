namespace ManagerServer.Api.Businesses.Business.Settings.CapitalSubaccounts
{
    [ProtoContract]
    internal sealed class GetSubAccountBatch : GetObjectBatchEndpoint<Model.SubAccount, GetSubAccount, PostSubAccount, PutSubAccount, DeleteSubAccount>
    {
    }
}
