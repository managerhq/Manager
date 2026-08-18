using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.CustomerSummary
{
    [ProtoContract]
    internal sealed class GetCustomerSummaryBatch : GetObjectBatchEndpoint<Model.CustomerSummary, GetCustomerSummary, PostCustomerSummary, PutCustomerSummary, DeleteCustomerSummary>
    {
    }
}
