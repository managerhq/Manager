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
    [Guid("3d93d7d9-f99b-44cc-a0ef-01f06607e872")]
    internal sealed class PurchaseInvoice02 : Object
    {
        [ProtoMember(3)]
        public DateTime? IssueDate;
        [ProtoMember(4)]
        public string Reference;
        [ProtoMember(5)]
        public Guid? From;
        [ProtoMember(6)]
        public PurchaseInvoiceLine[] Lines;
        [ProtoMember(7)]
        public DateTime? DueDate;
        [ProtoMember(8)]
        public string Notes;
    }

    [ProtoContract]
    internal sealed class PurchaseInvoiceLine
    {
        [ProtoMember(1)]
        public Guid? Account;
        [ProtoMember(3)]
        public Guid? Tax;
        [ProtoMember(5)]
        public Guid? TrackingCode1;
        [ProtoMember(6)]
        public Guid? TrackingCode2;
        [ProtoMember(7)]
        public Guid? TrackingCode3;
        [ProtoMember(8)]
        public decimal? Qty;
        [ProtoMember(9)]
        public decimal UnitPrice;
        [ProtoMember(10)]
        public Guid? Item;
        [ProtoMember(2)]
        public decimal? Obsolete_Amount;
    }
}
