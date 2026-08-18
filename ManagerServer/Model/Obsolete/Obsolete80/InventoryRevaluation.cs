using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;

namespace ManagerServer.Model.Obsolete.Obsolete80
{
    [ProtoContract]
    [Guid("7248ce93-de18-41e8-b566-3fb4bd7623a8")]
    public sealed class InventoryRevaluation
    {
        [ProtoMember(1)] public DateTime Date;
        [ProtoMember(2)] public Line[] Lines;

        [ProtoContract]
        public sealed class Line
        {
            [ProtoMember(1), Autocomplete(typeof(InventoryItem))] public Guid? InventoryItem;
            [ProtoMember(3), AppendBaseCurrency, Short, Sum] public decimal Amount;
        }
    }
}
