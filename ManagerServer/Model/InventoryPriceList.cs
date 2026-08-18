using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("14da35e1-9b94-4403-8575-9d64ade4d7b4")]
    public sealed class InventoryPriceList : Object, IHasCustomTheme
    {
        [Guide("Enter a name for this price list.")]
        [ProtoMember(1), Placeholder(nameof(Strings.InventoryPriceList))] public string Name { get; set; }
        [Guide("Check this box to show only items that match a specific custom field value.")]
        [ProtoMember(2)] public bool FilterByCustomField { get; set; }
        [Guide("Select the custom field to filter by.")]
        [ProtoMember(3), IfTrue(nameof(FilterByCustomField)), Prepend(nameof(Strings.CustomField)), NoLabel, NoWrap, Autocomplete(typeof(CustomField), Filter = typeof(ManagerServer.Model.InventoryItem))] public Guid? CustomField { get; set; }
        [Guide("Enter the custom field value to match.")]
        [ProtoMember(4), IfTrue(nameof(FilterByCustomField)), Prepend(nameof(Strings.Is)), NoLabel] public string Filter { get; set; }

        [ProtoMember(5), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(6), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}