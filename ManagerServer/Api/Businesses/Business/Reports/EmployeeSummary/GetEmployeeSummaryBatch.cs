using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.EmployeeSummary
{
    [ProtoContract]
    internal sealed class GetEmployeeSummaryBatch : GetObjectBatchEndpoint<Model.EmployeeSummary, GetEmployeeSummary, PostEmployeeSummary, PutEmployeeSummary, DeleteEmployeeSummary>
    {
    }
}
