using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete25
{
    [ProtoContract]
    [Guid("a5c544dd-579f-42e5-9039-1e1f3cc60376")]
    internal sealed class PurchaseInvoiceItem25 : Object
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
        public string Code;

        [ProtoMember(9)]
        public Guid? Obsolete_InventoryItem;
    }
}
