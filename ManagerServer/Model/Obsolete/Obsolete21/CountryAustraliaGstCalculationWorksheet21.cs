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
    [Guid("1f89eb6a-de29-4697-ae53-b5eda52560f7")]
    internal sealed class CountryAustraliaGstCalculationWorksheet21 : Object
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
