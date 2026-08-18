using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete43
{
    [ProtoContract]
    [Guid("ce976453-4e11-4457-875c-361e44edb7d2")]
    internal sealed class CashReceipt43 : Object
    {
        [ProtoMember(1)]
        public DateTime? Date;
        [ProtoMember(2)]
        public Guid? CashAccount;
        [ProtoMember(3)]
        public string Description;
        [ProtoMember(4)]
        public Obsolete.Obsolete76.TransactionLine[] Lines;
        [ProtoMember(5)]
        public string Reference;
        [ProtoMember(6)]
        public string Payer;
        [ProtoMember(8)]
        public Dictionary<Guid, string> CustomFields;             
        [ProtoMember(14)]
        public Guid? InventoryLocation;
        [ProtoMember(16)]
        public bool AmountsIncludeTax;
        [ProtoMember(17)]
        public bool CustomTheme;
        [ProtoMember(18)]
        public Guid? Theme;

        [ProtoMember(15)]
        internal ManagerServer.Model.Obsolete.Obsolete33.Receipt33 Obsolete_Receipt;
        [ProtoMember(10)]
        public string Obsolete_Notes;
    }
}
