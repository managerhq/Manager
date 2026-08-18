using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.DepreciationEntries
{
    [ProtoContract]
    internal sealed class GetDepreciationEntryBatch : GetObjectBatchEndpoint<Model.DepreciationEntry, GetDepreciationEntry, PostDepreciationEntry, PutDepreciationEntry, DeleteDepreciationEntry>
    {
    }
}
