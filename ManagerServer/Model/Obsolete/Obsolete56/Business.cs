using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete56
{
    [ProtoContract]
    [Guid("3dd0b8c5-72a0-4257-8857-557bed47a59d")]
    public sealed class Business : Object
    {
        [ProtoMember(1)]
        public string Name;
    }
}
