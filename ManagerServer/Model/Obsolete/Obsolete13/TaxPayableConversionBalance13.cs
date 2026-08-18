using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model.Obsolete.Obsolete13
{
    [ProtoContract]
    [Guid("89b898c8-d5f1-4cff-9a56-93120a92c89e")]
    internal sealed class TaxPayableConversionBalance13 : Object
    {
        [ProtoMember(1)]
        public decimal OpeningBalance;
        [ProtoMember(2)]
        public DebitCredit OpeningBalanceType;
    }
}
