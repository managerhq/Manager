using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Forecasts
{
    [ProtoContract]
    [Title(nameof(Strings.Forecast))]
    [Guide("Create financial forecasts to project future income and expenses.")]
    [Guide("Forecasts help with budgeting and planning future business activities.")]
    [Fields(typeof(ManagerServer.Model.Forecast))]
    internal sealed class ForecastForm : NakedVueForm<ManagerServer.Model.Forecast>
    {
    }
}
