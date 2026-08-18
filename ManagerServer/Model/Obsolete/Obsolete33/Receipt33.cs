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
    [Guid("8a995f93-a7a7-4297-a3b6-35a339a5ae0d")]
    internal sealed class Receipt33 : Object
    {
        [ProtoMember(1)]
        public DateTime Date;
        [ProtoMember(2)]
        public Guid? DebitAccount;
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
        [ProtoMember(10)]
        public string Notes;
        [ProtoMember(11)]
        public DateTime? BankClearDate;
        [ProtoMember(12)]
        public BankClearStatus BankClearStatus;
        [ProtoMember(14)]
        public Guid? InventoryLocation;

        [ProtoMember(7)]
        public decimal Obsolete_India_TaxDeductedAtSource;
        [ProtoMember(9)]
        public ManagerServer.Model.JournalEntry Obsolete_JournalEntry;
        [ProtoMember(13)]
        public Obsolete.Obsolete76.TransactionLine[] Obsolete_Lines;
    }
}
