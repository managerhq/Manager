using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.PayslipSummary
{
    [ProtoContract]
    internal sealed class GetPayslipSummary : GetObjectEndpoint<Model.PayslipSummary>
    {
    }
}
