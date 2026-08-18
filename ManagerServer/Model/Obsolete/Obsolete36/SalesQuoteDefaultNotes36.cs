using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete36
{
    [ProtoContract]
    [Guid("66fb2adf-55a5-4d02-a3f6-3b4440ee010c")]
    internal sealed class SalesQuoteDefaultNotes36 : Object
    {
        [ProtoMember(1)]
        public string Value;
    }
}
