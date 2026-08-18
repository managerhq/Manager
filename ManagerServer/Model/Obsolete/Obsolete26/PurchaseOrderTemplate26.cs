using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete26
{
    [ProtoContract]
    [Guid("2f777546-9a69-44ec-90bf-56c38563b100")]
    internal sealed class PurchaseOrderTemplate26 : Object
    {
        [ProtoMember(2)]
        public string ReferenceNumberPrefix;

        [ProtoMember(1)]
        public bool Obsolete_AmountsIncludeTax;
    }
}
