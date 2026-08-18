using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model.Obsolete.Obsolete18
{
    [ProtoContract]
    [Guid("9a8a03cc-14a4-46ed-8875-dc0e0fc9f3b2")]
    internal sealed class MultiStepIncomeStatementTotal18 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public int? Position;
    }
}
