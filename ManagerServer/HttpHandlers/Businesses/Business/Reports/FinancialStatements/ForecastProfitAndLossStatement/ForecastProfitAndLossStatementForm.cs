using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ForecastProfitAndLossStatement
{
    [ProtoContract]
    [Title(nameof(Strings.ForecastProfitAndLossStatement))]
    [Guide("The Forecast Profit & Loss Statement form configures parameters for forecast reports.")]
    [Guide("Set date ranges and forecasts to project financial performance.")]
    [Fields(typeof(ManagerServer.Model.ForecastProfitAndLossStatement))]
    internal sealed class ForecastProfitAndLossStatementForm : NakedVueForm<ManagerServer.Model.ForecastProfitAndLossStatement>
    {
    }
}
