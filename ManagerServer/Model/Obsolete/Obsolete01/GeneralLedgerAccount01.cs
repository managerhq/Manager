using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete01
{
    [ProtoContract]
    [Guid("ea82d3a4-ab0c-4733-a56f-a49688a56275")]
    internal sealed class GeneralLedgerAccount01 : Object
    {
        [ProtoMember(3)]
        public string Name;
        [ProtoMember(5)]
        public ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18 Category;
    }
}
