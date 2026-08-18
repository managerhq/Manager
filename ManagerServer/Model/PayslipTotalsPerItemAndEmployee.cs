using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("13de6e5f-cacf-46a1-adb9-2250f76d77dd")]
    public sealed class PayslipTotalsPerItemAndEmployee : Object, IHasCustomTheme
    {
        [Guide("Enter an optional description for this report, such as 'Annual Payroll Analysis' or 'Department Payroll Breakdown'.")]
        [ProtoMember(3)] public string Description { get; set; }
        [Guide("Define the reporting periods for comparison. You can add multiple periods to compare payroll data across different time frames.")]
        [ProtoMember(4)] public Period[] Periods { get; set; }

        [ProtoMember(5), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(6), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [ProtoContract]
        public sealed class Period
        {
            [Guide("Select the starting date for this reporting period.")]
            [ProtoMember(1)] public DateTime FromDate { get; set; }
            [Guide("Select the ending date for this reporting period.")]
            [ProtoMember(2)] public DateTime ToDate { get; set; }
            [Guide("Enter a name for this period column in the report, such as 'Q1 2024' or 'January'.")]
            [ProtoMember(3)] public string ColumnName { get; set; }
        }

        [ProtoMember(1)] public DateTime Obsolete_From { get; set; }
        [ProtoMember(2)] public DateTime? Obsolete_To { get; set; }
    }
}
