using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.SalesQuotes
{
    [ProtoContract]
    internal sealed class GetSalesQuoteBatch : GetObjectBatchEndpoint<Model.SalesQuote, GetSalesQuote, PostSalesQuote, PutSalesQuote, DeleteSalesQuote>
    {
    }
}
