namespace ManagerServer.Api.Businesses.Business.Settings.StartingBalances.SpecialAccounts
{
    [ProtoContract]
    internal sealed class GetSpecialAccountStartingBalanceBatch : GetObjectBatchEndpoint<Model.SpecialAccountStartingBalance, GetSpecialAccountStartingBalance, PostSpecialAccountStartingBalance, PutSpecialAccountStartingBalance, DeleteSpecialAccountStartingBalance>
    {
    }
}
