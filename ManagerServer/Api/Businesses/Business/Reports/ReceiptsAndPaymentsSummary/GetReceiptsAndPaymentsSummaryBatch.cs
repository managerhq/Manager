using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.ReceiptsAndPaymentsSummary
{
    [ProtoContract]
    internal sealed class GetReceiptsAndPaymentsSummaryBatch : GetObjectBatchEndpoint<Model.ReceiptsAndPaymentsSummary, GetReceiptsAndPaymentsSummary, PostReceiptsAndPaymentsSummary, PutReceiptsAndPaymentsSummary, DeleteReceiptsAndPaymentsSummary>
    {
    }
}
