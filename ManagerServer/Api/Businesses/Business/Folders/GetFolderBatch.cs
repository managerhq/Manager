using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Folders
{
    [ProtoContract]
    internal sealed class GetFolderBatch : GetObjectBatchEndpoint<Model.Folder, GetFolder, PostFolder, PutFolder, DeleteFolder>
    {
    }
}
