namespace ManagerServer.Api.Businesses.Business.Settings.Currencies.ForeignCurrencies
{
    [ProtoContract]
    internal sealed class GetForeignCurrencyBatch : GetObjectBatchEndpoint<Model.ForeignCurrency, GetForeignCurrency, PostForeignCurrency, PutForeignCurrency, DeleteForeignCurrency>
    {
    }
}
