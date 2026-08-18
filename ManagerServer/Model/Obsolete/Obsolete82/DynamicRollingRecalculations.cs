using ProtoBuf;
using System;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model.Obsolete.Obsolete82
{
    [ProtoContract]
    [Guid("dc7cee2e-2db4-4c63-9c49-5447d9f242a4")]
    public sealed class DynamicRollingRecalculations : Object
    {
        [ProtoMember(2)] public bool ForeignCurrencies;

        [ProtoMember(3)] public bool Obsolete_Investments;
        [ProtoMember(1)] public bool Obsolete_InventoryItems;
    }
}
