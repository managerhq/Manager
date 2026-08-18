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
    [Guid("01c00313-4790-451e-ae05-1ad6fc6fa476")]
    internal sealed class RetainedEarningsConversionBalance13 : Object
    {
        [ProtoMember(1)]
        public decimal OpeningBalance;
        [ProtoMember(2)]
        public DebitCredit OpeningBalanceType;
    }
}
