using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("80d4616c-d083-4f8a-9ea9-b9dd5f04360f")]
    public sealed class SupplierSummary : Object, IHasCustomTheme
    {
        [Guide("The starting date for the period covered by the report.")]
        [ProtoMember(1), NoWrap] public DateTime FromDate { get; set; }
        [Guide("The ending date for the period covered by the report.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }
        [Guide("Select a division to filter suppliers, or leave blank to include all divisions.")]
        [ProtoMember(3), Autocomplete(typeof(Division))] public Guid? Division { get; set; }

        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(5), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
