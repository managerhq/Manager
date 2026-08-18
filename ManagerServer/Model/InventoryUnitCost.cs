using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("d5d7bad5-0abd-4501-af7f-cb6289cabc30")]
    public sealed class InventoryUnitCost : ManagerServer.Model.Object, IComparable<InventoryUnitCost>
    {
        [Guide("Select the date from which this unit cost should apply. The system will use this cost for calculations from this date forward until a newer cost is entered.")]
        [ProtoMember(1)] public DateTime Date { get; set; }
        [Guide("Select the inventory item for which you want to set or adjust the unit cost.")]
        [ProtoMember(2), Autocomplete(typeof(InventoryItem))] public Guid? InventoryItem { get; set; }
        [Guide("Enter the unit cost for this inventory item. This cost will be used for inventory valuation and cost of goods sold calculations.")]
        [ProtoMember(3), AppendBaseCurrency] public decimal UnitCost { get; set; }

        int IComparable<InventoryUnitCost>.CompareTo(InventoryUnitCost other)
        {
            return (InventoryItem, Date).CompareTo((other.InventoryItem, other.Date));
        }
    }
}