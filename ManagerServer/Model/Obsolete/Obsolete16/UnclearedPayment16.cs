using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete16
{
    [ProtoContract]
    [Guid("5cabad9a-a31a-4713-9d82-d568d45f471f")]
    internal sealed class UnclearedPayment16 : Object
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
        [ProtoMember(6)]
        public string Payee;
    }
}
