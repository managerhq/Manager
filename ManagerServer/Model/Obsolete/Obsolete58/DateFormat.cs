using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete58
{
    [ProtoContract]
    [Guid("9514eb71-8e8b-4d91-b58b-76a5ce7e21d4")]
    public sealed class DateFormat : Object
    {
        [ProtoMember(1)]
        public string ShortDatePattern;
    }
}
