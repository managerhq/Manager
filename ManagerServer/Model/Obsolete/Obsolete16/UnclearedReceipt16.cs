using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete16
{
    [ProtoContract]
    [Guid("0db83cbb-95cc-47a2-9016-71ddbe3da223")]
    internal sealed class UnclearedReceipt16 : Object
    {
        [ProtoMember(1)]
        public DateTime Date;
        [ProtoMember(2)]
        public Guid? BankAccount;
        [ProtoMember(3)]
        public string Description;
        [ProtoMember(4)]
        public Obsolete.Obsolete76.TransactionLine[] Lines;
        [ProtoMember(5)]
        public string Reference;
    }
}
