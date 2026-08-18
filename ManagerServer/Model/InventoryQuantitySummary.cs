using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("bd9e1f26-91e4-4c7b-b410-a62cb966bcfc")]
    public sealed class InventoryQuantitySummary : Object, IHasCustomTheme
    {
        [Guide("Enter an optional description or title for this report to help identify it later.")]
        [ProtoMember(3)] public string Description { get; set; }
        [Guide("Select the starting date for the inventory movement analysis. Opening balances will be calculated as of this date.")]
        [ProtoMember(1)] public DateTime FromDate { get; set; }
        [Guide("Select the ending date for the inventory movement analysis. All movements between the from and to dates will be included.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }
        [Guide("Check this box to hide inventory items that had no movement during the selected period. This helps focus on active inventory items only.")]
        [ProtoMember(4)] public bool ExcludeItemsWithNoMovement { get; set; }

        [ProtoMember(5), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(6), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
