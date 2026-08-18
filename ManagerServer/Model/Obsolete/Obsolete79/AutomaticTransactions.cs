using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;

namespace ManagerServer.Model.Obsolete.Obsolete79
{
    [ProtoContract]
    [Guid("6F5AC0D9-EC74-4D5E-A9D8-74162F2CE040")]
    public sealed class AutomaticTransactions : Object
    {
        [ProtoMember(1)] public bool AutomaticInvestmentRevaluations;
        [ProtoMember(2)] public bool AutomaticForexRevaluations;
    }
}
