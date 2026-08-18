using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.StatementOfChangesInEquity
{
    [ProtoContract]
    internal sealed class GetStatementOfChangesInEquityBatch : GetObjectBatchEndpoint<Model.StatementOfChangesInEquity, GetStatementOfChangesInEquity, PostStatementOfChangesInEquity, PutStatementOfChangesInEquity, DeleteStatementOfChangesInEquity>
    {
    }
}
