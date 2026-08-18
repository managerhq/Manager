using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete23
{
    [ProtoContract]
    [Guid("0ad8c1f6-d2dc-4d8f-83f2-583c162fb352")]
    internal sealed class ControlAccountForCashAccounts23 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public int? Code;
        [ProtoMember(3)]
        public Guid? Group;
    }
}
