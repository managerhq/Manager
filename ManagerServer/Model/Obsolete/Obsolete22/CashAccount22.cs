using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete22
{
    [ProtoContract]
    [Guid("6ef63462-17e4-40f0-8428-dee773825dec")]
    internal sealed class CashAccount22 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public decimal StartingBalance;
        [ProtoMember(4)]
        public Guid? Currency;
        [ProtoMember(5)]
        public bool HasStartingBalance;

        [ProtoMember(3)]
        public DateTime Obsolete_StartingBalanceDate;
    }
}
