using ManagerServer.Api.Businesses.Business.Settings.Forecasts;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using System;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Forecasts
{
    [ProtoContract]
    [Title(nameof(Strings.Forecast), nameof(Strings.View))]
    [Guide("This screen displays the details of a *forecast* including all projected values and calculations.")]
    [Guide("Use this view to review your *forecast assumptions* and see how they impact future financial projections.")]
    [Guide("The forecast shows projected figures based on the parameters you have set up, helping you plan and make informed business decisions.")]
    [LinkGuide("To edit this forecast, see:", typeof(ForecastForm))]
    internal sealed class ForecastView : DefaultView<GetForecastView>
    {
    }
}
