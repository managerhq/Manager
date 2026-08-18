using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxReconciliation
{
    [ProtoContract]
    internal sealed class GetTaxReconciliationBatch : GetObjectBatchEndpoint<Model.TaxReconciliation, GetTaxReconciliation, PostTaxReconciliation, PutTaxReconciliation, DeleteTaxReconciliation>
    {
    }
}
