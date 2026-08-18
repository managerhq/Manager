namespace ManagerServer.Api.Businesses.Business.Settings.StartingBalances.Investments
{
    [ProtoContract]
    internal sealed class GetInvestmentStartingBalanceBatch : GetObjectBatchEndpoint<Model.InvestmentStartingBalance, GetInvestmentStartingBalance, PostInvestmentStartingBalance, PutInvestmentStartingBalance, DeleteInvestmentStartingBalance>
    {
    }
}
