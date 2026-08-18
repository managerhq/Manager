using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete13
{
    [ProtoContract]
    [Guid("910bd356-43fc-456f-915e-4ac3615c0ead")]
    internal sealed class CapitalControlAccount13 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18? Category;
    }
}
