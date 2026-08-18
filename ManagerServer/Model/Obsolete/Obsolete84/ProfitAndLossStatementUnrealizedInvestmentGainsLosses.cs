using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete84
{
    [ProtoContract]
    [Guid("11955564-c54a-4ca4-9bac-ac7a518654b2")]
    [Singleton]
    public sealed class ProfitAndLossStatementUnrealizedInvestmentGainsLosses : Object
    {
        [ProtoMember(1)] public string Name;
        [ProtoMember(11)] public string Code;
        [ProtoMember(3)] public Guid? Group;
        [ProtoMember(10)] public int Position;
    }
}
