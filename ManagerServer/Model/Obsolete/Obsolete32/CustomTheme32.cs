using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete32
{
    [ProtoContract]
    [Guid("517a0621-ae92-4104-b9fe-c90563da34e1")]
    internal sealed class CustomTheme32 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public string Definition;
    }
}
