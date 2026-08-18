using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("5516d6e5-d1fe-4271-8625-f7f0acc7f961")]
    public sealed class BillableTimeSummary : Object, IHasCustomTheme
    {
        [Guide("Enter the start date for the report period.")]
        [ProtoMember(1), NoWrap] public DateTime FromDate { get; set; }
        [Guide("Enter the end date for the report period.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }
        [Guide("Enter an optional description for this report.")]
        [ProtoMember(3)] public string Description { get; set; }

        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(5), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
