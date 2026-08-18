namespace ManagerServer.Api.Businesses.Business.Settings.InvestmentMarketPrices
{
    [ProtoContract]
    internal sealed class GetInvestmentMarketPriceBatch : GetObjectBatchEndpoint<Model.InvestmentMarketPrice, GetInvestmentMarketPrice, PostInvestmentMarketPrice, PutInvestmentMarketPrice, DeleteInvestmentMarketPrice>
    {
    }
}
