using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.ProfitAndLossStatementActualVsBudget
{
    [ProtoContract]
    internal sealed class GetProfitAndLossStatementActualVsBudgetBatch : GetObjectBatchEndpoint<Model.ProfitAndLossStatementActualVsBudget, GetProfitAndLossStatementActualVsBudget, PostProfitAndLossStatementActualVsBudget, PutProfitAndLossStatementActualVsBudget, DeleteProfitAndLossStatementActualVsBudget>
    {
    }
}
