using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model.Obsolete.Obsolete29
{
    [ProtoContract]
    [Guid("b79737d6-d925-4813-96b2-72bd3ba69a05")]
    internal sealed class CountryUnitedKingdomVatFlatRateTaxCode29 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public decimal VatRate;
        [ProtoMember(3)]
        public decimal FlatRate;
    }
}
