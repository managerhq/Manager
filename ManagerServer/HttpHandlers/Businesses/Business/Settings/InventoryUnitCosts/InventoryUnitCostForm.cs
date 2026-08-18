using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Query;
using HttpFramework;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.InventoryUnitCosts
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryUnitCost))]
    [Guide("Set specific unit costs for inventory items on particular dates.")]
    [Guide("Unit costs affect inventory valuations and cost of goods sold calculations.")]
    [Fields(typeof(ManagerServer.Model.InventoryUnitCost))]
    internal sealed class InventoryUnitCostForm : NakedVueForm<ManagerServer.Model.InventoryUnitCost>
    {
        [ProtoMember(1)] public Guid? InventoryItem;
        [ProtoMember(2)] public DateTime? Date;
        [ProtoMember(3)] public decimal? UnitCost;

        protected override void OnSource(InventoryUnitCost form, ManagerServer.Model.Object source)
        {
            if (!Key.HasValue)
            {
                if (InventoryItem.HasValue) form.InventoryItem = InventoryItem.Value;
                if (Date.HasValue) form.Date = Date.Value;
                if (UnitCost.HasValue) form.UnitCost = UnitCost.Value;
            }
        }
    }
}