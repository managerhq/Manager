using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.ExpenseClaimsSummary
{
    [ProtoContract]
    internal sealed class GetExpenseClaimsSummaryBatch : GetObjectBatchEndpoint<Model.ExpenseClaimsSummary, GetExpenseClaimsSummary, PostExpenseClaimsSummary, PutExpenseClaimsSummary, DeleteExpenseClaimsSummary>
    {
    }
}
