using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.AgedPayables
{
    [ProtoContract]
    internal sealed class GetAgedPayablesBatch : GetObjectBatchEndpoint<Model.AgedPayables, GetAgedPayables, PostAgedPayables, PutAgedPayables, DeleteAgedPayables>
    {
    }
}
