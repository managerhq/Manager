using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.LatePaymentFees
{
    [ProtoContract]
    internal sealed class GetLatePaymentFeeBatch : GetObjectBatchEndpoint<Model.LatePaymentFee, GetLatePaymentFee, PostLatePaymentFee, PutLatePaymentFee, DeleteLatePaymentFee>
    {
    }
}
