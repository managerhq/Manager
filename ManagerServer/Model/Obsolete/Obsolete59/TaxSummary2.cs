using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete59
{
    [ProtoContract]
    [Guid("c26047fb-f59a-42ee-9a5a-26a471c094a6")]
    public sealed class TaxSummary2 : Object
    {
        [ProtoMember(1)]
        public DateTime From;
        [ProtoMember(2)]
        public DateTime? To;
        [ProtoMember(3)]
        public AccountingBasis AccountingBasis;
        [ProtoMember(4)]
        public string Description;
    }
}
