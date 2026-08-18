using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete84
{
    [ProtoContract]
    [Guid("24bc5086-8ac4-42e0-9af6-d97242f75209")]
    public sealed class UnrealizedInvestmentGainsSummary : Object
    {
        [ProtoMember(1)] public string Description;
        [ProtoMember(2)] public DateTime FromDate;
        [ProtoMember(3)] public DateTime ToDate;
    }
}
