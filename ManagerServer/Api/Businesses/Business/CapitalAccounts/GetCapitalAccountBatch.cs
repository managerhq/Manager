using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.CapitalAccounts
{
    [ProtoContract]
    internal sealed class GetCapitalAccountBatch : GetObjectBatchEndpoint<Model.CapitalAccount, GetCapitalAccount, PostCapitalAccount, PutCapitalAccount, DeleteCapitalAccount>
    {
    }
}
