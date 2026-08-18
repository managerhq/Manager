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
    [Guid("acf26524-c96a-45fc-a2da-456f38ecab42")]
    internal sealed class PurchaseInvoice01 : Object
    {
        [ProtoMember(3)]
        public DateTime? IssueDate;
        [ProtoMember(4)]
        public string Reference;
        [ProtoMember(5)]
        public string From;
        [ProtoMember(6)]
        public PurchaseInvoiceLine[] Lines;
        [ProtoMember(7)]
        public DateTime? DueDate;
        [ProtoMember(8)]
        public string Notes;
        [ProtoMember(9)]
        public string ControlAccount;

        [ProtoContract]
        internal sealed class PurchaseInvoiceLine
        {
            [ProtoMember(1)]
            public string Item;
            [ProtoMember(2)]
            public decimal? Amount;
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
