using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete02
{
    [ProtoContract]
    [Guid("633f63e8-d52a-4bad-b752-9e81f78ad480")]
    internal sealed class Associate02 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public string AccountName;
        [ProtoMember(3)]
        public ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18? Category;
    }
}
