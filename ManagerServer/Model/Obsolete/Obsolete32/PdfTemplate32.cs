using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete32
{
    [ProtoContract]
    [Guid("81a959a7-67f7-4139-b2f0-890e1f1bd2de")]
    internal sealed class PdfTemplate32 : Object
    {
        [ProtoMember(1)]
        public Guid Value;

        [ProtoMember(2)]
        public string Obsolete_Template;
    }
}
