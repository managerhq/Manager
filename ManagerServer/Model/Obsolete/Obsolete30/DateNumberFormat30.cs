using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete30
{
    [ProtoContract]
    [Guid("beee8855-c7e8-4568-8c10-9146972c2ce3")]
    internal sealed class DateNumberFormat30 : Object
    {
        [ProtoMember(1)]
        public string Culture;
        [ProtoMember(2)]
        public bool ISO8601;
    }
}
