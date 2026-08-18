using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;
using System.Linq;
using ManagerServer.Globalization;

namespace ManagerServer.Model.Obsolete.Obsolete87
{
    [ProtoContract]
    [Guid("13cefbaa-8f29-4c41-b53d-d0daf1837b54")]
    public sealed class InventoryRevaluation : ManagerServer.Model.Object
    {
        [ProtoMember(1)] public DateTime Date;
        [ProtoMember(5)] public string Description;
        [ProtoMember(4)] public Line[] Lines;

        [ProtoContract]
        public sealed class Line
        {
            [ProtoMember(1), Autocomplete(typeof(InventoryItem))] public Guid? InventoryItem;
            [ProtoMember(2), AppendBaseCurrency] public decimal AverageCost;
        }

        [ProtoMember(2)] public Guid? Obsolete_InventoryItem;
        [ProtoMember(3)] public decimal Obsolete_AverageCost;
    }
}