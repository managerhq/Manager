using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete40
{
    [ProtoContract]
    [Guid("69e98f0a-fa50-4185-b06d-e32edfb06771")]
    internal sealed class OutOfPocketExpensePayor40 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public Guid? Account;
    }
}
