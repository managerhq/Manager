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
    [Guid("44f9f8bd-2938-40b8-93e9-52d5055ef96b")]
    internal sealed class OutOfPocketExpense04 : Object
    {
        [ProtoMember(1)]
        public DateTime Date;
        [ProtoMember(2)]
        public Guid? From;
        [ProtoMember(3)]
        public string To;
        [ProtoMember(4)]
        public Obsolete.Obsolete76.TransactionLine[] Lines;
        [ProtoMember(5)]
        public string Notes;
    }
}
