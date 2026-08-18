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
    [Guid("ade4affa-d566-4e69-97a7-8a986422f950")]
    internal sealed class ClassifiedBalanceSheetLiabilityGroup18 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public int? Position;
    }
}
