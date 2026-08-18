namespace ManagerServer.Api.Businesses.Business.Settings.Currencies.ExchangeRates
{
    [ProtoContract]
    internal sealed class GetExchangeRateBatch : GetObjectBatchEndpoint<Model.ExchangeRate, GetExchangeRate, PostExchangeRate, PutExchangeRate, DeleteExchangeRate>
    {
    }
}
