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
    [Guid("a2adda4e-4767-4cb2-a452-8f5507070e69")]
    internal sealed class CashPayment43 : Object
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
        public string Payee;
        [ProtoMember(7)]
        public Dictionary<Guid, string> CustomFields;        
        [ProtoMember(13)]
        public Guid? InventoryLocation;
        [ProtoMember(15)]
        public bool AmountsIncludeTax;
        [ProtoMember(16)]
        public bool CustomTheme;
        [ProtoMember(17)]
        public Guid? Theme;

        [ProtoMember(14)]
        internal ManagerServer.Model.Obsolete.Obsolete33.Payment33 Obsolete_Payment;
        [ProtoMember(9)]
        public string Obsolete_Notes;
    }
}
