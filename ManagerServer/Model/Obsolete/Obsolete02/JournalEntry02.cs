using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model.Obsolete.Obsolete02
{
    [ProtoContract]
    [Guid("897f5b38-d057-408a-a380-14fc5f5db048")]
    internal sealed class JournalEntry02 : Object
    {
        [ProtoMember(3)]
        public DateTime? Date;
        [ProtoMember(4)]
        public string Reference;
        [ProtoMember(5)]
        public string Narration;
        [ProtoMember(6)]
        public JournalEntryLine[] Lines;
        [ProtoMember(7)]
        public string Notes;
        [ProtoMember(8)]
        public bool IsReversing;
        [ProtoMember(9)]
        public string Batch;
    }

    [ProtoContract]
    internal sealed class JournalEntryLine
    {
        [ProtoMember(1)]
        public Guid? Account;
        [ProtoMember(2)]
        public decimal? Debit;
        [ProtoMember(3)]
        public decimal? Credit;
        [ProtoMember(4)]
        public Guid? Tax;
        [ProtoMember(6)]
        public Guid? TrackingCode1;
        [ProtoMember(7)]
        public Guid? TrackingCode2;
        [ProtoMember(8)]
        public Guid? TrackingCode3;
        [ProtoMember(9)]
        public string Description;
    }
}
