using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.CustomerSummary
{
    [ProtoContract]
    internal sealed class GetCustomerSummary : GetObjectEndpoint<Model.CustomerSummary>
    {
    }
}
