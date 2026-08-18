using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.WithholdingTaxReceipts
{
    [ProtoContract]
    internal sealed class GetWithholdingTaxReceiptBatch : GetObjectBatchEndpoint<Model.WithholdingTaxReceipt, GetWithholdingTaxReceipt, PostWithholdingTaxReceipt, PutWithholdingTaxReceipt, DeleteWithholdingTaxReceipt>
    {
    }
}
