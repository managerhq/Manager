using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Projects
{
    [ProtoContract]
    internal sealed class GetProjectBatch : GetObjectBatchEndpoint<Model.Project, GetProject, PostProject, PutProject, DeleteProject>
    {
    }
}
