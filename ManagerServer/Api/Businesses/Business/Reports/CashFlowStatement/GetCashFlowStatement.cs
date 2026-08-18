using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.CashFlowStatement
{
    [ProtoContract]
    internal sealed class GetCashFlowStatement : GetObjectEndpoint<Model.CashFlowStatement>
    {
    }
}
