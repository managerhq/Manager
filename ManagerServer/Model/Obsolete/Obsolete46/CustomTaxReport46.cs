using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete46
{
    [ProtoContract]
    [Guid("15f9edf9-53bd-4432-8dbf-dcaf425e57c4")]
    internal sealed class CustomTaxReport46 : Object
    {
        [ProtoMember(1)]
        public Guid Type;
        [ProtoMember(2)]
        public DateTime From;
        [ProtoMember(3)]
        public DateTime To;
        [ProtoMember(4)]
        public AccountingBasis AccountingBasis;
        [ProtoMember(5)]
        public string Description;
    }
}
