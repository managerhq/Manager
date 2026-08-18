using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.ExpenseClaims
{
    [ProtoContract]
    internal sealed class GetExpenseClaimBatch : GetObjectBatchEndpoint<Model.ExpenseClaim, GetExpenseClaim, PostExpenseClaim, PutExpenseClaim, DeleteExpenseClaim>
    {
    }
}
