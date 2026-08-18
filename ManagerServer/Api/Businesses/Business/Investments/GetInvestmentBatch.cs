using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Investments
{
    [ProtoContract]
    internal sealed class GetInvestmentBatch : GetObjectBatchEndpoint<Model.Investment, GetInvestment, PostInvestment, PutInvestment, DeleteInvestment>
    {
    }
}
