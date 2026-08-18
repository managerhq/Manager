using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("b8583039-fa11-441a-a727-40aa2311e74c")]
    public sealed class CustomerSummary : Object, IHasCustomTheme
    {
        [Guide("Enter the start date for the summary period.")]
        [ProtoMember(1), NoWrap] public DateTime FromDate { get; set; }
        [Guide("Enter the end date for the summary period.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }
        [Guide("Select a division to filter the report to show only customers in that division.")]
        [ProtoMember(3), Autocomplete(typeof(Division))] public Guid? Division { get; set; }

        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(5), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
