using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete58
{
    [ProtoContract]
    [Guid("b05acce7-06ba-414c-a88c-b2cf9881569e")]
    public sealed class Language : Object
    {
        [ProtoMember(1)]
        public string Value;
    }
}
