using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete24
{
    [ProtoContract]
    [Guid("02693518-0c86-464a-9c3d-333053885034")]
    internal sealed class SalesInvoiceItem24 : Object
    {
        [ProtoMember(2)]
        public string Name;
        [ProtoMember(3)]
        public string Description;
        [ProtoMember(4)]
        public decimal? UnitPrice;
        [ProtoMember(5)]
        public Guid? AccountID;
        [ProtoMember(6)]
        public Guid? TaxCode;
        [ProtoMember(7)]
        public Dictionary<Guid, string> CustomFields;
        [ProtoMember(8)]
        public Guid? TrackingCode;
        [ProtoMember(10)]
        public bool Inactive;
        [ProtoMember(11)]
        public decimal? Discount;
        [ProtoMember(12)]
        public string Code;

        [ProtoMember(9)]
        public Guid? Obsolete_InventoryItem;
    }
}
