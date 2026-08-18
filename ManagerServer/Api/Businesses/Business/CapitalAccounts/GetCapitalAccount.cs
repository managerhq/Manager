using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.CapitalAccounts
{
    [ProtoContract]
    internal sealed class GetCapitalAccount : GetObjectEndpoint<Model.CapitalAccount>
    {
    }
}
