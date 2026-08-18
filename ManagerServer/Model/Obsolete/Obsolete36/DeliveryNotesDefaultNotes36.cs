using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete36
{
    [ProtoContract]
    [Guid("28b482b6-c22e-4333-a9e8-ad727d057f4b")]
    internal sealed class DeliveryNotesDefaultNotes36 : Object
    {
        [ProtoMember(1)]
        public string Value;
    }
}
