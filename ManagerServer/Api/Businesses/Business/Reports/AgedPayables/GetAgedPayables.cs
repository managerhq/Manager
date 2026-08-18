using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.AgedPayables
{
    [ProtoContract]
    internal sealed class GetAgedPayables : GetObjectEndpoint<Model.AgedPayables>
    {
    }
}
