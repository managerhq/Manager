using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.PurchaseQuotes
{
    [ProtoContract]
    internal sealed class GetPurchaseQuoteBatch : GetObjectBatchEndpoint<Model.PurchaseQuote, GetPurchaseQuote, PostPurchaseQuote, PutPurchaseQuote, DeletePurchaseQuote>
    {
    }
}
