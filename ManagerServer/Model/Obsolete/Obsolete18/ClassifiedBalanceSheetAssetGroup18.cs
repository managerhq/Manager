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
    [Guid("44364250-7db1-4fc7-bca5-da641e5cb125")]
    internal sealed class ClassifiedBalanceSheetAssetGroup18 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public int? Position;
    }
}
