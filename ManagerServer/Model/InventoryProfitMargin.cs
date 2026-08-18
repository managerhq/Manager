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
    [Guid("796da7cf-3551-4ff4-afad-942d4fc750cc")]
    public sealed class InventoryProfitMargin : Object, IHasCustomTheme
    {
        [Guide("Enter an optional description for this report.")]
        [ProtoMember(3)] public string Description { get; set; }
        [Guide("Enter the start date for the profit analysis period.")]
        [ProtoMember(1)] public DateTime FromDate { get; set; }
        [Guide("Enter the end date for the profit analysis period.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }

        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(5), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
