using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete25
{
    [ProtoContract]
    [Guid("3e2e733d-f34d-4b8d-b4ee-3f2e012a92e8")]
    internal sealed class SalesInvoiceItem25 : Object
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
