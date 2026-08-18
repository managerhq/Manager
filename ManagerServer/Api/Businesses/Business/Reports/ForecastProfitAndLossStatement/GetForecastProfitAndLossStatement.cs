using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.ForecastProfitAndLossStatement
{
    [ProtoContract]
    internal sealed class GetForecastProfitAndLossStatement : GetObjectEndpoint<Model.ForecastProfitAndLossStatement>
    {
    }
}
