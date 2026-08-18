using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Investments
{
    [ProtoContract]
    internal sealed class GetInvestment : GetObjectEndpoint<Model.Investment>
    {
    }
}
