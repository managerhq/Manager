using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.AgedReceivables
{
    [ProtoContract]
    internal sealed class GetAgedReceivablesBatch : GetObjectBatchEndpoint<Model.AgedReceivables, GetAgedReceivables, PostAgedReceivables, PutAgedReceivables, DeleteAgedReceivables>
    {
    }
}
