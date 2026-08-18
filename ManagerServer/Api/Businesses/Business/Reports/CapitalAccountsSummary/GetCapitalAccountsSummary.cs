using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.CapitalAccountsSummary
{
    [ProtoContract]
    internal sealed class GetCapitalAccountsSummary : GetObjectEndpoint<Model.CapitalAccountsSummary>
    {
    }
}
