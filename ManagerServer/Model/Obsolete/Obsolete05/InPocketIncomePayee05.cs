using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete05
{
    [ProtoContract]
    [Guid("12b8e712-a7a7-4510-9022-d1f565652625")]
    internal sealed class InPocketIncomePayee05 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public Guid? Account;
    }
}
