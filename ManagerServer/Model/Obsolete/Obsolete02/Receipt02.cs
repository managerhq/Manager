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
    [Guid("0da2a4c5-645d-4595-a211-65d018eb8e64")]
    internal sealed class Receipt02 : Object
    {
        [ProtoMember(3)]
        public Guid? DebitAccount;
        [ProtoMember(4)]
        public DateTime? Date;
        [ProtoMember(5)]
        public string Notes;
        [ProtoMember(6)]
        public ReceiptLine[] Lines;
        [ProtoMember(7)]
        public Guid? Obsolete_From;
        [ProtoMember(8)]
        public string Reference;
        [ProtoMember(9)]
        public string From;
        [ProtoMember(10)]
        public Obsolete.Obsolete76.TransactionLine[] Lines2;
    }

    [ProtoContract]
    internal sealed class ReceiptLine
    {
        [ProtoMember(1)]
        public Guid? CreditAccount;
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
