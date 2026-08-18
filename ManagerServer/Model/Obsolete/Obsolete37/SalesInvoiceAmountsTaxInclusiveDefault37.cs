using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete37
{
    [ProtoContract]
    [Guid("061a37e0-5d8b-4d76-b149-c3df6401611a")]
    internal sealed class SalesInvoiceAmountsTaxInclusiveDefault37 : Object
    {
        [ProtoMember(1)]
        public bool Value;
    }
}
