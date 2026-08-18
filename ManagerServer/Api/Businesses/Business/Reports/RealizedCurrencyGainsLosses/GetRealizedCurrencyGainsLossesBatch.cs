using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.RealizedCurrencyGainsLosses
{
    [ProtoContract]
    internal sealed class GetRealizedCurrencyGainsLossesBatch : GetObjectBatchEndpoint<Model.RealizedCurrencyGainsLosses, GetRealizedCurrencyGainsLosses, PostRealizedCurrencyGainsLosses, PutRealizedCurrencyGainsLosses, DeleteRealizedCurrencyGainsLosses>
    {
    }
}
