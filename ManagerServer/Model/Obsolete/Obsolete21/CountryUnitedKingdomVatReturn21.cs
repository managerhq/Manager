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
    [Guid("b0be5c01-8c5c-47c2-a763-c729f0645cd3")]
    internal sealed class CountryUnitedKingdomVatReturn21 : Object
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
