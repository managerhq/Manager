using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.TrialBalance
{
    [ProtoContract]
    internal sealed class GetTrialBalance : GetObjectEndpoint<Model.TrialBalance>
    {
    }
}
