using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete32
{
    [ProtoContract]
    [Guid("955304b5-effc-491b-818c-959499448a4d")]
    internal sealed class WindowFacedEnvelope32 : Object
    {
        [ProtoMember(1)]
        public int? CustomerAddressHorizontalPadding;
        [ProtoMember(2)]
        public int? CustomerAddressVerticalPadding;
    }
}
