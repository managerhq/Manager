using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model.Obsolete.Obsolete33
{
    [ProtoContract]
    [Guid("cb120a14-83cf-46e9-b05f-6725342310ad")]
    internal sealed class Payment33 : Object
    {
        [ProtoMember(1)]
        public DateTime Date;
        [ProtoMember(2)]
        public Guid? CreditAccount;
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
        [ProtoMember(9)]
        public string Notes;
        [ProtoMember(10)]
        public DateTime? BankClearDate;
        [ProtoMember(11)]
        public BankClearStatus BankClearStatus;
        [ProtoMember(13)]
        public Guid? InventoryLocation;

        [ProtoMember(8)]
        public ManagerServer.Model.JournalEntry Obsolete_JournalEntry;
        [ProtoMember(12)]
        public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines;
    }
}
