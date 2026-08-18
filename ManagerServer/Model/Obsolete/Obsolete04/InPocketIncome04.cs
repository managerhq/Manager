using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete04
{
    [ProtoContract]
    [Guid("452bdb74-8541-4bc8-80bc-ecc5c591b5d9")]
    internal sealed class InPocketIncome04 : Object
    {
        [ProtoMember(1)]
        public DateTime Date;
        [ProtoMember(2)]
        public string From;
        [ProtoMember(3)]
        public Guid? To;
        [ProtoMember(4)]
        public Obsolete.Obsolete76.TransactionLine[] Lines;
        [ProtoMember(5)]
        public string Notes;
    }
}
