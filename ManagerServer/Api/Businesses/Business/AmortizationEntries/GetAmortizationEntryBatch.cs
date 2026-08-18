using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.AmortizationEntries
{
    [ProtoContract]
    internal sealed class GetAmortizationEntryBatch : GetObjectBatchEndpoint<Model.AmortizationEntry, GetAmortizationEntry, PostAmortizationEntry, PutAmortizationEntry, DeleteAmortizationEntry>
    {
    }
}
