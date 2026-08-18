using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxAudit
{
    [ProtoContract]
    internal sealed class GetTaxAuditBatch : GetObjectBatchEndpoint<Model.TaxAudit, GetTaxAudit, PostTaxAudit, PutTaxAudit, DeleteTaxAudit>
    {
    }
}
