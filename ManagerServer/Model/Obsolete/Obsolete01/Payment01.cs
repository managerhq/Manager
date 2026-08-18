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
    [Guid("1abdb7e9-d7d4-4247-bf76-b6516d9b8fd3")]
    internal sealed class Payment01 : Object
    {
        [ProtoMember(3)]
        public string CreditAccount;
        [ProtoMember(4)]
        public DateTime? Date;
        [ProtoMember(5)]
        public string Notes;
        [ProtoMember(6)]
        public PaymentLine[] Lines;
        [ProtoMember(7)]
        public string To;
        [ProtoMember(8)]
        public string Reference;

        [ProtoContract]
        internal sealed class PaymentLine
        {
            [ProtoMember(1)]
            public string DebitAccount;
            [ProtoMember(2)]
            public decimal Amount;
            [ProtoMember(3)]
            public string Tax1;
            [ProtoMember(4)]
            public string Tax2;
            [ProtoMember(5)]
            public string TrackingCode1;
            [ProtoMember(6)]
            public string TrackingCode2;
            [ProtoMember(7)]
            public string TrackingCode3;
        }
    }
}
