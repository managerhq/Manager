using System;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("fa064288-9c14-404b-9dce-da46957d46e2")]
    public sealed class InventoryCostingCalculationWorksheet : Object, IHasCustomTheme
    {
        [Guide("Specify the date for which the figures should be calculated.")]
        [ProtoMember(1)] public DateTime Date { get; set; }

        [Guide("Enter a description for the report. This helps differentiate between various `InventoryCostingCalculationWorksheet` reports in the list.")]
        [ProtoMember(2), Long, Placeholder(nameof(Strings.Optional))] public string Description { get; set; }

        [Guide("Specify the valuation method as per which the figures should be calculated.")]
        [ProtoMember(3)] public InventoryValuationMethodWithoutManual ValuationMethod { get; set; }

        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(5), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
