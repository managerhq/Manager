using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete14
{
    [ProtoContract]
    [Guid("8d36efb6-e17e-499c-974a-712db5968324")]
    internal sealed class TransactionExchangeRate14 : Object
    {
        [ProtoMember(1)]
        public Guid Transaction;
        [ProtoMember(2)]
        public Guid Account;
        [ProtoMember(3)]
        public decimal ExchangeRate;
    }
}
