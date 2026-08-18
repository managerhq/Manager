namespace ManagerServer.Api.Businesses.Business.Settings.Forecasts
{
    [ProtoContract]
    internal sealed class GetForecastBatch : GetObjectBatchEndpoint<Model.Forecast, GetForecast, PostForecast, PutForecast, DeleteForecast>
    {
    }
}
