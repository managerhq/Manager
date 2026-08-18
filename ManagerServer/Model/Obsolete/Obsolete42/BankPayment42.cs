using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model.Obsolete.Obsolete42
{
    [ProtoContract]
    [Guid("091e11d8-29dd-4207-8cd5-ee5b169b4560")]
    internal sealed class BankPayment42 : Object
    {
        [ProtoMember(1)]
        public DateTime? Date;
        [ProtoMember(2)]
        public Guid? BankAccount;
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
        [ProtoMember(10)]
        public DateTime? BankClearDate;
        [ProtoMember(11)]
        public BankClearStatus BankClearStatus;
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
