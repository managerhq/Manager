using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model.Obsolete.Obsolete02
{
    [ProtoContract]
    [Guid("aec22480-da4a-4cba-8dad-a25c8ad2e683")]
    internal sealed class OutOfPocketExpense02 : Object
    {
        [ProtoMember(3)]
        public Guid? CreditAccount;
        [ProtoMember(4)]
        public DateTime? Date;
        [ProtoMember(5)]
        public string Notes;
        [ProtoMember(6)]
        public OutOfPocketExpenseLine[] Lines;
        [ProtoMember(8)]
        public string Reference;
        [ProtoMember(9)]
        public string To;
    }

    [ProtoContract]
    internal sealed class OutOfPocketExpenseLine
    {
        [ProtoMember(1)]
        public Guid? DebitAccount;
        [ProtoMember(2)]
        public decimal? Amount;
        [ProtoMember(3)]
        public Guid? Tax;
        [ProtoMember(4)]
        public string Description;
    }
}
