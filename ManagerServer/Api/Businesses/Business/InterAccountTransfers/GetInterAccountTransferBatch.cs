using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.InterAccountTransfers
{
    [ProtoContract]
    internal sealed class GetInterAccountTransferBatch : GetObjectBatchEndpoint<Model.InterAccountTransfer, GetInterAccountTransfer, PostInterAccountTransfer, PutInterAccountTransfer, DeleteInterAccountTransfer>
    {
    }
}
