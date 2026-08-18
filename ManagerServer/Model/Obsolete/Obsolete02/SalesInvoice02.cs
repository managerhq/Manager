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
    [Guid("644a4013-689a-4709-a8d1-aa57c969222e")]
    internal sealed class SalesInvoice02 : Object
    {
        [ProtoMember(1)]
        public DateTime? IssueDate;
        [ProtoMember(2)]
        public string Reference;
        [ProtoMember(3)]
        public Guid? To;
        [ProtoMember(4)]
        public string BillingAddress;
        [ProtoMember(5)]
        public SalesInvoiceLine[] Lines;
        [ProtoMember(6)]
        public DateTime? DueDate;
        [ProtoMember(7)]
        public string Notes;
        [ProtoMember(8)]
        public bool AmountsIncludeTax;
    }

    [ProtoContract]
    internal sealed class SalesInvoiceLine
    {
        [ProtoMember(1)]
        public Guid? Account;
        [ProtoMember(2)]
        public string Description;
        [ProtoMember(4)]
        public Guid? Tax;
        [ProtoMember(6)]
        public Guid? TrackingCode1;
        [ProtoMember(7)]
        public Guid? TrackingCode2;
        [ProtoMember(8)]
        public Guid? TrackingCode3;
        [ProtoMember(9)]
        public decimal? Qty;
        [ProtoMember(10)]
        public decimal UnitPrice;
        [ProtoMember(11)]
        public Guid? Item;
        [ProtoMember(3)]
        public decimal? Obsolete_Amount;
    }
}
