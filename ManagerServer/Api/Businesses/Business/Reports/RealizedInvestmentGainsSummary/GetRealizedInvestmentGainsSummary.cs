using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.RealizedInvestmentGainsSummary
{
    [ProtoContract]
    internal sealed class GetRealizedInvestmentGainsSummary : GetObjectEndpoint<Model.RealizedInvestmentGainsSummary>
    {
    }
}
