using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.CashFlowStatement
{
    [ProtoContract]
    internal sealed class GetCashFlowStatementBatch : GetObjectBatchEndpoint<Model.CashFlowStatement, GetCashFlowStatement, PostCashFlowStatement, PutCashFlowStatement, DeleteCashFlowStatement>
    {
    }
}
