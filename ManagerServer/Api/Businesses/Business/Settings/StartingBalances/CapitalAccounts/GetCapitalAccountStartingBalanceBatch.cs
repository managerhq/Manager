namespace ManagerServer.Api.Businesses.Business.Settings.StartingBalances.CapitalAccounts
{
    [ProtoContract]
    internal sealed class GetCapitalAccountStartingBalanceBatch : GetObjectBatchEndpoint<Model.CapitalAccountStartingBalance, GetCapitalAccountStartingBalance, PostCapitalAccountStartingBalance, PutCapitalAccountStartingBalance, DeleteCapitalAccountStartingBalance>
    {
    }
}
