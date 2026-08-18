using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Payments
{
    [ProtoContract]
    internal sealed class GetPayment : GetObjectEndpoint<Model.Payment>
    {
    }
}
