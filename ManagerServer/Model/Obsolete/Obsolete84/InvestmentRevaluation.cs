using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;

namespace ManagerServer.Model.Obsolete.Obsolete84
{
    [ProtoContract]
    [CustomFields]
    [Guid("A7DCF4FB-8D5C-4A71-A9B1-812FFDD753E9")]
    public sealed class InvestmentRevaluation : Object
    {
        [ProtoMember(1)] public DateTime Date;
        [ProtoMember(4)] public string Reference;
        [ProtoMember(2)] public string Description;
        [ProtoMember(3)] public Line[] Lines;
        [ProtoMember(6)] public Dictionary<Guid, string> CustomFields;
        [ProtoMember(7)] public CustomFields CustomFields2;
        [ProtoMember(5)] public bool AutomaticReference;

        [ProtoMember(8)] public bool Obsolete_UnrealizedGains;
        [ProtoMember(9)] public decimal Obsolete_UnrealizedGainsAmount;

        [ProtoContract]
        public sealed class Line
        {
            [ProtoMember(1)] public Guid? Investment;
            [ProtoMember(3)] public decimal UnrealizedGains;
        }
    }
}
