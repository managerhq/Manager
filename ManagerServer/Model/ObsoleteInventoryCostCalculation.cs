using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("6d6d2b53-eb10-461b-90f6-eb2fa0609521")]
    public sealed class ObsoleteInventoryCostCalculation : Object
    {
        [Guide("This setting is obsolete and no longer has any effect on inventory cost calculations.")]
        [ProtoMember(1)] public bool Enabled { get; set; }
    }
}