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
    [Guid("3f768b6a-37f7-469a-9fd0-e7ce39d693e3")]
    internal sealed class Receipt01 : Object
    {
        [ProtoMember(3)]
        public string DebitAccount;
        [ProtoMember(4)]
        public DateTime? Date;
        [ProtoMember(5)]
        public string Notes;
        [ProtoMember(6)]
        public ReceiptLine[] Lines;
        [ProtoMember(7)]
        public string From;
        [ProtoMember(8)]
        public string Reference;

        [ProtoContract]
        internal sealed class ReceiptLine
        {
            [ProtoMember(1)]
            public string CreditAccount;
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
