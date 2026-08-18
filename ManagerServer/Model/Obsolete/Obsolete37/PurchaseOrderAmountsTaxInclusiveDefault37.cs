using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete37
{
    [ProtoContract]
    [Guid("7ad7d874-9084-4c7e-a4ee-bd80936d8999")]
    internal sealed class PurchaseOrderAmountsTaxInclusiveDefault37 : Object
    {
        [ProtoMember(1)]
        public bool Value;
    }
}
