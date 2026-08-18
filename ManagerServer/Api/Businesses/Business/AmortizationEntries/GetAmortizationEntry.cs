using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.AmortizationEntries
{
    [ProtoContract]
    internal sealed class GetAmortizationEntry : GetObjectEndpoint<Model.AmortizationEntry>
    {
    }
}
