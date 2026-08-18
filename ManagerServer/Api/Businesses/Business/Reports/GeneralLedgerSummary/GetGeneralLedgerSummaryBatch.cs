using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.GeneralLedgerSummary
{
    [ProtoContract]
    internal sealed class GetGeneralLedgerSummaryBatch : GetObjectBatchEndpoint<Model.GeneralLedgerSummary, GetGeneralLedgerSummary, PostGeneralLedgerSummary, PutGeneralLedgerSummary, DeleteGeneralLedgerSummary>
    {
    }
}
