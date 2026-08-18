using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.InterAccountTransfers
{
    [ProtoContract]
    internal sealed class GetInterAccountTransfer : GetObjectEndpoint<Model.InterAccountTransfer>
    {
    }
}
