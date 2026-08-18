using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("0e7711f8-1db1-4bcd-ac91-020d74a06dab")]
    public sealed class DivisionExceptionReport : Object, IHasCustomTheme
    {
        [Guide("Enter the start date for the report period.")]
        [Guide("The report will analyze division assignments from this date forward.")]
        [ProtoMember(1), NoWrap] public DateTime FromDate { get; set; }
        [Guide("Enter the end date for the report period.")]
        [Guide("All transactions up to and including this date will be checked for division issues.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }
        [Guide("Enter an optional description for this report.")]
        [Guide("This report identifies transactions that may have incorrect or missing division assignments.")]
        [Guide("It helps ensure all transactions are properly allocated to divisions for accurate divisional reporting.")]
        [ProtoMember(3), Placeholder(nameof(Strings.Optional))] public string Description { get; set; }

        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(5), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}