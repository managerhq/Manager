using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.BillableTimeSummary
{
    [ProtoContract]
    internal sealed class GetBillableTimeSummary : GetObjectEndpoint<Model.BillableTimeSummary>
    {
    }
}
