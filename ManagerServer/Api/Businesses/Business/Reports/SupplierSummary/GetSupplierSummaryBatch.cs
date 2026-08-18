using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.SupplierSummary
{
    [ProtoContract]
    internal sealed class GetSupplierSummaryBatch : GetObjectBatchEndpoint<Model.SupplierSummary, GetSupplierSummary, PostSupplierSummary, PutSupplierSummary, DeleteSupplierSummary>
    {
    }
}
