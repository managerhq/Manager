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
    [Guid("19115fb2-b4fb-45d8-9a42-730eb6fca288")]
    internal sealed class Payment02 : Object
    {
        [ProtoMember(3)]
        public Guid? CreditAccount;
        [ProtoMember(4)]
        public DateTime? Date;
        [ProtoMember(5)]
        public string Notes;
        [ProtoMember(6)]
        public PaymentLine[] Lines;
        [ProtoMember(8)]
        public string Reference;
        [ProtoMember(9)]
        public string To;
        [ProtoMember(7)]
        public Guid? Obsolete_To;
    }

    [ProtoContract]
    internal sealed class PaymentLine
    {
        [ProtoMember(1)]
        public Guid? DebitAccount;
        [ProtoMember(2)]
        public decimal? Amount;
        [ProtoMember(3)]
        public Guid? Tax;
        [ProtoMember(5)]
        public Guid? TrackingCode1;
        [ProtoMember(6)]
        public Guid? TrackingCode2;
        [ProtoMember(7)]
        public Guid? TrackingCode3;
        [ProtoMember(8)]
        public string Description;
    }
}
