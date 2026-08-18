using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete42
{
    [ProtoContract]
    [Guid("ee6594b8-b0f9-4f02-9cbf-94418d6270e0")]
    internal sealed class BankReceipt42 : Object
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
        public string Payer;
        [ProtoMember(8)]
        public Dictionary<Guid, string> CustomFields;        
        [ProtoMember(11)]
        public DateTime? BankClearDate;
        [ProtoMember(12)]
        public BankClearStatus BankClearStatus;
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
