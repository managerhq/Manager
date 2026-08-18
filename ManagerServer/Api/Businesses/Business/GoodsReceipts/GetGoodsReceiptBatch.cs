using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.GoodsReceipts
{
    [ProtoContract]
    internal sealed class GetGoodsReceiptBatch : GetObjectBatchEndpoint<Model.GoodsReceipt, GetGoodsReceipt, PostGoodsReceipt, PutGoodsReceipt, DeleteGoodsReceipt>
    {
    }
}
