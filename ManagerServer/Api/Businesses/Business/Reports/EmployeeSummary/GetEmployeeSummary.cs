using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.EmployeeSummary
{
    [ProtoContract]
    internal sealed class GetEmployeeSummary : GetObjectEndpoint<Model.EmployeeSummary>
    {
    }
}
