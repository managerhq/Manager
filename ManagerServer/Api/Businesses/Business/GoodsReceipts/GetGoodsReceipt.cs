using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.GoodsReceipts
{
    [ProtoContract]
    internal sealed class GetGoodsReceipt : GetObjectEndpoint<Model.GoodsReceipt>
    {
    }
}
