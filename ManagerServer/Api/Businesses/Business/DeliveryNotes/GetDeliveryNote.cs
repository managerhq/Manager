using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.DeliveryNotes
{
    [ProtoContract]
    internal sealed class GetDeliveryNote : GetObjectEndpoint<Model.DeliveryNote>
    {
    }
}
