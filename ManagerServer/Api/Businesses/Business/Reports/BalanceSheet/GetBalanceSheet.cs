using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.BalanceSheet
{
    [ProtoContract]
    internal sealed class GetBalanceSheet : GetObjectEndpoint<Model.BalanceSheet>
    {
    }
}
