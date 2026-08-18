using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Receipts
{
    [ProtoContract]
    internal sealed class GetReceipt : GetObjectEndpoint<Model.Receipt>
    {
    }
}
