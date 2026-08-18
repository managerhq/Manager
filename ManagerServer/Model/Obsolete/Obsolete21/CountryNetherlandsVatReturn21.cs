using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model.Obsolete.Obsolete21
{
    [ProtoContract]
    [Guid("748804f8-d6a0-492e-a264-4e4646b5d839")]
    internal sealed class CountryNetherlandsVatReturn21 : Object
    {
        [ProtoMember(2)]
        public DateTime From;
        [ProtoMember(3)]
        public DateTime? To;
        [ProtoMember(4)]
        public AccountingBasis AccountingBasis;
        [ProtoMember(5)]
        public string Description;
    }
}
