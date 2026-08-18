using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete01
{
    [ProtoContract]
    [Guid("65339404-ed1d-4168-a65f-ce15a0adac30")]
    internal sealed class JournalEntry01 : Object
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
        [ProtoMember(10)]
        public bool CashBasis;

        [ProtoContract]
        internal sealed class JournalEntryLine
        {
            [ProtoMember(1)]
            public string Account;
            [ProtoMember(2)]
            public decimal? Debit;
            [ProtoMember(3)]
            public decimal? Credit;
            [ProtoMember(4)]
            public string Tax1;
            [ProtoMember(5)]
            public string Tax2;
            [ProtoMember(6)]
            public string TrackingCode1;
            [ProtoMember(7)]
            public string TrackingCode2;
            [ProtoMember(8)]
            public string TrackingCode3;
        }
    }
}
