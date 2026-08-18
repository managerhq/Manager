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
    [Guid("0e50586b-d1d0-40a3-81eb-8b6602e3050b")]
    public sealed class InventoryQuantityByLocation : Object, IHasCustomTheme
    {
        [Guide("Enter the date to show inventory quantities as of.")]
        [ProtoMember(1)] public DateTime Date { get; set; }
        [Guide("Enter an optional description for this report.")]
        [ProtoMember(2)] public string Description { get; set; }
        [Guide("Check this box to show specific inventory locations only.")]
        [ProtoMember(4)] public bool CustomInventoryLocations { get; set; }
        [Guide("Select the inventory locations to include in the report.")]
        [ProtoMember(3), Autocomplete(typeof(CustomInventoryLocation)), IfTrue(nameof(CustomInventoryLocations)), NoLabel] public Guid[] InventoryLocations { get; set; }

        [ProtoMember(5), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(6), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
