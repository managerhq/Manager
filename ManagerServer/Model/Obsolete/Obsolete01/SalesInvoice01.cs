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
    [Guid("96def4fb-d88d-4a40-b2a9-f6b90b7df378")]
    internal sealed class SalesInvoice01 : Object
    {
        [ProtoMember(3)]
        public DateTime? IssueDate;
        [ProtoMember(4)]
        public string Reference;
        [ProtoMember(5)]
        public string To;
        [ProtoMember(6)]
        public string BillingAddress;
        [ProtoMember(7)]
        public SalesInvoiceLine[] Lines;
        [ProtoMember(8)]
        public DateTime? DueDate;
        [ProtoMember(9)]
        public string Notes;

        /*
        [ProtoMember(10)]
        public string TradingName;
        [ProtoMember(11)]
        public string ContactInformation;
         */

        [ProtoMember(12)]
        public bool AmountsIncludeTax;

        /*
        [ProtoMember(13)]
        public string HowToPay;
         */

        [ProtoMember(14)]
        public string ControlAccount;

        /*
        [ProtoMember(15)]
        public string ABN;
        [ProtoMember(16)]
        public bool HidePaymentSlip;
         */

        [ProtoContract]
        internal sealed class SalesInvoiceLine
        {
            [ProtoMember(1)]
            public string Item;
            [ProtoMember(2)]
            public string Description;
            [ProtoMember(3)]
            public decimal? Amount;
            [ProtoMember(4)]
            public string Tax1;
            [ProtoMember(5)]
            public string Tax2;
            [ProtoMember(6)]
            public string TrackingCode1;
            [ProtoMember(7)]
            public string TrackingCode2;
            [ProtoMember(8)]
            public string TrackingCode3;
            [ProtoMember(9)]
            public decimal? Qty;
            [ProtoMember(10)]
            public decimal? UnitPrice;
        }
    }
}
