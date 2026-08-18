using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.PurchaseInvoices
{
    [ProtoContract]
    internal sealed class GetPurchaseInvoiceBatch : GetObjectBatchEndpoint<Model.PurchaseInvoice, GetPurchaseInvoice, PostPurchaseInvoice, PutPurchaseInvoice, DeletePurchaseInvoice>
    {
    }
}
