using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.SpecialAccounts
{
    [ProtoContract]
    internal sealed class GetSpecialAccount : GetObjectEndpoint<Model.SpecialAccount>
    {
    }
}
