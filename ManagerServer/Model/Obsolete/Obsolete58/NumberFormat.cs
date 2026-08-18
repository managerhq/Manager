using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete58
{
    [ProtoContract]
    [Guid("7bf18ff5-d0a5-4413-8268-dbc463316c57")]
    public sealed class NumberFormat : Object
    {
        [ProtoMember(1)]
        public string DecimalSeparator;
        [ProtoMember(2)]
        public string GroupSeparator;
        [ProtoMember(3)]
        public int[] GroupSizes;
    }
}
