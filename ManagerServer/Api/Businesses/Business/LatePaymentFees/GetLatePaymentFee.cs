using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.LatePaymentFees
{
    [ProtoContract]
    internal sealed class GetLatePaymentFee : GetObjectEndpoint<Model.LatePaymentFee>
    {
    }
}
