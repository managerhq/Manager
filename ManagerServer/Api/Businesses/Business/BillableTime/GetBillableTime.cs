using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.BillableTime
{
    [ProtoContract]
    internal sealed class GetBillableTime : GetObjectEndpoint<Model.BillableTime>
    {
    }
}
