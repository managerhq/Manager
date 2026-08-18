using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model.Obsolete.Obsolete18
{
    [ProtoContract]
    [Guid("877159c9-e587-430c-bbfd-ea3db132fb66")]
    internal sealed class MultiStepIncomeStatementGroup18 : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public int? Position;
        [ProtoMember(4)]
        public Guid? MultiStepIncomeStatementTotal;
        [ProtoMember(5)]
        public bool IsExpense;

        [ProtoMember(3)]
        public string Obsolete_Total;
    }
}
