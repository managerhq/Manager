using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete24
{
    [ProtoContract]
    [Guid("4872d932-30bc-4f46-a27d-96a93a224789")]
    internal sealed class PurchaseInvoiceItem24 : Object
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
