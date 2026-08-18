using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete07
{
    [ProtoContract]
    [Guid("21840c59-e26f-4caa-8b91-81a885b08a30")]
    internal sealed class CashReceipt07 : Object
    {
        [ProtoMember(1)]
        public DateTime Date;
        [ProtoMember(2)]
        public Guid? CashAccount;
        [ProtoMember(3)]
        public string Description;
        [ProtoMember(4)]
        public Obsolete.Obsolete76.TransactionLine[] Lines;
        [ProtoMember(5)]
        public string Reference;
        [ProtoMember(6)]
        public string Payer;
        [ProtoMember(7)]
        public decimal India_TaxDeductedAtSource;
    }
}
