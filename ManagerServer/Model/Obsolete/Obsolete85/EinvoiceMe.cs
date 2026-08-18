using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model.Obsolete.Obsolete85
{
    [ProtoContract]
    [Guid("12e0885e-65c0-4a5c-85d6-98b4b3865518")]
    public sealed class EinvoiceMe : Object
    {
        [ProtoMember(3)] public bool Enabled;
        [ProtoMember(1), IfTrue(nameof(Enabled)), Prepend("Authorization: Bearer"), NoLabel] public string Authorization;
        [ProtoMember(2), IfTrue(nameof(Enabled)), Prepend("After ID"), NoLabel] public int? LastSync;
    }
}