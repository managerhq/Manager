using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxTransactions
{
    [ProtoContract]
    internal sealed class GetTaxTransactionsBatch : GetObjectBatchEndpoint<Model.TaxTransactions, GetTaxTransactions, PostTaxTransactions, PutTaxTransactions, DeleteTaxTransactions>
    {
    }
}
