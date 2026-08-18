using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.AgedReceivables
{
    [ProtoContract]
    internal sealed class GetAgedReceivables : GetObjectEndpoint<Model.AgedReceivables>
    {
    }
}
