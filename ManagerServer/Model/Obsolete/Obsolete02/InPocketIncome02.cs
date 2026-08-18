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
    [Guid("2d6b61ac-9f35-4243-952e-8189af08cc0d")]
    internal sealed class InPocketIncome02 : Object
    {
        [ProtoMember(3)]
        public Guid? DebitAccount;
        [ProtoMember(4)]
        public DateTime? Date;
        [ProtoMember(5)]
        public string Notes;
        [ProtoMember(6)]
        public InPocketIncomeLine[] Lines;
        [ProtoMember(8)]
        public string Reference;
        [ProtoMember(9)]
        public string From;
    }

    [ProtoContract]
    internal sealed class InPocketIncomeLine
    {
        [ProtoMember(1)]
        public Guid? CreditAccount;
        [ProtoMember(2)]
        public decimal? Amount;
        [ProtoMember(3)]
        public Guid? Tax;
        [ProtoMember(4)]
        public string Description;
    }
}
