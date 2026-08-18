using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.ProfitAndLossStatement
{
    [ProtoContract]
    internal sealed class GetProfitAndLossStatement : GetObjectEndpoint<Model.ProfitAndLossStatement>
    {
    }
}
