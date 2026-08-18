using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.SpecialAccounts
{
    [ProtoContract]
    internal sealed class GetSpecialAccountBatch : GetObjectBatchEndpoint<Model.SpecialAccount, GetSpecialAccount, PostSpecialAccount, PutSpecialAccount, DeleteSpecialAccount>
    {
    }
}
