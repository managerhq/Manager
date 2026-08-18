using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete37
{
    [ProtoContract]
    [Guid("888da62a-d33c-42f6-b15e-39c151306135")]
    internal sealed class SalesQuoteDefaultHeading37 : Object
    {
        [ProtoMember(1)]
        public string Value;
    }
}
